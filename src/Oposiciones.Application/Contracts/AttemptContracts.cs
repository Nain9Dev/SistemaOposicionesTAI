namespace Oposiciones.Application.Contracts;

/// <summary>Peticion de inicio de un intento sobre un test ya generado.</summary>
public sealed record StartAttemptRequest
{
    public long TestId { get; init; }

    /// <summary>Identificador del opositor. Si se omite se usa <c>demo</c>.</summary>
    public string UserName { get; init; } = "demo";
}

/// <summary>
/// Respuesta a una pregunta. <see cref="AnswerOptionId"/> nulo deja la pregunta en blanco,
/// que es una jugada legitima cuando el baremo penaliza los fallos.
/// </summary>
public sealed record SubmitAnswerRequest
{
    public long QuestionId { get; init; }

    public long? AnswerOptionId { get; init; }
}

/// <summary>Estado de un intento.</summary>
public sealed record AttemptDto(
    long AttemptId,
    long TestId,
    string UserName,
    string ExamCode,
    string TestTitle,
    string Mode,
    DateTimeOffset StartedAt,
    DateTimeOffset? FinishedAt,
    int? DurationSeconds,
    ScoreDto? Score);

/// <summary>Desglose de la nota, con los datos necesarios para reproducir el calculo.</summary>
public sealed record ScoreDto(
    int TotalQuestions,
    int Correct,
    int Incorrect,
    int Blank,
    decimal RawScore,
    decimal ScaledScore,
    decimal MaxScore,
    decimal PassMark,
    decimal AccuracyPercent,
    bool Passed);

/// <summary>Correccion completa de un intento con el rendimiento por bloque y por tema.</summary>
public sealed record AttemptResultDto(
    long AttemptId,
    long TestId,
    string ExamCode,
    DateTimeOffset FinishedAt,
    ScoreDto Score,
    IReadOnlyList<PerformanceDto> ByBlock,
    IReadOnlyList<PerformanceDto> ByTopic,
    IReadOnlyList<PerformanceDto> WeakestTopics);

/// <summary>Rendimiento agregado de un bloque o tema.</summary>
public sealed record PerformanceDto(
    string Key,
    string Label,
    int TotalQuestions,
    int Correct,
    int Incorrect,
    int Blank,
    decimal AccuracyPercent);

/// <summary>Fila del historial de intentos.</summary>
public sealed record AttemptSummaryDto(
    long AttemptId,
    long TestId,
    string TestTitle,
    string ExamCode,
    string Mode,
    DateTimeOffset StartedAt,
    DateTimeOffset? FinishedAt,
    int TotalQuestions,
    int Correct,
    int Incorrect,
    int Blank,
    decimal? ScaledScore,
    decimal? AccuracyPercent);

/// <summary>
/// Plan de repaso derivado del historial: que temas conviene reforzar y con cuantas preguntas,
/// listo para alimentar directamente una peticion de generacion de test.
/// </summary>
public sealed record StudyPlanDto(
    string UserName,
    string ExamCode,
    int AnalyzedQuestions,
    decimal OverallAccuracyPercent,
    IReadOnlyList<StudyPlanItemDto> Recommendations);

/// <summary>Tema recomendado para repasar.</summary>
public sealed record StudyPlanItemDto(
    string BlockCode,
    int TopicNumber,
    string Label,
    int Answered,
    decimal AccuracyPercent,
    int SuggestedQuestions,
    string Reason);
