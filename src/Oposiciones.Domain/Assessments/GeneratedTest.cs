using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Scoring;

namespace Oposiciones.Domain.Assessments;

/// <summary>
/// Test ya materializado: preguntas concretas, en un orden concreto, con el baremo y la duracion
/// con los que debe corregirse. Guardar la semilla permite regenerarlo identico mas adelante.
/// </summary>
public sealed class GeneratedTest
{
    public long Id { get; init; }

    public required string ExamCode { get; init; }

    public required string Title { get; init; }

    public TestMode Mode { get; init; } = TestMode.Study;

    /// <summary>Semilla con la que se extrajeron y ordenaron las preguntas.</summary>
    public int Seed { get; init; }

    public int DurationMinutes { get; init; }

    public ScoringPolicy Scoring { get; init; } = ScoringPolicy.Default;

    public DateTimeOffset CreatedAt { get; init; }

    public IReadOnlyList<Question> Questions { get; init; } = Array.Empty<Question>();

    public int TotalQuestions => Questions.Count;
}
