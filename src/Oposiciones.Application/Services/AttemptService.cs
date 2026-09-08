using Oposiciones.Application.Interfaces;
using Oposiciones.Domain.Exceptions;
using Oposiciones.Domain.Interfaces;

namespace Oposiciones.Application.Services;

public sealed class AttemptService : IAttemptService
{
    private readonly IAttemptRepository _attemptRepository;

    public AttemptService(IAttemptRepository attemptRepository)
    {
        _attemptRepository = attemptRepository;
    }

    public async Task<long> StartAsync(long testId, string owner, CancellationToken cancellationToken = default)
    {
        var attemptId = await _attemptRepository.StartAsync(testId, owner, cancellationToken);
        if (attemptId == 0)
        {
            throw NotFoundException.For("el test", testId);
        }

        return attemptId;
    }

    public async Task RegisterAnswerAsync(
        long attemptId,
        string owner,
        long questionId,
        long? answerOptionId,
        CancellationToken cancellationToken = default)
    {
        await EnsureOwnershipAsync(attemptId, owner, cancellationToken);

        if (await _attemptRepository.IsFinishedAsync(attemptId, cancellationToken))
        {
            throw new ConflictException("El intento ya esta finalizado y no admite mas respuestas.");
        }

        var registered = await _attemptRepository.AnswerAsync(attemptId, questionId, answerOptionId, cancellationToken);
        if (!registered)
        {
            throw new BusinessRuleException(
                "La pregunta no pertenece al test del intento o la opcion no corresponde a esa pregunta.");
        }
    }

    public async Task<FinishAttemptResult> FinishAsync(long attemptId, string owner, CancellationToken cancellationToken = default)
    {
        await EnsureOwnershipAsync(attemptId, owner, cancellationToken);

        if (await _attemptRepository.IsFinishedAsync(attemptId, cancellationToken))
        {
            throw new ConflictException("El intento ya estaba finalizado.");
        }

        return await _attemptRepository.FinishAsync(attemptId, cancellationToken);
    }

    private async Task EnsureOwnershipAsync(long attemptId, string owner, CancellationToken cancellationToken)
    {
        var actualOwner = await _attemptRepository.GetOwnerAsync(attemptId, cancellationToken);

        if (actualOwner is null)
        {
            throw NotFoundException.For("el intento", attemptId);
        }

        if (!string.Equals(actualOwner, owner, StringComparison.Ordinal))
        {
            throw new ForbiddenException("El intento pertenece a otro usuario.");
        }
    }
}
