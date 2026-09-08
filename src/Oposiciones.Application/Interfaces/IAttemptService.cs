using Oposiciones.Domain.Interfaces;

namespace Oposiciones.Application.Interfaces;

/// <summary>
/// Orquesta el ciclo de vida de un intento aplicando las reglas de propiedad. La version
/// anterior exponia los repositorios directamente, de modo que cualquier cliente podia
/// responder o cerrar el intento de otra persona indicando su identificador.
/// </summary>
public interface IAttemptService
{
    Task<long> StartAsync(long testId, string owner, CancellationToken cancellationToken = default);

    Task RegisterAnswerAsync(
        long attemptId,
        string owner,
        long questionId,
        long? answerOptionId,
        CancellationToken cancellationToken = default);

    Task<FinishAttemptResult> FinishAsync(long attemptId, string owner, CancellationToken cancellationToken = default);
}
