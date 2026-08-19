namespace Oposiciones.Domain.Scoring;

/// <summary>
/// Baremo de correccion de un ejercicio. Es un dato de la convocatoria, no una constante del
/// codigo: si unas bases cambian la penalizacion o la escala, basta editar el perfil del examen.
/// <para>
/// El baremo por defecto reproduce el del primer ejercicio del TAI: cada acierto suma un punto,
/// cada error descuenta un tercio de acierto, las respuestas en blanco no penalizan y la nota
/// final se escala sobre 50 puntos con un minimo de 25 para superar.
/// </para>
/// </summary>
public sealed record ScoringPolicy(
    decimal CorrectPoints,
    decimal IncorrectPoints,
    decimal BlankPoints,
    decimal ScaleMaxScore,
    decimal PassMark,
    bool ClampNegativeToZero = true)
{
    /// <summary>Baremo oficial del primer ejercicio del TAI.</summary>
    public static ScoringPolicy Default { get; } = new(
        CorrectPoints: 1m,
        IncorrectPoints: -1m / 3m,
        BlankPoints: 0m,
        ScaleMaxScore: 50m,
        PassMark: 25m);

    /// <summary>Baremo sin penalizacion, util para sesiones de repaso y modo estudio.</summary>
    public static ScoringPolicy NoPenalty { get; } = new(
        CorrectPoints: 1m,
        IncorrectPoints: 0m,
        BlankPoints: 0m,
        ScaleMaxScore: 100m,
        PassMark: 50m);

    /// <summary>
    /// Corrige un ejercicio a partir del recuento de respuestas.
    /// </summary>
    /// <param name="correct">Respuestas acertadas.</param>
    /// <param name="incorrect">Respuestas falladas.</param>
    /// <param name="blank">Preguntas sin contestar.</param>
    public ScoreBreakdown Evaluate(int correct, int incorrect, int blank)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(correct);
        ArgumentOutOfRangeException.ThrowIfNegative(incorrect);
        ArgumentOutOfRangeException.ThrowIfNegative(blank);

        int total = correct + incorrect + blank;
        decimal rawScore = (correct * CorrectPoints)
            + (incorrect * IncorrectPoints)
            + (blank * BlankPoints);

        if (ClampNegativeToZero && rawScore < 0m)
        {
            rawScore = 0m;
        }

        decimal maxRawScore = total * CorrectPoints;
        decimal scaledScore = maxRawScore <= 0m
            ? 0m
            : rawScore / maxRawScore * ScaleMaxScore;

        decimal accuracyPercent = total == 0
            ? 0m
            : correct / (decimal)total * 100m;

        return new ScoreBreakdown(
            TotalQuestions: total,
            Correct: correct,
            Incorrect: incorrect,
            Blank: blank,
            RawScore: Round(rawScore),
            ScaledScore: Round(scaledScore),
            MaxScore: ScaleMaxScore,
            PassMark: PassMark,
            AccuracyPercent: Round(accuracyPercent),
            Passed: Round(scaledScore) >= PassMark);
    }

    private static decimal Round(decimal value) => Math.Round(value, 3, MidpointRounding.AwayFromZero);
}
