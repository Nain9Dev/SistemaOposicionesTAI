CREATE OR ALTER PROCEDURE dbo.TestGenerate
    @Title NVARCHAR(200),
    @SyllabusTopicId INT,
    @Difficulty TINYINT,
    @TotalQuestions INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Seed INT = ABS(CHECKSUM(NEWID()));
    DECLARE @Pivot INT = ABS(CHECKSUM(@Seed, SYSUTCDATETIME()));

    INSERT INTO dbo.Tests (Title, TotalQuestions, Seed)
    VALUES (@Title, @TotalQuestions, @Seed);

    DECLARE @TestId BIGINT = SCOPE_IDENTITY();

    ;WITH PickA AS
    (
        SELECT TOP (@TotalQuestions) q.Id
        FROM dbo.Questions q WITH (INDEX(IX_Questions_SelectFast))
        WHERE q.SyllabusTopicId = @SyllabusTopicId
          AND q.Difficulty = @Difficulty
          AND q.IsActive = 1
          AND q.RandomKey >= @Pivot
        ORDER BY q.RandomKey
    ),
    PickB AS
    (
        SELECT TOP (@TotalQuestions) q.Id
        FROM dbo.Questions q WITH (INDEX(IX_Questions_SelectFast))
        WHERE q.SyllabusTopicId = @SyllabusTopicId
          AND q.Difficulty = @Difficulty
          AND q.IsActive = 1
          AND q.RandomKey < @Pivot
          AND NOT EXISTS (SELECT 1 FROM PickA a WHERE a.Id = q.Id)
        ORDER BY q.RandomKey
    ),
    FinalPick AS
    (
        SELECT Id FROM PickA
        UNION ALL
        SELECT Id FROM PickB
    )
    INSERT INTO dbo.TestQuestions (TestId, QuestionId, SortOrder)
    SELECT @TestId, fp.Id, ROW_NUMBER() OVER (ORDER BY (SELECT 1))
    FROM FinalPick fp;

    SELECT @TestId AS TestId;
END