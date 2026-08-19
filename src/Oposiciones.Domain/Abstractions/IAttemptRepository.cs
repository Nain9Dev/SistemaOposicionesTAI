using Oposiciones.Domain.Attempts;
using Oposiciones.Domain.Common;
using Oposiciones.Domain.Scoring;

namespace Oposiciones.Domain.Abstractions;

/// <summary>
/// Persistencia de intentos. Deliberadamente no calcula notas: el almacen guarda respuestas y
/// resultados, mientras que la correccion vive en <see cref="AttemptGrader"/> y en el baremo de
/// la convocatoria. Asi un cambio de baremo no obliga a tocar procedimientos almacenados.
/// </summary>
public interface IAttemptRepository
{
    Task<Attempt> StartAsync(long testId, string userName, CancellationToken cancellationToken = default);

    Task<Attempt?> GetAsync(long attemptId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra o sustituye la respuesta a una pregunta. Un <paramref name="answerOptionId"/> nulo
    /// deja la pregunta en blanco, que es una decision valida cuando el baremo penaliza el fallo.
    /// </summary>
    Task<bool> AnswerAsync(
        long attemptId,
        long questionId,
        long? answerOptionId,
        CancellationToken cancellationToken = default);

    /// <summary>Hoja de respuestas completa del intento, incluidas las preguntas sin contestar.</summary>
    Task<AnswerSheet?> GetAnswerSheetAsync(long attemptId, CancellationToken cancellationToken = default);

    /// <summary>Cierra el intento almacenando la correccion ya calculada por el dominio.</summary>
    Task CompleteAsync(
        long attemptId,
        ScoreBreakdown score,
        DateTimeOffset finishedAt,
        CancellationToken cancellationToken = default);

    Task<PagedResult<AttemptSummary>> GetHistoryAsync(
        string userName,
        string? examCode,
        Paging paging,
        CancellationToken cancellationToken = default);

    /// <summary>Rendimiento acumulado por tema de un usuario, base del plan de repaso.</summary>
    Task<IReadOnlyList<PerformanceSlice>> GetTopicPerformanceAsync(
        string userName,
        string? examCode,
        CancellationToken cancellationToken = default);
}
