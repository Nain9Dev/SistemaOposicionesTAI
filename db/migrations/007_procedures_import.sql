/*
    007_procedures_import.sql
    Importacion idempotente de contenido.

    Estos procedimientos son los que permiten "ir rellenando" el banco: los ficheros JSON de
    content/ se pueden reimportar cuantas veces haga falta y el resultado es siempre el mismo,
    porque cada entidad se identifica por su clave de negocio (codigo de convocatoria, numero de
    tema, identificador externo de pregunta) y no por su Id autonumerico.
*/

SET NOCOUNT ON;
GO

CREATE OR ALTER PROCEDURE dbo.ExamUpsert
    @Code               NVARCHAR(20),
    @Name               NVARCHAR(300),
    @Authority          NVARCHAR(200)  = N'',
    @Description        NVARCHAR(1000) = N'',
    @SourceReference    NVARCHAR(400)  = NULL,
    @SourcePublication  NVARCHAR(200)  = NULL,
    @SourceUrl          NVARCHAR(500)  = NULL,
    @CorrectPoints      DECIMAL(9,4)   = 1.0,
    @IncorrectPoints    DECIMAL(9,4)   = -0.3333,
    @BlankPoints        DECIMAL(9,4)   = 0.0,
    @MaxScore           DECIMAL(9,2)   = 50.0,
    @PassMark           DECIMAL(9,2)   = 25.0,
    @QuestionCount      INT            = 80,
    @ReserveQuestions   INT            = 5,
    @DurationMinutes    INT            = 120,
    @OptionsPerQuestion INT            = 4
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Exams
    SET Name = @Name,
        Authority = @Authority,
        Description = @Description,
        SourceReference = @SourceReference,
        SourcePublication = @SourcePublication,
        SourceUrl = @SourceUrl,
        CorrectPoints = @CorrectPoints,
        IncorrectPoints = @IncorrectPoints,
        BlankPoints = @BlankPoints,
        MaxScore = @MaxScore,
        PassMark = @PassMark,
        QuestionCount = @QuestionCount,
        ReserveQuestions = @ReserveQuestions,
        DurationMinutes = @DurationMinutes,
        OptionsPerQuestion = @OptionsPerQuestion,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Code = @Code;

    IF @@ROWCOUNT = 0
    BEGIN
        INSERT INTO dbo.Exams
            (Code, Name, Authority, Description, SourceReference, SourcePublication, SourceUrl,
             CorrectPoints, IncorrectPoints, BlankPoints, MaxScore, PassMark,
             QuestionCount, ReserveQuestions, DurationMinutes, OptionsPerQuestion)
        VALUES
            (@Code, @Name, @Authority, @Description, @SourceReference, @SourcePublication, @SourceUrl,
             @CorrectPoints, @IncorrectPoints, @BlankPoints, @MaxScore, @PassMark,
             @QuestionCount, @ReserveQuestions, @DurationMinutes, @OptionsPerQuestion);
    END

    SELECT TOP (1) Id AS ExamId FROM dbo.Exams WHERE Code = @Code;
END
GO

CREATE OR ALTER PROCEDURE dbo.SyllabusBlockUpsert
    @ExamCode          NVARCHAR(20),
    @Code              NVARCHAR(10),
    @Name              NVARCHAR(300),
    @DisplayOrder      INT          = 0,
    @ExamWeightPercent DECIMAL(5,2) = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ExamId INT = (SELECT TOP (1) Id FROM dbo.Exams WHERE Code = @ExamCode);

    IF @ExamId IS NULL
    BEGIN
        THROW 51020, N'No existe la convocatoria indicada.', 1;
    END

    UPDATE dbo.SyllabusBlocks
    SET Name = @Name,
        DisplayOrder = @DisplayOrder,
        ExamWeightPercent = @ExamWeightPercent
    WHERE ExamId = @ExamId AND Code = @Code;

    IF @@ROWCOUNT = 0
    BEGIN
        INSERT INTO dbo.SyllabusBlocks (ExamId, Code, Name, DisplayOrder, ExamWeightPercent)
        VALUES (@ExamId, @Code, @Name, @DisplayOrder, @ExamWeightPercent);
    END

    SELECT TOP (1) Id AS BlockId FROM dbo.SyllabusBlocks WHERE ExamId = @ExamId AND Code = @Code;
END
GO

CREATE OR ALTER PROCEDURE dbo.SyllabusTopicUpsert
    @ExamCode    NVARCHAR(20),
    @BlockCode   NVARCHAR(10),
    @TopicNumber INT,
    @Title       NVARCHAR(600),
    @Slug        NVARCHAR(200)  = N'',
    @Keywords    NVARCHAR(1000) = N''
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @BlockId INT =
    (
        SELECT TOP (1) b.Id
        FROM dbo.SyllabusBlocks AS b
        INNER JOIN dbo.Exams AS e ON e.Id = b.ExamId
        WHERE e.Code = @ExamCode AND b.Code = @BlockCode
    );

    IF @BlockId IS NULL
    BEGIN
        THROW 51021, N'No existe el bloque indicado en esa convocatoria.', 1;
    END

    UPDATE dbo.SyllabusTopics
    SET Title = @Title,
        Slug = @Slug,
        Keywords = @Keywords
    WHERE BlockId = @BlockId AND TopicNumber = @TopicNumber;

    IF @@ROWCOUNT = 0
    BEGIN
        INSERT INTO dbo.SyllabusTopics (BlockId, TopicNumber, Title, Slug, Keywords)
        VALUES (@BlockId, @TopicNumber, @Title, @Slug, @Keywords);
    END

    SELECT TOP (1) Id AS TopicId
    FROM dbo.SyllabusTopics
    WHERE BlockId = @BlockId AND TopicNumber = @TopicNumber;
END
GO

/*
    Alta o actualizacion de una pregunta con sus opciones y etiquetas.

    Las opciones se sincronizan por SortOrder en lugar de borrarse y reinsertarse: sus Id ya pueden
    estar referenciados por tests generados e intentos respondidos, y perderlos reescribiria el
    historial del opositor. Solo se eliminan las opciones sobrantes que nadie referencia.
*/
CREATE OR ALTER PROCEDURE dbo.QuestionUpsert
    @ExternalId        NVARCHAR(80),
    @ExamCode          NVARCHAR(20),
    @BlockCode         NVARCHAR(10),
    @TopicNumber       INT,
    @Difficulty        TINYINT,
    @Statement         NVARCHAR(2000),
    @Explanation       NVARCHAR(4000) = NULL,
    @SourceReference   NVARCHAR(400)  = NULL,
    @SourcePublication NVARCHAR(200)  = NULL,
    @SourceUrl         NVARCHAR(500)  = NULL,
    @IsActive          BIT            = 1,
    @Options           dbo.AnswerOptionList READONLY,
    @Tags              dbo.TagList READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @TopicId INT =
    (
        SELECT TOP (1) t.Id
        FROM dbo.SyllabusTopics AS t
        INNER JOIN dbo.SyllabusBlocks AS b ON b.Id = t.BlockId
        INNER JOIN dbo.Exams AS e ON e.Id = b.ExamId
        WHERE e.Code = @ExamCode AND b.Code = @BlockCode AND t.TopicNumber = @TopicNumber
    );

    IF @TopicId IS NULL
    BEGIN
        THROW 51022, N'No existe el tema indicado en el temario.', 1;
    END

    IF NOT EXISTS (SELECT 1 FROM @Options WHERE IsCorrect = 1)
    BEGIN
        THROW 51023, N'La pregunta debe declarar una opcion correcta.', 1;
    END

    IF (SELECT COUNT(1) FROM @Options WHERE IsCorrect = 1) > 1
    BEGIN
        THROW 51024, N'La pregunta no puede tener mas de una opcion correcta.', 1;
    END

    BEGIN TRANSACTION;

    UPDATE dbo.Questions
    SET SyllabusTopicId = @TopicId,
        Difficulty = @Difficulty,
        Statement = @Statement,
        Explanation = @Explanation,
        SourceReference = @SourceReference,
        SourcePublication = @SourcePublication,
        SourceUrl = @SourceUrl,
        IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME()
    WHERE ExternalId = @ExternalId;

    IF @@ROWCOUNT = 0
    BEGIN
        INSERT INTO dbo.Questions
            (ExternalId, SyllabusTopicId, Difficulty, Statement, Explanation,
             SourceReference, SourcePublication, SourceUrl, IsActive)
        VALUES
            (@ExternalId, @TopicId, @Difficulty, @Statement, @Explanation,
             @SourceReference, @SourcePublication, @SourceUrl, @IsActive);
    END

    DECLARE @QuestionId BIGINT = (SELECT TOP (1) Id FROM dbo.Questions WHERE ExternalId = @ExternalId);

    UPDATE ao
    SET OptionText = o.OptionText,
        IsCorrect = o.IsCorrect
    FROM dbo.AnswerOptions AS ao
    INNER JOIN @Options AS o ON o.SortOrder = ao.SortOrder
    WHERE ao.QuestionId = @QuestionId;

    INSERT INTO dbo.AnswerOptions (QuestionId, SortOrder, OptionText, IsCorrect)
    SELECT @QuestionId, o.SortOrder, o.OptionText, o.IsCorrect
    FROM @Options AS o
    WHERE NOT EXISTS (SELECT 1 FROM dbo.AnswerOptions AS ao
                      WHERE ao.QuestionId = @QuestionId AND ao.SortOrder = o.SortOrder);

    DELETE ao
    FROM dbo.AnswerOptions AS ao
    WHERE ao.QuestionId = @QuestionId
      AND NOT EXISTS (SELECT 1 FROM @Options AS o WHERE o.SortOrder = ao.SortOrder)
      AND NOT EXISTS (SELECT 1 FROM dbo.AttemptAnswers AS aa WHERE aa.AnswerOptionId = ao.Id)
      AND NOT EXISTS (SELECT 1 FROM dbo.TestQuestionOptions AS tqo WHERE tqo.AnswerOptionId = ao.Id);

    INSERT INTO dbo.Tags (Name)
    SELECT DISTINCT t.Name
    FROM @Tags AS t
    WHERE NOT EXISTS (SELECT 1 FROM dbo.Tags AS existing WHERE existing.Name = t.Name);

    DELETE qt
    FROM dbo.QuestionTags AS qt
    INNER JOIN dbo.Tags AS tg ON tg.Id = qt.TagId
    WHERE qt.QuestionId = @QuestionId
      AND NOT EXISTS (SELECT 1 FROM @Tags AS t WHERE t.Name = tg.Name);

    INSERT INTO dbo.QuestionTags (QuestionId, TagId)
    SELECT @QuestionId, tg.Id
    FROM @Tags AS t
    INNER JOIN dbo.Tags AS tg ON tg.Name = t.Name
    WHERE NOT EXISTS (SELECT 1 FROM dbo.QuestionTags AS qt
                      WHERE qt.QuestionId = @QuestionId AND qt.TagId = tg.Id);

    COMMIT TRANSACTION;

    SELECT @QuestionId AS QuestionId;
END
GO
