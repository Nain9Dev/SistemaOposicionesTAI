using Oposiciones.Application.Common;
using Oposiciones.Application.Contracts;
using Oposiciones.Application.Mapping;
using Oposiciones.Domain.Abstractions;
using Oposiciones.Domain.Assessments;
using Oposiciones.Domain.Attempts;
using Oposiciones.Domain.Common;

namespace Oposiciones.Application.Services;

/// <summary>Ciclo de vida de un intento: iniciar, responder, corregir y revisar.</summary>
public interface IAttemptService
{
    Task<AttemptDto> StartAsync(StartAttemptRequest request, CancellationToken cancellationToken = default);

    Task<AttemptDto> GetAsync(long attemptId, CancellationToken cancellationToken = default);

    Task<bool> AnswerAsync(
        long attemptId,
        SubmitAnswerRequest request,
        CancellationToken cancellationToken = default);

    Task<AttemptResultDto> FinishAsync(long attemptId, CancellationToken cancellationToken = default);

    /// <summary>Test con soluciones y explicaciones. Solo disponible una vez cerrado el intento.</summary>
    Task<TestDto> GetReviewAsync(long attemptId, CancellationToken cancellationToken = default);

    Task<PagedResponse<AttemptSummaryDto>> GetHistoryAsync(
        string userName,
        string? examCode,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default);

    Task<StudyPlanDto> GetStudyPlanAsync(
        string userName,
        string? examCode,
        int maxRecommendations = 5,
        CancellationToken cancellationToken = default);
}

/// <inheritdoc />
public sealed class AttemptService : IAttemptService
{
    /// <summary>Por debajo de este porcentaje de acierto un tema entra en el plan de repaso.</summary>
    private const decimal ReviewThresholdPercent = 70m;

    /// <summary>Minimo de respuestas para que la estadistica de un tema sea significativa.</summary>
    private const int MinimumAnswersForAdvice = 3;

    private readonly IAttemptRepository _attempts;
    private readonly ITestRepository _tests;
    private readonly TimeProvider _clock;

    public AttemptService(IAttemptRepository attempts, ITestRepository tests, TimeProvider clock)
    {
        _attempts = attempts;
        _tests = tests;
        _clock = clock;
    }

    public async Task<AttemptDto> StartAsync(
        StartAttemptRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var validation = new ValidationBuilder();
        validation.AddIf(request.TestId <= 0, nameof(request.TestId), "Debe indicarse un test valido.");
        validation.ThrowIfInvalid();

        string userName = NormalizeUser(request.UserName);

        GeneratedTest? test = await _tests.GetAsync(request.TestId, cancellationToken).ConfigureAwait(false);
        if (test is null)
        {
            throw new NotFoundException("el test", request.TestId);
        }

        Attempt attempt = await _attempts.StartAsync(test.Id, userName, cancellationToken).ConfigureAwait(false);
        return attempt.ToDto();
    }

    public async Task<AttemptDto> GetAsync(long attemptId, CancellationToken cancellationToken = default)
    {
        Attempt attempt = await RequireAttemptAsync(attemptId, cancellationToken).ConfigureAwait(false);
        return attempt.ToDto();
    }

    public async Task<bool> AnswerAsync(
        long attemptId,
        SubmitAnswerRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var validation = new ValidationBuilder();
        validation.AddIf(request.QuestionId <= 0, nameof(request.QuestionId), "Debe indicarse una pregunta valida.");
        validation.AddIf(
            request.AnswerOptionId is <= 0,
            nameof(request.AnswerOptionId),
            "El identificador de opcion debe ser positivo, o nulo para dejar la pregunta en blanco.");
        validation.ThrowIfInvalid();

        Attempt attempt = await RequireAttemptAsync(attemptId, cancellationToken).ConfigureAwait(false);
        if (attempt.IsFinished)
        {
            throw new DomainException("El intento ya esta finalizado y no admite mas respuestas.");
        }

        return await _attempts
            .AnswerAsync(attemptId, request.QuestionId, request.AnswerOptionId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<AttemptResultDto> FinishAsync(
        long attemptId,
        CancellationToken cancellationToken = default)
    {
        Attempt attempt = await RequireAttemptAsync(attemptId, cancellationToken).ConfigureAwait(false);

        AnswerSheet? sheet = await _attempts.GetAnswerSheetAsync(attemptId, cancellationToken)
            .ConfigureAwait(false);
        if (sheet is null)
        {
            throw new NotFoundException("la hoja de respuestas del intento", attemptId);
        }

        // Reentrante a proposito: volver a finalizar un intento ya cerrado devuelve la misma
        // correccion sin alterar la fecha de cierre registrada.
        DateTimeOffset finishedAt = attempt.FinishedAt ?? _clock.GetUtcNow();
        AttemptResult result = AttemptGrader.Grade(sheet, sheet.Scoring, finishedAt);

        if (!attempt.IsFinished)
        {
            await _attempts.CompleteAsync(attemptId, result.Score, finishedAt, cancellationToken)
                .ConfigureAwait(false);
        }

        return result.ToDto();
    }

    public async Task<TestDto> GetReviewAsync(long attemptId, CancellationToken cancellationToken = default)
    {
        Attempt attempt = await RequireAttemptAsync(attemptId, cancellationToken).ConfigureAwait(false);
        if (!attempt.IsFinished)
        {
            throw new DomainException(
                "La revision con soluciones solo esta disponible cuando el intento se ha finalizado.");
        }

        GeneratedTest? test = await _tests.GetAsync(attempt.TestId, cancellationToken).ConfigureAwait(false);
        if (test is null)
        {
            throw new NotFoundException("el test", attempt.TestId);
        }

        return test.ToDto(includeSolutions: true);
    }

    public async Task<PagedResponse<AttemptSummaryDto>> GetHistoryAsync(
        string userName,
        string? examCode,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        PagedResult<AttemptSummary> history = await _attempts
            .GetHistoryAsync(
                NormalizeUser(userName),
                string.IsNullOrWhiteSpace(examCode) ? null : examCode.Trim(),
                Paging.Of(page, pageSize),
                cancellationToken)
            .ConfigureAwait(false);

        return history.ToResponse(summary => summary.ToDto());
    }

    public async Task<StudyPlanDto> GetStudyPlanAsync(
        string userName,
        string? examCode,
        int maxRecommendations = 5,
        CancellationToken cancellationToken = default)
    {
        string user = NormalizeUser(userName);
        string? exam = string.IsNullOrWhiteSpace(examCode) ? null : examCode.Trim();
        int take = Math.Clamp(maxRecommendations, 1, 25);

        IReadOnlyList<PerformanceSlice> performance = await _attempts
            .GetTopicPerformanceAsync(user, exam, cancellationToken)
            .ConfigureAwait(false);

        int analyzed = performance.Sum(slice => slice.Answered);
        int correct = performance.Sum(slice => slice.Correct);
        decimal overall = analyzed == 0
            ? 0m
            : Math.Round(correct / (decimal)analyzed * 100m, 2, MidpointRounding.AwayFromZero);

        List<StudyPlanItemDto> recommendations = performance
            .Where(slice => slice.Answered >= MinimumAnswersForAdvice)
            .Where(slice => slice.AccuracyPercent < ReviewThresholdPercent)
            .OrderBy(slice => slice.AccuracyPercent)
            .ThenByDescending(slice => slice.Incorrect)
            .Take(take)
            .Select(slice => new StudyPlanItemDto(
                slice.BlockCode ?? string.Empty,
                slice.TopicNumber ?? 0,
                slice.Label,
                slice.Answered,
                slice.AccuracyPercent,
                SuggestQuestionCount(slice),
                $"Acierto del {slice.AccuracyPercent:0.##}% sobre {slice.Answered} respuestas."))
            .ToList();

        return new StudyPlanDto(user, exam ?? string.Empty, analyzed, overall, recommendations);
    }

    /// <summary>
    /// Cuantas preguntas conviene repasar de un tema: cuanto mas lejos esta del umbral, mas carga,
    /// acotado entre 5 y 25 para que el plan siga siendo abordable en una sesion.
    /// </summary>
    private static int SuggestQuestionCount(PerformanceSlice slice)
    {
        decimal gap = ReviewThresholdPercent - slice.AccuracyPercent;
        int suggested = 5 + (int)Math.Ceiling(gap / 5m);
        return Math.Clamp(suggested, 5, 25);
    }

    private async Task<Attempt> RequireAttemptAsync(long attemptId, CancellationToken cancellationToken)
    {
        if (attemptId <= 0)
        {
            throw new NotFoundException("el intento", attemptId);
        }

        Attempt? attempt = await _attempts.GetAsync(attemptId, cancellationToken).ConfigureAwait(false);
        return attempt ?? throw new NotFoundException("el intento", attemptId);
    }

    private static string NormalizeUser(string? userName) =>
        string.IsNullOrWhiteSpace(userName) ? "demo" : userName.Trim();
}
