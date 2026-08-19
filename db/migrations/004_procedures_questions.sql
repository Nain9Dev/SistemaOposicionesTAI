/*
    004_procedures_questions.sql
    Consulta y extraccion del banco de preguntas.

    Las listas (dificultades, etiquetas, exclusiones) llegan como cadenas separadas por comas y se
    expanden con STRING_SPLIT. Evita construir SQL dinamico, de modo que todo sigue siendo
    parametrizado y no hay superficie de inyeccion.
*/

SET NOCOUNT ON;
GO

/*
    Extraccion aleatoria y reproducible.

    El orden no se deja a NEWID(): se deriva de un hash de (Id de pregunta, semilla). Con la misma
    semilla y el mismo banco, la extraccion es identica, que es lo que permite compartir un examen
    por su semilla o reproducir exactamente el de ayer.
*/
CREATE OR ALTER PROCEDURE dbo.QuestionsDraw
    @ExamCode     NVARCHAR(20),
    @BlockCode    NVARCHAR(10)   = NULL,
    @TopicNumber  INT            = NULL,
    @TopicId      INT            = NULL,
    @Difficulties VARCHAR(50)    = NULL,
    @Tags         NVARCHAR(1000) = NULL,
    @ExcludeIds   VARCHAR(MAX)   = NULL,
    @Count        INT            = 20,
    @Seed         INT            = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF @Count IS NULL OR @Count < 1
    BEGIN
        SELECT TOP (0) CAST(NULL AS BIGINT) AS Id;
        SELECT TOP (0) CAST(NULL AS BIGINT) AS Id;
        RETURN;
    END

    DECLARE @TagCount INT =
        CASE
            WHEN @Tags IS NULL OR LEN(@Tags) = 0 THEN 0
            ELSE (SELECT COUNT(DISTINCT LTRIM(RTRIM(value)))
                  FROM STRING_SPLIT(@Tags, ',')
                  WHERE LTRIM(RTRIM(value)) <> N'')
        END;

    CREATE TABLE #Picked (Id BIGINT NOT NULL PRIMARY KEY);

    INSERT INTO #Picked (Id)
    SELECT TOP (@Count) q.Id
    FROM dbo.Questions AS q
    INNER JOIN dbo.SyllabusTopics AS t ON t.Id = q.SyllabusTopicId
    INNER JOIN dbo.SyllabusBlocks AS b ON b.Id = t.BlockId
    INNER JOIN dbo.Exams          AS e ON e.Id = b.ExamId
    WHERE q.IsActive = 1
      AND e.Code = @ExamCode
      AND (@BlockCode   IS NULL OR b.Code = @BlockCode)
      AND (@TopicNumber IS NULL OR t.TopicNumber = @TopicNumber)
      AND (@TopicId     IS NULL OR t.Id = @TopicId)
      AND (@Difficulties IS NULL OR LEN(@Difficulties) = 0 OR q.Difficulty IN
            (SELECT TRY_CAST(value AS TINYINT) FROM STRING_SPLIT(@Difficulties, ',')
             WHERE TRY_CAST(value AS TINYINT) IS NOT NULL))
      AND (@ExcludeIds IS NULL OR LEN(@ExcludeIds) = 0 OR q.Id NOT IN
            (SELECT TRY_CAST(value AS BIGINT) FROM STRING_SPLIT(@ExcludeIds, ',')
             WHERE TRY_CAST(value AS BIGINT) IS NOT NULL))
      AND (@TagCount = 0 OR
            (SELECT COUNT(DISTINCT tg.Name)
             FROM dbo.QuestionTags AS qt
             INNER JOIN dbo.Tags AS tg ON tg.Id = qt.TagId
             WHERE qt.QuestionId = q.Id
               AND tg.Name IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT(@Tags, ','))) = @TagCount)
    ORDER BY CONVERT(BIGINT, SUBSTRING(HASHBYTES('SHA2_256',
                CONCAT(CAST(q.Id AS VARCHAR(20)), ':', CAST(@Seed AS VARCHAR(20)))), 1, 8)),
             q.Id;

    EXEC dbo.QuestionsProjectPicked;

    DROP TABLE #Picked;
END
GO

/*
    Proyecta el contenido de #Picked en dos conjuntos: preguntas y opciones.
    Se extrae a su propio procedimiento porque la extraccion, la busqueda y la lectura de un test
    necesitan exactamente la misma forma de salida.
*/
CREATE OR ALTER PROCEDURE dbo.QuestionsProjectPicked
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  q.Id,
            q.ExternalId,
            q.SyllabusTopicId AS TopicId,
            e.Code AS ExamCode,
            b.Code AS BlockCode,
            t.TopicNumber,
            t.Title AS TopicTitle,
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
    FROM #Picked AS p
    INNER JOIN dbo.Questions       AS q ON q.Id = p.Id
    INNER JOIN dbo.SyllabusTopics  AS t ON t.Id = q.SyllabusTopicId
    INNER JOIN dbo.SyllabusBlocks  AS b ON b.Id = t.BlockId
    INNER JOIN dbo.Exams           AS e ON e.Id = b.ExamId
    ORDER BY q.Id;

    SELECT  ao.QuestionId,
            ao.Id,
            ao.SortOrder,
            ao.OptionText,
            ao.IsCorrect
    FROM #Picked AS p
    INNER JOIN dbo.AnswerOptions AS ao ON ao.QuestionId = p.Id
    ORDER BY ao.QuestionId, ao.SortOrder;
END
GO

/* Cuenta cuantas preguntas cumplen los filtros, para avisar antes de generar un test corto. */
CREATE OR ALTER PROCEDURE dbo.QuestionsCountAvailable
    @ExamCode     NVARCHAR(20),
    @BlockCode    NVARCHAR(10)   = NULL,
    @TopicNumber  INT            = NULL,
    @TopicId      INT            = NULL,
    @Difficulties VARCHAR(50)    = NULL,
    @Tags         NVARCHAR(1000) = NULL,
    @ExcludeIds   VARCHAR(MAX)   = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TagCount INT =
        CASE
            WHEN @Tags IS NULL OR LEN(@Tags) = 0 THEN 0
            ELSE (SELECT COUNT(DISTINCT LTRIM(RTRIM(value)))
                  FROM STRING_SPLIT(@Tags, ',')
                  WHERE LTRIM(RTRIM(value)) <> N'')
        END;

    SELECT COUNT_BIG(1) AS Available
    FROM dbo.Questions AS q
    INNER JOIN dbo.SyllabusTopics AS t ON t.Id = q.SyllabusTopicId
    INNER JOIN dbo.SyllabusBlocks AS b ON b.Id = t.BlockId
    INNER JOIN dbo.Exams          AS e ON e.Id = b.ExamId
    WHERE q.IsActive = 1
      AND e.Code = @ExamCode
      AND (@BlockCode   IS NULL OR b.Code = @BlockCode)
      AND (@TopicNumber IS NULL OR t.TopicNumber = @TopicNumber)
      AND (@TopicId     IS NULL OR t.Id = @TopicId)
      AND (@Difficulties IS NULL OR LEN(@Difficulties) = 0 OR q.Difficulty IN
            (SELECT TRY_CAST(value AS TINYINT) FROM STRING_SPLIT(@Difficulties, ',')
             WHERE TRY_CAST(value AS TINYINT) IS NOT NULL))
      AND (@ExcludeIds IS NULL OR LEN(@ExcludeIds) = 0 OR q.Id NOT IN
            (SELECT TRY_CAST(value AS BIGINT) FROM STRING_SPLIT(@ExcludeIds, ',')
             WHERE TRY_CAST(value AS BIGINT) IS NOT NULL))
      AND (@TagCount = 0 OR
            (SELECT COUNT(DISTINCT tg.Name)
             FROM dbo.QuestionTags AS qt
             INNER JOIN dbo.Tags AS tg ON tg.Id = qt.TagId
             WHERE qt.QuestionId = q.Id
               AND tg.Name IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT(@Tags, ','))) = @TagCount);
END
GO

/* Busqueda paginada del banco. Devuelve preguntas, opciones y el total de coincidencias. */
CREATE OR ALTER PROCEDURE dbo.QuestionSearch
    @ExamCode     NVARCHAR(20)   = NULL,
    @BlockCode    NVARCHAR(10)   = NULL,
    @TopicNumber  INT            = NULL,
    @TopicId      INT            = NULL,
    @Difficulties VARCHAR(50)    = NULL,
    @Tags         NVARCHAR(1000) = NULL,
    @Search       NVARCHAR(400)  = NULL,
    @IsActive     BIT            = 1,
    @Offset       INT            = 0,
    @PageSize     INT            = 25
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TagCount INT =
        CASE
            WHEN @Tags IS NULL OR LEN(@Tags) = 0 THEN 0
            ELSE (SELECT COUNT(DISTINCT LTRIM(RTRIM(value)))
                  FROM STRING_SPLIT(@Tags, ',')
                  WHERE LTRIM(RTRIM(value)) <> N'')
        END;

    DECLARE @Pattern NVARCHAR(410) =
        CASE WHEN @Search IS NULL OR LEN(@Search) = 0 THEN NULL ELSE N'%' + @Search + N'%' END;

    CREATE TABLE #Matches (Id BIGINT NOT NULL PRIMARY KEY);

    INSERT INTO #Matches (Id)
    SELECT q.Id
    FROM dbo.Questions AS q
    INNER JOIN dbo.SyllabusTopics AS t ON t.Id = q.SyllabusTopicId
    INNER JOIN dbo.SyllabusBlocks AS b ON b.Id = t.BlockId
    INNER JOIN dbo.Exams          AS e ON e.Id = b.ExamId
    WHERE (@IsActive     IS NULL OR q.IsActive = @IsActive)
      AND (@ExamCode     IS NULL OR e.Code = @ExamCode)
      AND (@BlockCode    IS NULL OR b.Code = @BlockCode)
      AND (@TopicNumber  IS NULL OR t.TopicNumber = @TopicNumber)
      AND (@TopicId      IS NULL OR t.Id = @TopicId)
      AND (@Difficulties IS NULL OR LEN(@Difficulties) = 0 OR q.Difficulty IN
            (SELECT TRY_CAST(value AS TINYINT) FROM STRING_SPLIT(@Difficulties, ',')
             WHERE TRY_CAST(value AS TINYINT) IS NOT NULL))
      AND (@Pattern IS NULL
           OR q.Statement LIKE @Pattern
           OR q.Explanation LIKE @Pattern
           OR q.SourceReference LIKE @Pattern
           OR EXISTS (SELECT 1 FROM dbo.AnswerOptions AS ao
                      WHERE ao.QuestionId = q.Id AND ao.OptionText LIKE @Pattern))
      AND (@TagCount = 0 OR
            (SELECT COUNT(DISTINCT tg.Name)
             FROM dbo.QuestionTags AS qt
             INNER JOIN dbo.Tags AS tg ON tg.Id = qt.TagId
             WHERE qt.QuestionId = q.Id
               AND tg.Name IN (SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT(@Tags, ','))) = @TagCount);

    DECLARE @Total BIGINT = (SELECT COUNT_BIG(1) FROM #Matches);

    CREATE TABLE #Picked (Id BIGINT NOT NULL PRIMARY KEY);

    INSERT INTO #Picked (Id)
    SELECT Id
    FROM #Matches
    ORDER BY Id
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    EXEC dbo.QuestionsProjectPicked;

    SELECT @Total AS TotalItems;

    DROP TABLE #Picked;
    DROP TABLE #Matches;
END
GO

/* Lectura de una pregunta concreta con sus opciones. */
CREATE OR ALTER PROCEDURE dbo.QuestionGet
    @QuestionId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #Picked (Id BIGINT NOT NULL PRIMARY KEY);

    INSERT INTO #Picked (Id)
    SELECT Id FROM dbo.Questions WHERE Id = @QuestionId;

    EXEC dbo.QuestionsProjectPicked;

    DROP TABLE #Picked;
END
GO

/*
    Cobertura del banco por tema.
    Recorre el temario y no las preguntas: los temas todavia vacios tienen que aparecer con cero,
    porque son precisamente los que hay que rellenar.
*/
CREATE OR ALTER PROCEDURE dbo.QuestionBankCoverage
    @ExamCode NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  t.Id AS TopicId,
            b.Code AS BlockCode,
            b.Name AS BlockName,
            t.TopicNumber,
            t.Title AS TopicTitle,
            COUNT(q.Id) AS QuestionCount,
            SUM(CASE WHEN q.IsActive = 1 THEN 1 ELSE 0 END) AS ActiveQuestionCount,
            CAST(ISNULL(AVG(CASE WHEN q.IsActive = 1 THEN CAST(q.Difficulty AS DECIMAL(9,2)) END), 0)
                 AS DECIMAL(9,2)) AS AverageDifficulty
    FROM dbo.SyllabusTopics AS t
    INNER JOIN dbo.SyllabusBlocks AS b ON b.Id = t.BlockId
    INNER JOIN dbo.Exams          AS e ON e.Id = b.ExamId
    LEFT  JOIN dbo.Questions      AS q ON q.SyllabusTopicId = t.Id
    WHERE e.Code = @ExamCode
    GROUP BY t.Id, b.Code, b.Name, b.DisplayOrder, t.TopicNumber, t.Title
    ORDER BY b.DisplayOrder, t.TopicNumber;
END
GO
