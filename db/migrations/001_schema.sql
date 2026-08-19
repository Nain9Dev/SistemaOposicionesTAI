/*
    001_schema.sql
    Esquema relacional del Sistema de Oposiciones.

    Es idempotente: puede ejecutarse tantas veces como haga falta sobre la misma base de datos.
    El diseno es multi-convocatoria desde la raiz (dbo.Exams), de modo que anadir una oposicion
    nueva es insertar filas, nunca modificar tablas.
*/

SET NOCOUNT ON;
GO

/* ---------------------------------------------------------------------------------------------
   Convocatorias
   El baremo y el formato del ejercicio se guardan como datos de la convocatoria: si unas bases
   cambian la penalizacion o el numero de preguntas, es un UPDATE y no un despliegue.
--------------------------------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Exams', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Exams
    (
        Id                INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Exams PRIMARY KEY,
        Code              NVARCHAR(20)   NOT NULL,
        Name              NVARCHAR(300)  NOT NULL,
        Authority         NVARCHAR(200)  NOT NULL CONSTRAINT DF_Exams_Authority DEFAULT (N''),
        Description       NVARCHAR(1000) NOT NULL CONSTRAINT DF_Exams_Description DEFAULT (N''),
        SourceReference   NVARCHAR(400)  NULL,
        SourcePublication NVARCHAR(200)  NULL,
        SourceUrl         NVARCHAR(500)  NULL,
        CorrectPoints     DECIMAL(9,4)   NOT NULL CONSTRAINT DF_Exams_CorrectPoints DEFAULT (1.0),
        IncorrectPoints   DECIMAL(9,4)   NOT NULL CONSTRAINT DF_Exams_IncorrectPoints DEFAULT (-0.3333),
        BlankPoints       DECIMAL(9,4)   NOT NULL CONSTRAINT DF_Exams_BlankPoints DEFAULT (0.0),
        MaxScore          DECIMAL(9,2)   NOT NULL CONSTRAINT DF_Exams_MaxScore DEFAULT (50.0),
        PassMark          DECIMAL(9,2)   NOT NULL CONSTRAINT DF_Exams_PassMark DEFAULT (25.0),
        QuestionCount     INT            NOT NULL CONSTRAINT DF_Exams_QuestionCount DEFAULT (80),
        ReserveQuestions  INT            NOT NULL CONSTRAINT DF_Exams_ReserveQuestions DEFAULT (5),
        DurationMinutes   INT            NOT NULL CONSTRAINT DF_Exams_DurationMinutes DEFAULT (120),
        OptionsPerQuestion INT           NOT NULL CONSTRAINT DF_Exams_OptionsPerQuestion DEFAULT (4),
        IsActive          BIT            NOT NULL CONSTRAINT DF_Exams_IsActive DEFAULT (1),
        CreatedAt         DATETIME2(3)   NOT NULL CONSTRAINT DF_Exams_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt         DATETIME2(3)   NOT NULL CONSTRAINT DF_Exams_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT UQ_Exams_Code UNIQUE (Code)
    );
END
GO

/* ---------------------------------------------------------------------------------------------
   Temario: bloques y temas
--------------------------------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.SyllabusBlocks', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyllabusBlocks
    (
        Id                INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyllabusBlocks PRIMARY KEY,
        ExamId            INT           NOT NULL,
        Code              NVARCHAR(10)  NOT NULL,
        Name              NVARCHAR(300) NOT NULL,
        DisplayOrder      INT           NOT NULL CONSTRAINT DF_SyllabusBlocks_DisplayOrder DEFAULT (0),
        ExamWeightPercent DECIMAL(5,2)  NOT NULL CONSTRAINT DF_SyllabusBlocks_Weight DEFAULT (0),
        CONSTRAINT FK_SyllabusBlocks_Exams FOREIGN KEY (ExamId) REFERENCES dbo.Exams(Id) ON DELETE CASCADE,
        CONSTRAINT UQ_SyllabusBlocks_Exam_Code UNIQUE (ExamId, Code)
    );
END
GO

IF OBJECT_ID(N'dbo.SyllabusTopics', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SyllabusTopics
    (
        Id          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SyllabusTopics PRIMARY KEY,
        BlockId     INT            NOT NULL,
        TopicNumber INT            NOT NULL,
        Title       NVARCHAR(600)  NOT NULL,
        Slug        NVARCHAR(200)  NOT NULL CONSTRAINT DF_SyllabusTopics_Slug DEFAULT (N''),
        Keywords    NVARCHAR(1000) NOT NULL CONSTRAINT DF_SyllabusTopics_Keywords DEFAULT (N''),
        CONSTRAINT FK_SyllabusTopics_Block FOREIGN KEY (BlockId) REFERENCES dbo.SyllabusBlocks(Id) ON DELETE CASCADE,
        CONSTRAINT UQ_SyllabusTopics_Block_Topic UNIQUE (BlockId, TopicNumber)
    );
END
GO

/* ---------------------------------------------------------------------------------------------
   Banco de preguntas

   ExternalId es la clave de negocio que viaja en los ficheros JSON de contenido: permite
   reimportar el banco cuantas veces se quiera sin duplicar preguntas ni romper referencias.

   RandomKey se conserva por compatibilidad y como criterio de desempate; la extraccion
   reproducible por semilla se resuelve en dbo.QuestionsDraw.
--------------------------------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Questions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Questions
    (
        Id                BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Questions PRIMARY KEY,
        ExternalId        NVARCHAR(80)   NOT NULL,
        SyllabusTopicId   INT            NOT NULL,
        Difficulty        TINYINT        NOT NULL,
        Statement         NVARCHAR(2000) NOT NULL,
        Explanation       NVARCHAR(4000) NULL,
        SourceReference   NVARCHAR(400)  NULL,
        SourcePublication NVARCHAR(200)  NULL,
        SourceUrl         NVARCHAR(500)  NULL,
        IsActive          BIT            NOT NULL CONSTRAINT DF_Questions_IsActive DEFAULT (1),
        RandomKey         INT            NOT NULL CONSTRAINT DF_Questions_RandomKey DEFAULT (ABS(CHECKSUM(NEWID()))),
        CreatedAt         DATETIME2(3)   NOT NULL CONSTRAINT DF_Questions_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt         DATETIME2(3)   NOT NULL CONSTRAINT DF_Questions_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT FK_Questions_SyllabusTopics FOREIGN KEY (SyllabusTopicId) REFERENCES dbo.SyllabusTopics(Id),
        CONSTRAINT CK_Questions_Difficulty CHECK (Difficulty BETWEEN 1 AND 5),
        CONSTRAINT UQ_Questions_ExternalId UNIQUE (ExternalId)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Questions_SelectFast' AND object_id = OBJECT_ID(N'dbo.Questions'))
BEGIN
    -- La clave del indice agrupado (Id) no se lista: SQL Server ya la arrastra en todo indice
    -- no agrupado, y repetirla solo anade ruido.
    CREATE INDEX IX_Questions_SelectFast
        ON dbo.Questions (SyllabusTopicId, IsActive, Difficulty)
        INCLUDE (RandomKey);
END
GO

IF OBJECT_ID(N'dbo.AnswerOptions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AnswerOptions
    (
        Id         BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AnswerOptions PRIMARY KEY,
        QuestionId BIGINT         NOT NULL,
        SortOrder  TINYINT        NOT NULL,
        OptionText NVARCHAR(1000) NOT NULL,
        IsCorrect  BIT            NOT NULL,
        CONSTRAINT FK_AnswerOptions_Questions FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(Id) ON DELETE CASCADE,
        CONSTRAINT UQ_AnswerOptions_Q_Sort UNIQUE (QuestionId, SortOrder)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AnswerOptions_QuestionId' AND object_id = OBJECT_ID(N'dbo.AnswerOptions'))
BEGIN
    CREATE INDEX IX_AnswerOptions_QuestionId ON dbo.AnswerOptions (QuestionId) INCLUDE (SortOrder, IsCorrect);
END
GO

/* ---------------------------------------------------------------------------------------------
   Etiquetas tematicas
   Se normalizan en su propia tabla en lugar de guardarse como texto separado por comas: es lo
   que permite filtrar por etiqueta con un indice en vez de con un LIKE sobre toda la tabla.
--------------------------------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Tags', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tags
    (
        Id   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Tags PRIMARY KEY,
        Name NVARCHAR(80) NOT NULL,
        CONSTRAINT UQ_Tags_Name UNIQUE (Name)
    );
END
GO

IF OBJECT_ID(N'dbo.QuestionTags', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.QuestionTags
    (
        QuestionId BIGINT NOT NULL,
        TagId      INT    NOT NULL,
        CONSTRAINT PK_QuestionTags PRIMARY KEY (QuestionId, TagId),
        CONSTRAINT FK_QuestionTags_Questions FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(Id) ON DELETE CASCADE,
        CONSTRAINT FK_QuestionTags_Tags FOREIGN KEY (TagId) REFERENCES dbo.Tags(Id) ON DELETE CASCADE
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_QuestionTags_TagId' AND object_id = OBJECT_ID(N'dbo.QuestionTags'))
BEGIN
    CREATE INDEX IX_QuestionTags_TagId ON dbo.QuestionTags (TagId) INCLUDE (QuestionId);
END
GO

/* ---------------------------------------------------------------------------------------------
   Tests generados
   El baremo se copia sobre el test al crearlo. Es deliberado: un examen ya realizado debe poder
   corregirse siempre con las reglas vigentes cuando se genero, aunque la convocatoria cambie.
--------------------------------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Tests', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tests
    (
        Id              BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Tests PRIMARY KEY,
        ExamId          INT           NOT NULL,
        Title           NVARCHAR(300) NOT NULL,
        Mode            TINYINT       NOT NULL CONSTRAINT DF_Tests_Mode DEFAULT (0),
        TotalQuestions  INT           NOT NULL,
        Seed            INT           NOT NULL,
        DurationMinutes INT           NOT NULL CONSTRAINT DF_Tests_DurationMinutes DEFAULT (0),
        CorrectPoints   DECIMAL(9,4)  NOT NULL CONSTRAINT DF_Tests_CorrectPoints DEFAULT (1.0),
        IncorrectPoints DECIMAL(9,4)  NOT NULL CONSTRAINT DF_Tests_IncorrectPoints DEFAULT (-0.3333),
        BlankPoints     DECIMAL(9,4)  NOT NULL CONSTRAINT DF_Tests_BlankPoints DEFAULT (0.0),
        MaxScore        DECIMAL(9,2)  NOT NULL CONSTRAINT DF_Tests_MaxScore DEFAULT (50.0),
        PassMark        DECIMAL(9,2)  NOT NULL CONSTRAINT DF_Tests_PassMark DEFAULT (25.0),
        BlueprintJson   NVARCHAR(MAX) NULL,
        CreatedAt       DATETIME2(3)  NOT NULL CONSTRAINT DF_Tests_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT FK_Tests_Exams FOREIGN KEY (ExamId) REFERENCES dbo.Exams(Id),
        CONSTRAINT CK_Tests_Mode CHECK (Mode IN (0, 1))
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Tests_Exam_CreatedAt' AND object_id = OBJECT_ID(N'dbo.Tests'))
BEGIN
    CREATE INDEX IX_Tests_Exam_CreatedAt ON dbo.Tests (ExamId, CreatedAt DESC) INCLUDE (Title, Mode);
END
GO

IF OBJECT_ID(N'dbo.TestQuestions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TestQuestions
    (
        TestId     BIGINT NOT NULL,
        QuestionId BIGINT NOT NULL,
        SortOrder  INT    NOT NULL,
        CONSTRAINT PK_TestQuestions PRIMARY KEY (TestId, QuestionId),
        CONSTRAINT FK_TestQuestions_Tests FOREIGN KEY (TestId) REFERENCES dbo.Tests(Id) ON DELETE CASCADE,
        CONSTRAINT FK_TestQuestions_Questions FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(Id),
        CONSTRAINT UQ_TestQuestions_Test_Sort UNIQUE (TestId, SortOrder)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TestQuestions_TestId' AND object_id = OBJECT_ID(N'dbo.TestQuestions'))
BEGIN
    CREATE INDEX IX_TestQuestions_TestId ON dbo.TestQuestions (TestId) INCLUDE (QuestionId, SortOrder);
END
GO

/*
   Orden de opciones especifico del test. Barajar las opciones y guardar el orden permite que la
   revision posterior muestre exactamente la misma pantalla que vio el opositor.
*/
IF OBJECT_ID(N'dbo.TestQuestionOptions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TestQuestionOptions
    (
        TestId         BIGINT  NOT NULL,
        QuestionId     BIGINT  NOT NULL,
        AnswerOptionId BIGINT  NOT NULL,
        SortOrder      TINYINT NOT NULL,
        CONSTRAINT PK_TestQuestionOptions PRIMARY KEY (TestId, QuestionId, AnswerOptionId),
        CONSTRAINT FK_TestQuestionOptions_Tests FOREIGN KEY (TestId) REFERENCES dbo.Tests(Id) ON DELETE CASCADE,
        CONSTRAINT FK_TestQuestionOptions_Options FOREIGN KEY (AnswerOptionId) REFERENCES dbo.AnswerOptions(Id)
    );
END
GO

/* ---------------------------------------------------------------------------------------------
   Intentos
   Se guarda el desglose completo de la nota, no solo la cifra final, para poder auditarla.
--------------------------------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Attempts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Attempts
    (
        Id              BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Attempts PRIMARY KEY,
        TestId          BIGINT        NOT NULL,
        UserName        NVARCHAR(120) NOT NULL,
        StartedAt       DATETIME2(3)  NOT NULL CONSTRAINT DF_Attempts_StartedAt DEFAULT (SYSUTCDATETIME()),
        FinishedAt      DATETIME2(3)  NULL,
        TotalQuestions  INT           NULL,
        CorrectCount    INT           NULL,
        IncorrectCount  INT           NULL,
        BlankCount      INT           NULL,
        RawScore        DECIMAL(9,3)  NULL,
        ScaledScore     DECIMAL(9,3)  NULL,
        MaxScore        DECIMAL(9,2)  NULL,
        PassMark        DECIMAL(9,2)  NULL,
        AccuracyPercent DECIMAL(6,2)  NULL,
        Passed          BIT           NULL,
        CONSTRAINT FK_Attempts_Tests FOREIGN KEY (TestId) REFERENCES dbo.Tests(Id) ON DELETE CASCADE
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Attempts_User_StartedAt' AND object_id = OBJECT_ID(N'dbo.Attempts'))
BEGIN
    CREATE INDEX IX_Attempts_User_StartedAt
        ON dbo.Attempts (UserName, StartedAt DESC)
        INCLUDE (TestId, FinishedAt, ScaledScore, AccuracyPercent);
END
GO

IF OBJECT_ID(N'dbo.AttemptAnswers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AttemptAnswers
    (
        AttemptId      BIGINT       NOT NULL,
        QuestionId     BIGINT       NOT NULL,
        AnswerOptionId BIGINT       NULL,
        AnsweredAt     DATETIME2(3) NOT NULL CONSTRAINT DF_AttemptAnswers_AnsweredAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_AttemptAnswers PRIMARY KEY (AttemptId, QuestionId),
        CONSTRAINT FK_AttemptAnswers_Attempts FOREIGN KEY (AttemptId) REFERENCES dbo.Attempts(Id) ON DELETE CASCADE,
        CONSTRAINT FK_AttemptAnswers_Questions FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(Id),
        CONSTRAINT FK_AttemptAnswers_AnswerOptions FOREIGN KEY (AnswerOptionId) REFERENCES dbo.AnswerOptions(Id)
    );
END
GO
