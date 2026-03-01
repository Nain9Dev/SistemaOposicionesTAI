CREATE OR ALTER PROCEDURE dbo.AttemptStart
    @TestId BIGINT,
    @UserName NVARCHAR(120)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Attempts(TestId, UserName)
    VALUES (@TestId, @UserName);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS AttemptId;
END
GO

CREATE OR ALTER PROCEDURE dbo.AttemptAnswerUpsert
    @AttemptId BIGINT,
    @QuestionId BIGINT,
    @AnswerOptionId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.AttemptAnswers WHERE AttemptId=@AttemptId AND QuestionId=@QuestionId)
    BEGIN
        UPDATE dbo.AttemptAnswers
        SET AnswerOptionId = @AnswerOptionId,
            AnsweredAt = SYSUTCDATETIME()
        WHERE AttemptId=@AttemptId AND QuestionId=@QuestionId;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.AttemptAnswers(AttemptId, QuestionId, AnswerOptionId)
        VALUES (@AttemptId, @QuestionId, @AnswerOptionId);
    END

    SELECT 1 AS Ok;
END
GO

CREATE OR ALTER PROCEDURE dbo.AttemptFinish
    @AttemptId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH Answers AS
    (
        SELECT aa.AttemptId, aa.QuestionId, aa.AnswerOptionId
        FROM dbo.AttemptAnswers aa
        WHERE aa.AttemptId = @AttemptId
    ),
    Correct AS
    (
        SELECT a.AttemptId,
               SUM(CASE WHEN ao.IsCorrect = 1 THEN 1 ELSE 0 END) AS CorrectCount,
               COUNT(*) AS AnsweredCount
        FROM Answers a
        JOIN dbo.AnswerOptions ao ON ao.Id = a.AnswerOptionId
        GROUP BY a.AttemptId
    ),
    Total AS
    (
        SELECT a.Id AS AttemptId, t.TotalQuestions
        FROM dbo.Attempts a
        JOIN dbo.Tests t ON t.Id = a.TestId
        WHERE a.Id = @AttemptId
    )
    UPDATE a
    SET FinishedAt = SYSUTCDATETIME(),
        Score = CASE WHEN tot.TotalQuestions = 0 THEN 0
                     ELSE CAST(100.0 * ISNULL(c.CorrectCount,0) / tot.TotalQuestions AS DECIMAL(5,2))
                END
    FROM dbo.Attempts a
    JOIN Total tot ON tot.AttemptId = a.Id
    LEFT JOIN Correct c ON c.AttemptId = a.Id
    WHERE a.Id = @AttemptId;

    SELECT Id AS AttemptId, Score, FinishedAt
    FROM dbo.Attempts
    WHERE Id = @AttemptId;
END
GO