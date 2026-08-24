CREATE OR REPLACE FUNCTION AttemptStart(
    p_TestId BIGINT,
    p_UserName VARCHAR(120)
)
RETURNS BIGINT
LANGUAGE plpgsql
AS $$
DECLARE
    v_AttemptId BIGINT;
BEGIN
    INSERT INTO Attempts(TestId, UserName)
    VALUES (p_TestId, p_UserName)
    RETURNING Id INTO v_AttemptId;
    
    RETURN v_AttemptId;
END;
$$;

CREATE OR REPLACE FUNCTION AttemptAnswerUpsert(
    p_AttemptId BIGINT,
    p_QuestionId BIGINT,
    p_AnswerOptionId BIGINT
)
RETURNS INT
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO AttemptAnswers(AttemptId, QuestionId, AnswerOptionId)
    VALUES (p_AttemptId, p_QuestionId, p_AnswerOptionId)
    ON CONFLICT (AttemptId, QuestionId)
    DO UPDATE SET 
        AnswerOptionId = EXCLUDED.AnswerOptionId,
        AnsweredAt = CURRENT_TIMESTAMP;
    
    RETURN 1;
END;
$$;

CREATE OR REPLACE FUNCTION AttemptFinish(
    p_AttemptId BIGINT
)
RETURNS TABLE (AttemptId BIGINT, Score DECIMAL(5,2), FinishedAt TIMESTAMP)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE Attempts a
    SET FinishedAt = CURRENT_TIMESTAMP,
        Score = CASE 
            WHEN tot.TotalQuestions = 0 THEN 0
            ELSE (100.0 * COALESCE(c.CorrectCount, 0)) / tot.TotalQuestions 
        END
    FROM Tests tot
    LEFT JOIN (
        SELECT aa.AttemptId,
               SUM(CASE WHEN ao.IsCorrect THEN 1 ELSE 0 END) AS CorrectCount
        FROM AttemptAnswers aa
        JOIN AnswerOptions ao ON ao.Id = aa.AnswerOptionId
        WHERE aa.AttemptId = p_AttemptId
        GROUP BY aa.AttemptId
    ) c ON c.AttemptId = p_AttemptId
    WHERE a.Id = p_AttemptId 
      AND a.TestId = tot.Id;

    RETURN QUERY
    SELECT Id, a.Score, a.FinishedAt
    FROM Attempts a
    WHERE Id = p_AttemptId;
END;
$$;