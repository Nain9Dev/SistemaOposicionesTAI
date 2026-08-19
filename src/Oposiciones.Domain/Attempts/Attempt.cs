using Oposiciones.Domain.Assessments;
using Oposiciones.Domain.Scoring;

namespace Oposiciones.Domain.Attempts;

/// <summary>Ejecucion de un test por parte de un usuario.</summary>
public sealed class Attempt
{
    public long Id { get; init; }

    public long TestId { get; init; }

    public required string UserName { get; init; }

    public string ExamCode { get; init; } = string.Empty;

    public string TestTitle { get; init; } = string.Empty;

    public TestMode Mode { get; init; }

    public DateTimeOffset StartedAt { get; init; }

    public DateTimeOffset? FinishedAt { get; init; }

    public ScoreBreakdown? Score { get; init; }

    public bool IsFinished => FinishedAt is not null;

    /// <summary>Tiempo empleado. Nulo mientras el intento sigue abierto.</summary>
    public TimeSpan? Duration => FinishedAt is null ? null : FinishedAt.Value - StartedAt;
}

/// <summary>Fila resumida del historial de intentos, pensada para listados paginados.</summary>
public sealed record AttemptSummary(
    long AttemptId,
    long TestId,
    string TestTitle,
    string ExamCode,
    TestMode Mode,
    DateTimeOffset StartedAt,
    DateTimeOffset? FinishedAt,
    int TotalQuestions,
    int Correct,
    int Incorrect,
    int Blank,
    decimal? ScaledScore,
    decimal? AccuracyPercent);
