-- 006_attempt_scoring.sql
-- La version anterior de AttemptFinish puntuaba con el porcentaje bruto de aciertos,
-- lo que no coincide con el baremo oficial INAP que aplica el cliente (+1,00 / -0,33 / 0,00).
-- Se unifica el calculo en base de datos y se devuelve el desglose completo del intento.

DROP FUNCTION IF EXISTS AttemptFinish(BIGINT);

CREATE OR REPLACE FUNCTION AttemptFinish(p_AttemptId BIGINT)
RETURNS TABLE (
    AttemptId  BIGINT,
    Correct    INT,
    Wrong      INT,
    Blank      INT,
    Total      INT,
    Score      NUMERIC(5,2),
    FinishedAt TIMESTAMPTZ
)
LANGUAGE plpgsql
AS $$
#variable_conflict use_column
DECLARE
    v_Total    INT;
    v_Correct  INT;
    v_Wrong    INT;
    v_Blank    INT;
    v_Score    NUMERIC(5,2);
    v_Finished TIMESTAMPTZ;
BEGIN
    -- Numero real de preguntas asignadas al test del intento (no el solicitado al generarlo).
    SELECT COUNT(*)::INT
      INTO v_Total
      FROM Attempts a
      JOIN TestQuestions tq ON tq.TestId = a.TestId
     WHERE a.Id = p_AttemptId;

    IF COALESCE(v_Total, 0) = 0 THEN
        RETURN QUERY SELECT p_AttemptId, 0, 0, 0, 0, 0::NUMERIC(5,2), NULL::TIMESTAMPTZ;
        RETURN;
    END IF;

    -- Una respuesta sin opcion asociada (AnswerOptionId NULL) cuenta como blanco, no como fallo.
    SELECT
        COALESCE(SUM(CASE WHEN ao.IsCorrect THEN 1 ELSE 0 END), 0)::INT,
        COALESCE(SUM(CASE WHEN ao.Id IS NOT NULL AND NOT ao.IsCorrect THEN 1 ELSE 0 END), 0)::INT
      INTO v_Correct, v_Wrong
      FROM AttemptAnswers aa
      LEFT JOIN AnswerOptions ao ON ao.Id = aa.AnswerOptionId
     WHERE aa.AttemptId = p_AttemptId;

    v_Blank := GREATEST(v_Total - (v_Correct + v_Wrong), 0);

    -- Baremo oficial INAP: +1,00 por acierto, -0,33 por fallo, 0,00 en blanco. Nota sobre 10 sin negativos.
    v_Score := GREATEST(0, ROUND(((v_Correct - (v_Wrong * 0.33)) / v_Total) * 10, 2));

    UPDATE Attempts
       SET FinishedAt = CURRENT_TIMESTAMP,
           Score      = v_Score
     WHERE Id = p_AttemptId
    RETURNING Attempts.FinishedAt INTO v_Finished;

    RETURN QUERY SELECT p_AttemptId, v_Correct, v_Wrong, v_Blank, v_Total, v_Score, v_Finished;
END;
$$;

-- El motor de estudio pide N preguntas aleatorias filtrando por bloque, no por tema concreto.
CREATE INDEX IF NOT EXISTS IX_Questions_Active_Random
    ON Questions (IsActive, RandomKey) INCLUDE (Id, SyllabusTopicId);

-- Solo hay una opcion correcta por pregunta: se refuerza con indice unico parcial.
CREATE UNIQUE INDEX IF NOT EXISTS UX_AnswerOptions_UnicaCorrecta
    ON AnswerOptions (QuestionId) WHERE IsCorrect;

-- Un intento pertenece siempre al usuario que lo inicio; el filtro por propietario es constante.
CREATE INDEX IF NOT EXISTS IX_Attempts_UserName
    ON Attempts (UserName, StartedAt DESC);
