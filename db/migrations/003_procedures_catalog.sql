/*
    003_procedures_catalog.sql
    Lectura del catalogo: convocatorias, bloques y temas.
*/

SET NOCOUNT ON;
GO

CREATE OR ALTER PROCEDURE dbo.ExamList
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  e.Id,
            e.Code,
            e.Name,
            e.Authority,
            e.Description,
            e.SourceReference,
            e.SourcePublication,
            e.SourceUrl,
            e.CorrectPoints,
            e.IncorrectPoints,
            e.BlankPoints,
            e.MaxScore,
            e.PassMark,
            e.QuestionCount,
            e.ReserveQuestions,
            e.DurationMinutes,
            e.OptionsPerQuestion,
            e.IsActive
    FROM dbo.Exams AS e
    WHERE e.IsActive = 1
    ORDER BY e.Code;
END
GO

/*
    Devuelve tres conjuntos de resultados: convocatoria, bloques y temas.
    Se resuelve en una sola llamada porque el cliente siempre necesita el temario completo para
    poder generar un test, y tres viajes distintos solo anadirian latencia.
*/
CREATE OR ALTER PROCEDURE dbo.ExamGet
    @Code NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ExamId INT = (SELECT TOP (1) Id FROM dbo.Exams WHERE Code = @Code);

    SELECT  e.Id,
            e.Code,
            e.Name,
            e.Authority,
            e.Description,
            e.SourceReference,
            e.SourcePublication,
            e.SourceUrl,
            e.CorrectPoints,
            e.IncorrectPoints,
            e.BlankPoints,
            e.MaxScore,
            e.PassMark,
            e.QuestionCount,
            e.ReserveQuestions,
            e.DurationMinutes,
            e.OptionsPerQuestion,
            e.IsActive
    FROM dbo.Exams AS e
    WHERE e.Id = @ExamId;

    SELECT  b.Id,
            b.ExamId,
            @Code AS ExamCode,
            b.Code,
            b.Name,
            b.DisplayOrder,
            b.ExamWeightPercent
    FROM dbo.SyllabusBlocks AS b
    WHERE b.ExamId = @ExamId
    ORDER BY b.DisplayOrder, b.Code;

    SELECT  t.Id,
            t.BlockId,
            b.Code AS BlockCode,
            @Code AS ExamCode,
            t.TopicNumber AS Number,
            t.Title,
            t.Slug,
            t.Keywords
    FROM dbo.SyllabusTopics AS t
    INNER JOIN dbo.SyllabusBlocks AS b ON b.Id = t.BlockId
    WHERE b.ExamId = @ExamId
    ORDER BY b.DisplayOrder, t.TopicNumber;
END
GO

CREATE OR ALTER PROCEDURE dbo.SyllabusTopicList
    @ExamCode  NVARCHAR(20),
    @BlockCode NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  t.Id,
            t.BlockId,
            b.Code AS BlockCode,
            e.Code AS ExamCode,
            t.TopicNumber AS Number,
            t.Title,
            t.Slug,
            t.Keywords
    FROM dbo.SyllabusTopics AS t
    INNER JOIN dbo.SyllabusBlocks AS b ON b.Id = t.BlockId
    INNER JOIN dbo.Exams AS e ON e.Id = b.ExamId
    WHERE e.Code = @ExamCode
      AND (@BlockCode IS NULL OR b.Code = @BlockCode)
    ORDER BY b.DisplayOrder, t.TopicNumber;
END
GO
