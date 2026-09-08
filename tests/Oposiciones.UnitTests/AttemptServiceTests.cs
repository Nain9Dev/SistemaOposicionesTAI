using Oposiciones.Application.Services;
using Oposiciones.Domain.Exceptions;
using Oposiciones.Domain.Interfaces;
using Xunit;

namespace Oposiciones.UnitTests;

/// <summary>
/// El controlador de intentos era anonimo y aceptaba el propietario como texto en el cuerpo:
/// cualquiera podia responder o cerrar el intento de otra persona conociendo su identificador.
/// </summary>
public class AttemptServiceTests
{
    private const string Owner = "u7";
    private const string Intruder = "u9";

    private readonly FakeAttemptRepository _repository = new();
    private readonly AttemptService _service;

    public AttemptServiceTests()
    {
        _service = new AttemptService(_repository);
    }

    [Fact]
    public async Task Empezar_un_test_inexistente_devuelve_no_encontrado()
    {
        _repository.NextAttemptId = 0;

        await Assert.ThrowsAsync<NotFoundException>(() => _service.StartAsync(404, Owner));
    }

    [Fact]
    public async Task Responder_un_intento_ajeno_esta_prohibido()
    {
        _repository.Owner = Owner;

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _service.RegisterAnswerAsync(1, Intruder, questionId: 1, answerOptionId: 1));
    }

    [Fact]
    public async Task Cerrar_un_intento_ajeno_esta_prohibido()
    {
        _repository.Owner = Owner;

        await Assert.ThrowsAsync<ForbiddenException>(() => _service.FinishAsync(1, Intruder));
    }

    [Fact]
    public async Task Un_intento_inexistente_no_revela_su_ausencia_como_error_generico()
    {
        _repository.Owner = null;

        await Assert.ThrowsAsync<NotFoundException>(() => _service.FinishAsync(1, Owner));
    }

    [Fact]
    public async Task Un_intento_cerrado_no_admite_mas_respuestas()
    {
        _repository.Owner = Owner;
        _repository.Finished = true;

        await Assert.ThrowsAsync<ConflictException>(
            () => _service.RegisterAnswerAsync(1, Owner, questionId: 1, answerOptionId: 1));
    }

    [Fact]
    public async Task Un_intento_no_se_puede_cerrar_dos_veces()
    {
        _repository.Owner = Owner;
        _repository.Finished = true;

        await Assert.ThrowsAsync<ConflictException>(() => _service.FinishAsync(1, Owner));
    }

    [Fact]
    public async Task Una_respuesta_de_otro_test_se_rechaza()
    {
        _repository.Owner = Owner;
        _repository.AnswerAccepted = false;

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => _service.RegisterAnswerAsync(1, Owner, questionId: 999, answerOptionId: 999));
    }

    [Fact]
    public async Task El_propietario_puede_responder_y_cerrar()
    {
        _repository.Owner = Owner;

        await _service.RegisterAnswerAsync(1, Owner, questionId: 1, answerOptionId: 1);
        var resultado = await _service.FinishAsync(1, Owner);

        Assert.Equal(1, resultado.AttemptId);
    }

    private sealed class FakeAttemptRepository : IAttemptRepository
    {
        public string? Owner { get; set; } = "u7";
        public bool Finished { get; set; }
        public bool AnswerAccepted { get; set; } = true;
        public long NextAttemptId { get; set; } = 1;

        public Task<long> StartAsync(long testId, string userName, CancellationToken cancellationToken = default) =>
            Task.FromResult(NextAttemptId);

        public Task<bool> AnswerAsync(long attemptId, long questionId, long? answerOptionId, CancellationToken cancellationToken = default) =>
            Task.FromResult(AnswerAccepted);

        public Task<FinishAttemptResult> FinishAsync(long attemptId, CancellationToken cancellationToken = default) =>
            Task.FromResult(new FinishAttemptResult { AttemptId = attemptId });

        public Task<string?> GetOwnerAsync(long attemptId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Owner);

        public Task<bool> IsFinishedAsync(long attemptId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Finished);
    }
}
