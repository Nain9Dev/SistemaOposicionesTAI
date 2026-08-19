using Oposiciones.Domain.Scoring;

namespace Oposiciones.Domain.Attempts;

/// <summary>
/// Correccion completa de un intento: nota, desglose y rendimiento por bloque y por tema.
/// El desglose es el que senala donde conviene repasar, que es el objetivo de la plataforma.
/// </summary>
public sealed record AttemptResult(
    long AttemptId,
    long TestId,
    string ExamCode,
    DateTimeOffset FinishedAt,
    ScoreBreakdown Score,
    IReadOnlyList<PerformanceSlice> ByBlock,
    IReadOnlyList<PerformanceSlice> ByTopic)
{
    /// <summary>Areas mas debiles, ordenadas por porcentaje de acierto ascendente.</summary>
    public IEnumerable<PerformanceSlice> WeakestTopics(int take = 5) =>
        ByTopic.Where(slice => slice.Answered > 0)
               .OrderBy(slice => slice.AccuracyPercent)
               .ThenByDescending(slice => slice.Incorrect)
               .Take(take);
}

/// <summary>Rendimiento agregado de un bloque o de un tema.</summary>
public sealed record PerformanceSlice(
    string Key,
    string Label,
    int TotalQuestions,
    int Correct,
    int Incorrect,
    int Blank,
    decimal AccuracyPercent)
{
    public int Answered => Correct + Incorrect;

    /// <summary>Bloque al que pertenece el corte, cuando aplica.</summary>
    public string? BlockCode { get; init; }

    /// <summary>Numero de tema, presente solo en los cortes por tema.</summary>
    public int? TopicNumber { get; init; }
}
