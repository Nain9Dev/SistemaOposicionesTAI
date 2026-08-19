/*
    002_types.sql
    Tipos de tabla definidos por el usuario.

    Se usan para enviar conjuntos completos en una sola llamada (las preguntas de un test, las
    opciones de una pregunta) en lugar de recorrer la red una vez por fila.
*/

SET NOCOUNT ON;
GO

IF TYPE_ID(N'dbo.TestQuestionList') IS NULL
BEGIN
    CREATE TYPE dbo.TestQuestionList AS TABLE
    (
        QuestionId BIGINT NOT NULL,
        SortOrder  INT    NOT NULL,
        PRIMARY KEY (QuestionId)
    );
END
GO

IF TYPE_ID(N'dbo.TestQuestionOptionList') IS NULL
BEGIN
    CREATE TYPE dbo.TestQuestionOptionList AS TABLE
    (
        QuestionId     BIGINT  NOT NULL,
        AnswerOptionId BIGINT  NOT NULL,
        SortOrder      TINYINT NOT NULL,
        PRIMARY KEY (QuestionId, AnswerOptionId)
    );
END
GO

IF TYPE_ID(N'dbo.AnswerOptionList') IS NULL
BEGIN
    CREATE TYPE dbo.AnswerOptionList AS TABLE
    (
        SortOrder  TINYINT        NOT NULL,
        OptionText NVARCHAR(1000) NOT NULL,
        IsCorrect  BIT            NOT NULL,
        PRIMARY KEY (SortOrder)
    );
END
GO

IF TYPE_ID(N'dbo.TagList') IS NULL
BEGIN
    CREATE TYPE dbo.TagList AS TABLE
    (
        Name NVARCHAR(80) NOT NULL,
        PRIMARY KEY (Name)
    );
END
GO
