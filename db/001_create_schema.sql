CREATE TABLE dbo.SyllabusBlocks
(
    Id   INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Code NVARCHAR(10) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    CONSTRAINT UQ_SyllabusBlocks_Code UNIQUE (Code)
);

CREATE TABLE dbo.SyllabusTopics
(
    Id         INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    BlockId    INT NOT NULL,
    TopicNumber INT NOT NULL,
    Title      NVARCHAR(400) NOT NULL,
    CONSTRAINT FK_SyllabusTopics_Block FOREIGN KEY (BlockId) REFERENCES dbo.SyllabusBlocks(Id),
    CONSTRAINT UQ_SyllabusTopics_Block_Topic UNIQUE (BlockId, TopicNumber)
);

CREATE TABLE dbo.Questions
(
    Id              BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    SyllabusTopicId INT NOT NULL,
    Difficulty      TINYINT NOT NULL,
    Statement       NVARCHAR(2000) NOT NULL,
    Explanation     NVARCHAR(4000) NULL,
    IsActive        BIT NOT NULL CONSTRAINT DF_Questions_IsActive DEFAULT(1),
    RandomKey       INT NOT NULL CONSTRAINT DF_Questions_RandomKey DEFAULT (ABS(CHECKSUM(NEWID()))),
    CreatedAt       DATETIME2(3) NOT NULL CONSTRAINT DF_Questions_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_Questions_SyllabusTopics FOREIGN KEY (SyllabusTopicId) REFERENCES dbo.SyllabusTopics(Id),
    CONSTRAINT CK_Questions_Difficulty CHECK (Difficulty BETWEEN 1 AND 5)
);

CREATE INDEX IX_Questions_SelectFast
ON dbo.Questions (SyllabusTopicId, Difficulty, IsActive, RandomKey)
INCLUDE (Id);

CREATE TABLE dbo.AnswerOptions
(
    Id         BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    QuestionId BIGINT NOT NULL,
    SortOrder  TINYINT NOT NULL,
    OptionText NVARCHAR(1000) NOT NULL,
    IsCorrect  BIT NOT NULL,
    CONSTRAINT FK_AnswerOptions_Questions FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_AnswerOptions_Q_Sort UNIQUE (QuestionId, SortOrder)
);

CREATE INDEX IX_AnswerOptions_QuestionId ON dbo.AnswerOptions(QuestionId);

CREATE TABLE dbo.Tests
(
    Id            BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Title         NVARCHAR(200) NOT NULL,
    TotalQuestions INT NOT NULL,
    Seed          INT NOT NULL,
    CreatedAt     DATETIME2(3) NOT NULL CONSTRAINT DF_Tests_CreatedAt DEFAULT (SYSUTCDATETIME())
);

CREATE TABLE dbo.TestQuestions
(
    TestId     BIGINT NOT NULL,
    QuestionId BIGINT NOT NULL,
    SortOrder  INT NOT NULL,
    CONSTRAINT PK_TestQuestions PRIMARY KEY (TestId, QuestionId),
    CONSTRAINT FK_TestQuestions_Tests FOREIGN KEY (TestId) REFERENCES dbo.Tests(Id) ON DELETE CASCADE,
    CONSTRAINT FK_TestQuestions_Questions FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(Id),
    CONSTRAINT UQ_TestQuestions_Test_Sort UNIQUE (TestId, SortOrder)
);

CREATE INDEX IX_TestQuestions_TestId ON dbo.TestQuestions(TestId) INCLUDE (QuestionId, SortOrder);

CREATE TABLE dbo.Attempts
(
    Id        BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TestId    BIGINT NOT NULL,
    UserName  NVARCHAR(120) NOT NULL,
    StartedAt DATETIME2(3) NOT NULL CONSTRAINT DF_Attempts_StartedAt DEFAULT (SYSUTCDATETIME()),
    FinishedAt DATETIME2(3) NULL,
    Score     DECIMAL(5,2) NULL,
    CONSTRAINT FK_Attempts_Tests FOREIGN KEY (TestId) REFERENCES dbo.Tests(Id) ON DELETE CASCADE
);

CREATE TABLE dbo.AttemptAnswers
(
    AttemptId      BIGINT NOT NULL,
    QuestionId     BIGINT NOT NULL,
    AnswerOptionId BIGINT NULL,
    AnsweredAt     DATETIME2(3) NOT NULL CONSTRAINT DF_AttemptAnswers_AnsweredAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT PK_AttemptAnswers PRIMARY KEY (AttemptId, QuestionId),
    CONSTRAINT FK_AttemptAnswers_Attempts FOREIGN KEY (AttemptId) REFERENCES dbo.Attempts(Id) ON DELETE CASCADE,
    CONSTRAINT FK_AttemptAnswers_Questions FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(Id),
    CONSTRAINT FK_AttemptAnswers_AnswerOptions FOREIGN KEY (AnswerOptionId) REFERENCES dbo.AnswerOptions(Id)
);