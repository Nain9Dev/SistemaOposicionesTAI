namespace Oposiciones.Domain.Scoring;

/// <summary>
/// Resultado de aplicar un <see cref="ScoringPolicy"/>: se devuelve el desglose completo y no
/// solo la nota, para que el opositor pueda comprobar como se ha calculado.
/// </summary>
public sealed record ScoreBreakdown(
    int TotalQuestions,
    int Correct,
    int Incorrect,
    int Blank,
    decimal RawScore,
    decimal ScaledScore,
    decimal MaxScore,
    decimal PassMark,
    decimal AccuracyPercent,
    bool Passed)
{
    public static ScoreBreakdown Empty { get; } = new(0, 0, 0, 0, 0m, 0m, 0m, 0m, 0m, false);

    /// <summary>Preguntas efectivamente contestadas (aciertos mas fallos).</summary>
    public int Answered => Correct + Incorrect;
}
