using Oposiciones.Domain.Catalog;

namespace Oposiciones.Infrastructure.Content;

/// <summary>
/// Contenido ya cargado, validado y con identificadores asignados de forma determinista.
/// Es inmutable: se construye una vez al arrancar y se comparte entre peticiones.
/// </summary>
public sealed class ContentCatalog
{
    public static ContentCatalog Empty { get; } = new(
        Array.Empty<ExamProfile>(),
        Array.Empty<Question>(),
        Array.Empty<string>());

    public ContentCatalog(
        IReadOnlyList<ExamProfile> exams,
        IReadOnlyList<Question> questions,
        IReadOnlyList<string> issues)
    {
        Exams = exams;
        Questions = questions;
        Issues = issues;

        ExamsByCode = exams.ToDictionary(exam => exam.Code, StringComparer.OrdinalIgnoreCase);
        QuestionsById = questions.ToDictionary(question => question.Id);
        TopicsById = exams
            .SelectMany(exam => exam.AllTopics())
            .ToDictionary(topic => topic.Id);
    }

    public IReadOnlyList<ExamProfile> Exams { get; }

    public IReadOnlyList<Question> Questions { get; }

    /// <summary>Problemas detectados al validar el contenido. Vacio significa contenido correcto.</summary>
    public IReadOnlyList<string> Issues { get; }

    public IReadOnlyDictionary<string, ExamProfile> ExamsByCode { get; }

    public IReadOnlyDictionary<long, Question> QuestionsById { get; }

    public IReadOnlyDictionary<int, SyllabusTopic> TopicsById { get; }

    public ExamProfile? FindExam(string? examCode) =>
        examCode is not null && ExamsByCode.TryGetValue(examCode, out ExamProfile? exam) ? exam : null;
}
