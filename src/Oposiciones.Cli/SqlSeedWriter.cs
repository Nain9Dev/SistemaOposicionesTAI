using System.Globalization;
using System.Text;
using Oposiciones.Domain.Catalog;
using Oposiciones.Infrastructure.Content;

namespace Oposiciones.Cli;

/// <summary>
/// Genera el guion T-SQL que carga el contenido en SQL Server.
/// <para>
/// El guion solo invoca los procedimientos de importacion, que trabajan por clave de negocio
/// (codigo de convocatoria, numero de tema, identificador externo de pregunta). Por eso puede
/// ejecutarse tantas veces como se quiera: la segunda ejecucion actualiza lo que cambio y deja
/// intacto lo demas, sin duplicar preguntas ni romper los intentos ya registrados.
/// </para>
/// </summary>
public static class SqlSeedWriter
{
    public static string Build(ContentCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        var sql = new StringBuilder();

        sql.AppendLine("/*");
        sql.AppendLine("    Carga de contenido del Sistema de Oposiciones.");
        sql.AppendLine();
        sql.AppendLine("    GENERADO AUTOMATICAMENTE a partir de la carpeta content/ mediante:");
        sql.AppendLine("        dotnet run --project src/Oposiciones.Cli -- sql --out db/seed/content.sql");
        sql.AppendLine();
        sql.AppendLine("    No editar a mano: cualquier cambio debe hacerse en los ficheros JSON de contenido");
        sql.AppendLine("    y regenerarse este guion. Requiere haber ejecutado antes db/migrations/.");
        sql.AppendLine("*/");
        sql.AppendLine();
        sql.AppendLine("SET NOCOUNT ON;");
        sql.AppendLine("SET XACT_ABORT ON;");
        sql.AppendLine("GO");
        sql.AppendLine();
        sql.AppendLine("DECLARE @Options dbo.AnswerOptionList;");
        sql.AppendLine("DECLARE @Tags dbo.TagList;");
        sql.AppendLine();

        foreach (ExamProfile exam in catalog.Exams)
        {
            AppendExam(sql, exam);
        }

        foreach (ExamProfile exam in catalog.Exams)
        {
            IEnumerable<Question> questions = catalog.Questions
                .Where(question => string.Equals(question.ExamCode, exam.Code, StringComparison.OrdinalIgnoreCase))
                .OrderBy(question => question.ExternalId, StringComparer.Ordinal);

            sql.AppendLine();
            sql.AppendLine($"-- ---------- Banco de preguntas de {exam.Code} ----------");

            foreach (Question question in questions)
            {
                AppendQuestion(sql, question);
            }
        }

        sql.AppendLine();
        sql.AppendLine("GO");
        sql.AppendLine();
        sql.AppendLine("PRINT 'Carga de contenido completada.';");
        sql.AppendLine("GO");

        return sql.ToString();
    }

    private static void AppendExam(StringBuilder sql, ExamProfile exam)
    {
        sql.AppendLine($"-- ---------- Convocatoria {exam.Code} ----------");
        sql.AppendLine("EXEC dbo.ExamUpsert");
        sql.AppendLine($"    @Code = {Text(exam.Code)},");
        sql.AppendLine($"    @Name = {Text(exam.Name)},");
        sql.AppendLine($"    @Authority = {Text(exam.Authority)},");
        sql.AppendLine($"    @Description = {Text(exam.Description)},");
        sql.AppendLine($"    @SourceReference = {Text(exam.OfficialSource?.Reference)},");
        sql.AppendLine($"    @SourcePublication = {Text(exam.OfficialSource?.Publication)},");
        sql.AppendLine($"    @SourceUrl = {Text(exam.OfficialSource?.Url)},");
        sql.AppendLine($"    @CorrectPoints = {Number(exam.Scoring.CorrectPoints)},");
        sql.AppendLine($"    @IncorrectPoints = {Number(exam.Scoring.IncorrectPoints)},");
        sql.AppendLine($"    @BlankPoints = {Number(exam.Scoring.BlankPoints)},");
        sql.AppendLine($"    @MaxScore = {Number(exam.Scoring.ScaleMaxScore)},");
        sql.AppendLine($"    @PassMark = {Number(exam.Scoring.PassMark)},");
        sql.AppendLine($"    @QuestionCount = {exam.Format.QuestionCount},");
        sql.AppendLine($"    @ReserveQuestions = {exam.Format.ReserveQuestions},");
        sql.AppendLine($"    @DurationMinutes = {exam.Format.DurationMinutes},");
        sql.AppendLine($"    @OptionsPerQuestion = {exam.Format.OptionsPerQuestion};");
        sql.AppendLine();

        foreach (SyllabusBlock block in exam.Blocks)
        {
            sql.AppendLine($"EXEC dbo.SyllabusBlockUpsert @ExamCode = {Text(exam.Code)}, "
                + $"@Code = {Text(block.Code)}, @Name = {Text(block.Name)}, "
                + $"@DisplayOrder = {block.DisplayOrder}, @ExamWeightPercent = {Number(block.ExamWeightPercent)};");

            foreach (SyllabusTopic topic in block.Topics)
            {
                sql.AppendLine($"EXEC dbo.SyllabusTopicUpsert @ExamCode = {Text(exam.Code)}, "
                    + $"@BlockCode = {Text(block.Code)}, @TopicNumber = {topic.Number}, "
                    + $"@Title = {Text(topic.Title)}, @Slug = {Text(topic.Slug)}, "
                    + $"@Keywords = {Text(string.Join(',', topic.Keywords))};");
            }

            sql.AppendLine();
        }
    }

    private static void AppendQuestion(StringBuilder sql, Question question)
    {
        sql.AppendLine();
        sql.AppendLine($"-- {question.ExternalId} | Bloque {question.BlockCode} | Tema {question.TopicNumber}");
        sql.AppendLine("DELETE @Options; DELETE @Tags;");

        foreach (AnswerOption option in question.Options.OrderBy(option => option.SortOrder))
        {
            sql.AppendLine("INSERT INTO @Options (SortOrder, OptionText, IsCorrect) VALUES "
                + $"({option.SortOrder}, {Text(option.Text)}, {(option.IsCorrect ? 1 : 0)});");
        }

        foreach (string tag in question.Tags)
        {
            sql.AppendLine($"INSERT INTO @Tags (Name) VALUES ({Text(tag)});");
        }

        sql.AppendLine("EXEC dbo.QuestionUpsert");
        sql.AppendLine($"    @ExternalId = {Text(question.ExternalId)},");
        sql.AppendLine($"    @ExamCode = {Text(question.ExamCode)},");
        sql.AppendLine($"    @BlockCode = {Text(question.BlockCode)},");
        sql.AppendLine($"    @TopicNumber = {question.TopicNumber},");
        sql.AppendLine($"    @Difficulty = {(int)question.Difficulty},");
        sql.AppendLine($"    @Statement = {Text(question.Statement)},");
        sql.AppendLine($"    @Explanation = {Text(question.Explanation)},");
        sql.AppendLine($"    @SourceReference = {Text(question.Source?.Reference)},");
        sql.AppendLine($"    @SourcePublication = {Text(question.Source?.Publication)},");
        sql.AppendLine($"    @SourceUrl = {Text(question.Source?.Url)},");
        sql.AppendLine($"    @IsActive = {(question.IsActive ? 1 : 0)},");
        sql.AppendLine("    @Options = @Options,");
        sql.AppendLine("    @Tags = @Tags;");
    }

    /// <summary>
    /// Emite un literal Unicode escapando las comillas simples. Todo el contenido pasa por aqui,
    /// que es lo que impide que un enunciado con un apostrofo rompa el guion.
    /// </summary>
    private static string Text(string? value) =>
        value is null ? "NULL" : $"N'{value.Replace("'", "''", StringComparison.Ordinal)}'";

    private static string Number(decimal value) =>
        value.ToString("0.####", CultureInfo.InvariantCulture);
}
