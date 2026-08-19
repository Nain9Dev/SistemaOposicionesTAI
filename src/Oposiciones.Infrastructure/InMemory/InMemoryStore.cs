using System.Collections.Concurrent;
using Oposiciones.Domain.Assessments;
using Oposiciones.Domain.Attempts;
using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Scoring;
using Oposiciones.Infrastructure.Content;

namespace Oposiciones.Infrastructure.InMemory;

/// <summary>
/// Estado del proveedor en memoria: catalogo inmutable cargado del contenido y los tests e
/// intentos creados durante la vida del proceso.
/// <para>
/// Existe para que la plataforma arranque y sea plenamente funcional sin SQL Server: es el modo
/// que usan la demo publica, las pruebas de integracion y cualquiera que clone el repositorio.
/// </para>
/// </summary>
public sealed class InMemoryStore
{
    private readonly ConcurrentDictionary<long, GeneratedTest> _tests = new();
    private readonly ConcurrentDictionary<long, AttemptState> _attempts = new();
    private long _testSequence;
    private long _attemptSequence;

    public InMemoryStore(ContentCatalog catalog)
    {
        Catalog = catalog;
        BlockNames = catalog.Exams
            .SelectMany(exam => exam.Blocks)
            .ToDictionary(
                block => $"{block.ExamCode}|{block.Code}",
                block => block.Name,
                StringComparer.OrdinalIgnoreCase);
    }

    public ContentCatalog Catalog { get; }

    /// <summary>Nombres de bloque indexados por convocatoria y codigo, para etiquetar la analitica.</summary>
    public IReadOnlyDictionary<string, string> BlockNames { get; }

    public string GetBlockName(string examCode, string blockCode) =>
        BlockNames.TryGetValue($"{examCode}|{blockCode}", out string? name) ? name : $"Bloque {blockCode}";

    public long NextTestId() => Interlocked.Increment(ref _testSequence);

    public long NextAttemptId() => Interlocked.Increment(ref _attemptSequence);

    public void SaveTest(GeneratedTest test) => _tests[test.Id] = test;

    public GeneratedTest? FindTest(long testId) =>
        _tests.TryGetValue(testId, out GeneratedTest? test) ? test : null;

    public void SaveAttempt(AttemptState attempt) => _attempts[attempt.Id] = attempt;

    public AttemptState? FindAttempt(long attemptId) =>
        _attempts.TryGetValue(attemptId, out AttemptState? attempt) ? attempt : null;

    public IEnumerable<AttemptState> Attempts => _attempts.Values;
}

/// <summary>Intento en curso o finalizado, con sus respuestas.</summary>
public sealed class AttemptState
{
    private readonly ConcurrentDictionary<long, long?> _answers = new();

    public required long Id { get; init; }

    public required long TestId { get; init; }

    public required string UserName { get; init; }

    public required string ExamCode { get; init; }

    public required string TestTitle { get; init; }

    public TestMode Mode { get; init; }

    public required DateTimeOffset StartedAt { get; init; }

    public DateTimeOffset? FinishedAt { get; set; }

    public ScoreBreakdown? Score { get; set; }

    public IReadOnlyDictionary<long, long?> Answers => _answers;

    /// <summary>Registra la respuesta a una pregunta; sobrescribe la anterior si ya existia.</summary>
    public void SetAnswer(long questionId, long? optionId) => _answers[questionId] = optionId;

    public Attempt ToDomain() => new()
    {
        Id = Id,
        TestId = TestId,
        UserName = UserName,
        ExamCode = ExamCode,
        TestTitle = TestTitle,
        Mode = Mode,
        StartedAt = StartedAt,
        FinishedAt = FinishedAt,
        Score = Score,
    };

    /// <summary>Recuento por bloque y tema de un intento ya corregido, para el historico.</summary>
    public IEnumerable<AnswerSheetRow> BuildRows(GeneratedTest test, Func<string, string, string> blockNameResolver)
    {
        foreach (Question question in test.Questions)
        {
            _answers.TryGetValue(question.Id, out long? selected);
            yield return new AnswerSheetRow(
                question.Id,
                selected,
                question.CorrectOption?.Id,
                question.BlockCode,
                blockNameResolver(test.ExamCode, question.BlockCode),
                question.TopicNumber,
                question.TopicTitle);
        }
    }
}
