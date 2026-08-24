CREATE TABLE Usuarios (
    Id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Rol VARCHAR(50) NOT NULL,
    FechaRegistro TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE SyllabusBlocks
(
    Id   INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    Code VARCHAR(10) NOT NULL,
    Name VARCHAR(200) NOT NULL,
    CONSTRAINT UQ_SyllabusBlocks_Code UNIQUE (Code)
);

CREATE TABLE SyllabusTopics
(
    Id         INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    BlockId    INT NOT NULL,
    TopicNumber INT NOT NULL,
    Title      VARCHAR(400) NOT NULL,
    CONSTRAINT FK_SyllabusTopics_Block FOREIGN KEY (BlockId) REFERENCES SyllabusBlocks(Id),
    CONSTRAINT UQ_SyllabusTopics_Block_Topic UNIQUE (BlockId, TopicNumber)
);

CREATE TABLE Questions
(
    Id              BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    SyllabusTopicId INT NOT NULL,
    Difficulty      SMALLINT NOT NULL,
    Statement       VARCHAR(2000) NOT NULL,
    Explanation     VARCHAR(4000) NULL,
    IsActive        BOOLEAN NOT NULL DEFAULT TRUE,
    RandomKey       INT NOT NULL DEFAULT (random() * 2147483647)::INT,
    CreatedAt       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_Questions_SyllabusTopics FOREIGN KEY (SyllabusTopicId) REFERENCES SyllabusTopics(Id),
    CONSTRAINT CK_Questions_Difficulty CHECK (Difficulty BETWEEN 1 AND 5)
);

CREATE INDEX IX_Questions_SelectFast
ON Questions (SyllabusTopicId, Difficulty, IsActive, RandomKey)
INCLUDE (Id);

CREATE TABLE AnswerOptions
(
    Id         BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    QuestionId BIGINT NOT NULL,
    SortOrder  SMALLINT NOT NULL,
    OptionText VARCHAR(1000) NOT NULL,
    IsCorrect  BOOLEAN NOT NULL,
    CONSTRAINT FK_AnswerOptions_Questions FOREIGN KEY (QuestionId) REFERENCES Questions(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_AnswerOptions_Q_Sort UNIQUE (QuestionId, SortOrder)
);

CREATE INDEX IX_AnswerOptions_QuestionId ON AnswerOptions(QuestionId);

CREATE TABLE Tests
(
    Id            BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    Title         VARCHAR(200) NOT NULL,
    TotalQuestions INT NOT NULL,
    Seed          INT NOT NULL,
    CreatedAt     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE TestQuestions
(
    TestId     BIGINT NOT NULL,
    QuestionId BIGINT NOT NULL,
    SortOrder  INT NOT NULL,
    CONSTRAINT PK_TestQuestions PRIMARY KEY (TestId, QuestionId),
    CONSTRAINT FK_TestQuestions_Tests FOREIGN KEY (TestId) REFERENCES Tests(Id) ON DELETE CASCADE,
    CONSTRAINT FK_TestQuestions_Questions FOREIGN KEY (QuestionId) REFERENCES Questions(Id),
    CONSTRAINT UQ_TestQuestions_Test_Sort UNIQUE (TestId, SortOrder)
);

CREATE INDEX IX_TestQuestions_TestId ON TestQuestions(TestId) INCLUDE (QuestionId, SortOrder);

CREATE TABLE Attempts
(
    Id        BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    TestId    BIGINT NOT NULL,
    UserName  VARCHAR(120) NOT NULL,
    StartedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FinishedAt TIMESTAMP NULL,
    Score     DECIMAL(5,2) NULL,
    CONSTRAINT FK_Attempts_Tests FOREIGN KEY (TestId) REFERENCES Tests(Id) ON DELETE CASCADE
);

CREATE TABLE AttemptAnswers
(
    AttemptId      BIGINT NOT NULL,
    QuestionId     BIGINT NOT NULL,
    AnswerOptionId BIGINT NULL,
    AnsweredAt     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT PK_AttemptAnswers PRIMARY KEY (AttemptId, QuestionId),
    CONSTRAINT FK_AttemptAnswers_Attempts FOREIGN KEY (AttemptId) REFERENCES Attempts(Id) ON DELETE CASCADE,
    CONSTRAINT FK_AttemptAnswers_Questions FOREIGN KEY (QuestionId) REFERENCES Questions(Id),
    CONSTRAINT FK_AttemptAnswers_AnswerOptions FOREIGN KEY (AnswerOptionId) REFERENCES AnswerOptions(Id)
);