/*
    005_procedures_tests.sql
    Creacion y lectura de tests generados.
*/

SET NOCOUNT ON;
GO

/*
    Crea el test con sus preguntas y el orden barajado de opciones.

    El baremo se copia sobre la fila del test en lugar de leerse de la convocatoria al corregir:
    un examen realizado hace meses debe seguir corrigiendose con las reglas que tenia entonces.
    Todo se ejecuta en una transaccion para que un test nunca quede a medias.
*/
CREATE OR ALTER PROCEDURE dbo.TestCreate
    @ExamCode        NVARCHAR(20),
    @Title           NVARCHAR(300),
    @Mode            TINYINT,
    @Seed            INT,
    @DurationMinutes INT,
    @CorrectPoints   DECIMAL(9,4),
    @IncorrectPoints DECIMAL(9,4),
    @BlankPoints     DECIMAL(9,4),
    @MaxScore        DECIMAL(9,2),
    @PassMark        DECIMAL(9,2),
    @BlueprintJson   NVARCHAR(MAX) = NULL,
    @Questions       dbo.TestQuestionList READONLY,
    @Options         dbo.TestQuestionOptionList READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ExamId INT = (SELECT TOP (1) Id FROM dbo.Exams WHERE Code = @ExamCode);

    IF @ExamId IS NULL
    BEGIN
        THROW 51001, N'No existe la convocatoria indicada.', 1;
    END

    DECLARE @Total INT = (SELECT COUNT(1) FROM @Questions);

    IF @Total = 0
    BEGIN
        THROW 51002, N'No se puede crear un test sin preguntas.', 1;
    END

    BEGIN TRANSACTION;

    INSERT INTO dbo.Tests
        (ExamId, Title, Mode, TotalQuestions, Seed, DurationMinutes,
         CorrectPoints, IncorrectPoints, BlankPoints, MaxScore, PassMark, BlueprintJson)
    VALUES
        (@ExamId, @Title, @Mode, @Total, @Seed, @DurationMinutes,
         @CorrectPoints, @IncorrectPoints, @BlankPoints, @MaxScore, @PassMark, @BlueprintJson);

    DECLARE @TestId BIGINT = CAST(SCOPE_IDENTITY() AS BIGINT);

    INSERT INTO dbo.TestQuestions (TestId, QuestionId, SortOrder)
    SELECT @TestId, q.QuestionId, q.SortOrder
    FROM @Questions AS q;

    INSERT INTO dbo.TestQuestionOptions (TestId, QuestionId, AnswerOptionId, SortOrder)
    SELECT @TestId, o.QuestionId, o.AnswerOptionId, o.SortOrder
    FROM @Options AS o;

    COMMIT TRANSACTION;

    EXEC dbo.TestGet @TestId = @TestId;
END
GO

/*
    Devuelve el test en tres conjuntos: cabecera, preguntas y opciones.

    Las opciones respetan el orden barajado que se guardo al crear el test (TestQuestionOptions);
    si un test antiguo no lo tiene, se cae al orden natural de la pregunta.
*/
CREATE OR ALTER PROCEDURE dbo.TestGet
    @TestId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  t.Id,
            e.Code AS ExamCode,
            t.Title,
            t.Mode,
            t.Seed,
            t.DurationMinutes,
            t.TotalQuestions,
            t.CorrectPoints,
            t.IncorrectPoints,
            t.BlankPoints,
            t.MaxScore,
            t.PassMark,
            t.CreatedAt
    FROM dbo.Tests AS t
    INNER JOIN dbo.Exams AS e ON e.Id = t.ExamId
    WHERE t.Id = @TestId;

    SELECT  q.Id,
            q.ExternalId,
            q.SyllabusTopicId AS TopicId,
            e.Code AS ExamCode,
            b.Code AS BlockCode,
            sc.TopicNumber,
            sc.Title AS TopicTitle,
            q.Difficulty,
            q.Statement,
            q.Explanation,
            q.SourceReference,
            q.SourcePublication,
            q.SourceUrl,
            q.IsActive,
            STUFF((SELECT ',' + tg.Name
                   FROM dbo.QuestionTags AS qt
                   INNER JOIN dbo.Tags AS tg ON tg.Id = qt.TagId
                   WHERE qt.QuestionId = q.Id
                   ORDER BY tg.Name
                   FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, N'') AS Tags
    FROM dbo.TestQuestions AS tq
    INNER JOIN dbo.Questions      AS q  ON q.Id = tq.QuestionId
    INNER JOIN dbo.SyllabusTopics AS sc ON sc.Id = q.SyllabusTopicId
    INNER JOIN dbo.SyllabusBlocks AS b  ON b.Id = sc.BlockId
    INNER JOIN dbo.Exams          AS e  ON e.Id = b.ExamId
    WHERE tq.TestId = @TestId
    ORDER BY tq.SortOrder;

    SELECT  ao.QuestionId,
            ao.Id,
            CAST(ISNULL(tqo.SortOrder, ao.SortOrder) AS TINYINT) AS SortOrder,
            ao.OptionText,
            ao.IsCorrect
    FROM dbo.TestQuestions AS tq
    INNER JOIN dbo.AnswerOptions AS ao ON ao.QuestionId = tq.QuestionId
    LEFT  JOIN dbo.TestQuestionOptions AS tqo
            ON tqo.TestId = tq.TestId
           AND tqo.QuestionId = ao.QuestionId
           AND tqo.AnswerOptionId = ao.Id
    WHERE tq.TestId = @TestId
    ORDER BY ao.QuestionId, ISNULL(tqo.SortOrder, ao.SortOrder);
END
GO
