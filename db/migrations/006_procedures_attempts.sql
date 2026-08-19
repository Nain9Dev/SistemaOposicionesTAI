/*
    006_procedures_attempts.sql
    Ciclo de vida de los intentos y analitica de rendimiento.

    La nota no se calcula aqui: la base guarda respuestas y resultados, y la correccion vive en el
    dominio (.NET), donde el baremo es un dato configurable de la convocatoria. Cambiar la
    penalizacion no obliga a reescribir ningun procedimiento almacenado.
*/

SET NOCOUNT ON;
GO

CREATE OR ALTER PROCEDURE dbo.AttemptStart
    @TestId   BIGINT,
    @UserName NVARCHAR(120)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Tests WHERE Id = @TestId)
    BEGIN
        THROW 51010, N'No existe el test indicado.', 1;
    END

    INSERT INTO dbo.Attempts (TestId, UserName)
    VALUES (@TestId, @UserName);

    DECLARE @AttemptId BIGINT = CAST(SCOPE_IDENTITY() AS BIGINT);

    EXEC dbo.AttemptGet @AttemptId = @AttemptId;
END
GO

CREATE OR ALTER PROCEDURE dbo.AttemptGet
    @AttemptId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  a.Id,
            a.TestId,
            a.UserName,
            e.Code AS ExamCode,
            t.Title AS TestTitle,
            t.Mode,
            a.StartedAt,
            a.FinishedAt,
            a.TotalQuestions,
            a.CorrectCount,
            a.IncorrectCount,
            a.BlankCount,
            a.RawScore,
            a.ScaledScore,
            a.MaxScore,
            a.PassMark,
            a.AccuracyPercent,
            a.Passed
    FROM dbo.Attempts AS a
    INNER JOIN dbo.Tests AS t ON t.Id = a.TestId
    INNER JOIN dbo.Exams AS e ON e.Id = t.ExamId
    WHERE a.Id = @AttemptId;
END
GO

/*
    Registra o sustituye la respuesta a una pregunta.

    @AnswerOptionId nulo deja la pregunta en blanco de forma explicita, que con un baremo que
    penaliza los fallos es una decision de examen y no un dato ausente. Se valida que la pregunta
    pertenezca al test y la opcion a la pregunta: el cliente no puede inventar respuestas.
*/
CREATE OR ALTER PROCEDURE dbo.AttemptAnswerUpsert
    @AttemptId      BIGINT,
    @QuestionId     BIGINT,
    @AnswerOptionId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TestId BIGINT = (SELECT TOP (1) TestId FROM dbo.Attempts WHERE Id = @AttemptId);

    IF @TestId IS NULL
    BEGIN
        THROW 51011, N'No existe el intento indicado.', 1;
    END

    IF EXISTS (SELECT 1 FROM dbo.Attempts WHERE Id = @AttemptId AND FinishedAt IS NOT NULL)
    BEGIN
        THROW 51012, N'El intento ya esta finalizado y no admite mas respuestas.', 1;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.TestQuestions WHERE TestId = @TestId AND QuestionId = @QuestionId)
    BEGIN
        THROW 51013, N'La pregunta no pertenece al test del intento.', 1;
    END

    IF @AnswerOptionId IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.AnswerOptions
                       WHERE Id = @AnswerOptionId AND QuestionId = @QuestionId)
    BEGIN
        THROW 51014, N'La opcion no pertenece a la pregunta.', 1;
    END

    UPDATE dbo.AttemptAnswers
    SET AnswerOptionId = @AnswerOptionId,
        AnsweredAt = SYSUTCDATETIME()
    WHERE AttemptId = @AttemptId
      AND QuestionId = @QuestionId;

    IF @@ROWCOUNT = 0
    BEGIN
        INSERT INTO dbo.AttemptAnswers (AttemptId, QuestionId, AnswerOptionId)
        VALUES (@AttemptId, @QuestionId, @AnswerOptionId);
    END

    SELECT 1 AS Ok;
END
GO

/*
    Hoja de respuestas completa.

    Parte de las preguntas del test y no de las respuestas registradas: las preguntas sin contestar
    tienen que aparecer como filas en blanco, porque en el baremo oficial no penalizan y hay que
    contarlas como tales.
*/
CREATE OR ALTER PROCEDURE dbo.AttemptAnswerSheet
    @AttemptId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  a.Id AS AttemptId,
            a.TestId,
            e.Code AS ExamCode,
            t.CorrectPoints,
            t.IncorrectPoints,
            t.BlankPoints,
            t.MaxScore,
            t.PassMark
    FROM dbo.Attempts AS a
    INNER JOIN dbo.Tests AS t ON t.Id = a.TestId
    INNER JOIN dbo.Exams AS e ON e.Id = t.ExamId
    WHERE a.Id = @AttemptId;

    SELECT  tq.QuestionId,
            aa.AnswerOptionId AS SelectedOptionId,
            (SELECT TOP (1) ao.Id
             FROM dbo.AnswerOptions AS ao
             WHERE ao.QuestionId = tq.QuestionId AND ao.IsCorrect = 1) AS CorrectOptionId,
            b.Code AS BlockCode,
            b.Name AS BlockName,
            sc.TopicNumber,
            sc.Title AS TopicTitle
    FROM dbo.Attempts AS a
    INNER JOIN dbo.TestQuestions  AS tq ON tq.TestId = a.TestId
    INNER JOIN dbo.Questions      AS q  ON q.Id = tq.QuestionId
    INNER JOIN dbo.SyllabusTopics AS sc ON sc.Id = q.SyllabusTopicId
    INNER JOIN dbo.SyllabusBlocks AS b  ON b.Id = sc.BlockId
    LEFT  JOIN dbo.AttemptAnswers AS aa ON aa.AttemptId = a.Id AND aa.QuestionId = tq.QuestionId
    WHERE a.Id = @AttemptId
    ORDER BY tq.SortOrder;
END
GO

/* Cierra el intento almacenando el desglose de la nota ya calculado por el dominio. */
CREATE OR ALTER PROCEDURE dbo.AttemptComplete
    @AttemptId       BIGINT,
    @FinishedAt      DATETIME2(3),
    @TotalQuestions  INT,
    @CorrectCount    INT,
    @IncorrectCount  INT,
    @BlankCount      INT,
    @RawScore        DECIMAL(9,3),
    @ScaledScore     DECIMAL(9,3),
    @MaxScore        DECIMAL(9,2),
    @PassMark        DECIMAL(9,2),
    @AccuracyPercent DECIMAL(6,2),
    @Passed          BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Attempts
    SET FinishedAt      = @FinishedAt,
        TotalQuestions  = @TotalQuestions,
        CorrectCount    = @CorrectCount,
        IncorrectCount  = @IncorrectCount,
        BlankCount      = @BlankCount,
        RawScore        = @RawScore,
        ScaledScore     = @ScaledScore,
        MaxScore        = @MaxScore,
        PassMark        = @PassMark,
        AccuracyPercent = @AccuracyPercent,
        Passed          = @Passed
    WHERE Id = @AttemptId
      AND FinishedAt IS NULL;

    SELECT @@ROWCOUNT AS Updated;
END
GO

/* Historial paginado de un usuario. */
CREATE OR ALTER PROCEDURE dbo.AttemptHistory
    @UserName NVARCHAR(120),
    @ExamCode NVARCHAR(20) = NULL,
    @Offset   INT = 0,
    @PageSize INT = 25
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  a.Id AS AttemptId,
            a.TestId,
            t.Title AS TestTitle,
            e.Code AS ExamCode,
            t.Mode,
            a.StartedAt,
            a.FinishedAt,
            ISNULL(a.TotalQuestions, t.TotalQuestions) AS TotalQuestions,
            ISNULL(a.CorrectCount, 0)   AS Correct,
            ISNULL(a.IncorrectCount, 0) AS Incorrect,
            ISNULL(a.BlankCount, 0)     AS Blank,
            a.ScaledScore,
            a.AccuracyPercent
    FROM dbo.Attempts AS a
    INNER JOIN dbo.Tests AS t ON t.Id = a.TestId
    INNER JOIN dbo.Exams AS e ON e.Id = t.ExamId
    WHERE a.UserName = @UserName
      AND (@ExamCode IS NULL OR e.Code = @ExamCode)
    ORDER BY a.StartedAt DESC, a.Id DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT_BIG(1) AS TotalItems
    FROM dbo.Attempts AS a
    INNER JOIN dbo.Tests AS t ON t.Id = a.TestId
    INNER JOIN dbo.Exams AS e ON e.Id = t.ExamId
    WHERE a.UserName = @UserName
      AND (@ExamCode IS NULL OR e.Code = @ExamCode);
END
GO

/*
    Rendimiento acumulado por tema.
    Solo cuenta intentos cerrados: incluir uno a medias contaria como fallos preguntas que el
    opositor todavia no ha llegado a leer.
*/
CREATE OR ALTER PROCEDURE dbo.AttemptTopicPerformance
    @UserName NVARCHAR(120),
    @ExamCode NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    WITH Rows_CTE AS
    (
        SELECT  b.Code AS BlockCode,
                sc.TopicNumber,
                sc.Title AS TopicTitle,
                CASE WHEN aa.AnswerOptionId IS NULL THEN 1 ELSE 0 END AS IsBlank,
                CASE WHEN aa.AnswerOptionId IS NOT NULL AND ao.IsCorrect = 1 THEN 1 ELSE 0 END AS IsCorrect,
                CASE WHEN aa.AnswerOptionId IS NOT NULL AND ISNULL(ao.IsCorrect, 0) = 0 THEN 1 ELSE 0 END AS IsIncorrect
        FROM dbo.Attempts AS a
        INNER JOIN dbo.Tests          AS t  ON t.Id = a.TestId
        INNER JOIN dbo.Exams          AS e  ON e.Id = t.ExamId
        INNER JOIN dbo.TestQuestions  AS tq ON tq.TestId = a.TestId
        INNER JOIN dbo.Questions      AS q  ON q.Id = tq.QuestionId
        INNER JOIN dbo.SyllabusTopics AS sc ON sc.Id = q.SyllabusTopicId
        INNER JOIN dbo.SyllabusBlocks AS b  ON b.Id = sc.BlockId
        LEFT  JOIN dbo.AttemptAnswers AS aa ON aa.AttemptId = a.Id AND aa.QuestionId = tq.QuestionId
        LEFT  JOIN dbo.AnswerOptions  AS ao ON ao.Id = aa.AnswerOptionId
        WHERE a.UserName = @UserName
          AND a.FinishedAt IS NOT NULL
          AND (@ExamCode IS NULL OR e.Code = @ExamCode)
    )
    SELECT  CONCAT(BlockCode, '.', TopicNumber) AS [Key],
            CONCAT(N'Tema ', TopicNumber, N'. ', MAX(TopicTitle)) AS Label,
            BlockCode,
            TopicNumber,
            COUNT(1) AS TotalQuestions,
            SUM(IsCorrect) AS Correct,
            SUM(IsIncorrect) AS Incorrect,
            SUM(IsBlank) AS Blank,
            CAST(CASE WHEN COUNT(1) = 0 THEN 0
                      ELSE 100.0 * SUM(IsCorrect) / COUNT(1) END AS DECIMAL(6,2)) AS AccuracyPercent
    FROM Rows_CTE
    GROUP BY BlockCode, TopicNumber
    ORDER BY BlockCode, TopicNumber;
END
GO
