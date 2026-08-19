using Oposiciones.Domain.Abstractions;
using Oposiciones.Domain.Assessments;
using Oposiciones.Domain.Attempts;
using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Common;
using Oposiciones.Domain.Scoring;

namespace Oposiciones.Infrastructure.InMemory;

/// <summary>Intentos almacenados en memoria del proceso.</summary>
public sealed class InMemoryAttemptRepository : IAttemptRepository
{
    private readonly InMemoryStore _store;
    private readonly TimeProvider _clock;

    public InMemoryAttemptRepository(InMemoryStore store, TimeProvider clock)
    {
        _store = store;
        _clock = clock;
    }

    public Task<Attempt> StartAsync(
        long testId,
        string userName,
        CancellationToken cancellationToken = default)
    {
        GeneratedTest test = _store.FindTest(testId)
            ?? throw new NotFoundException("el test", testId);

        var state = new AttemptState
        {
            Id = _store.NextAttemptId(),
            TestId = test.Id,
            UserName = userName,
            ExamCode = test.ExamCode,
            TestTitle = test.Title,
            Mode = test.Mode,
            StartedAt = _clock.GetUtcNow(),
        };

        _store.SaveAttempt(state);
        return Task.FromResult(state.ToDomain());
    }

    public Task<Attempt?> GetAsync(long attemptId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_store.FindAttempt(attemptId)?.ToDomain());

    public Task<bool> AnswerAsync(
        long attemptId,
        long questionId,
        long? answerOptionId,
        CancellationToken cancellationToken = default)
    {
        AttemptState? attempt = _store.FindAttempt(attemptId);
        if (attempt is null)
        {
            throw new NotFoundException("el intento", attemptId);
        }

        GeneratedTest test = _store.FindTest(attempt.TestId)
            ?? throw new NotFoundException("el test", attempt.TestId);

        Question? question = test.Questions.FirstOrDefault(q => q.Id == questionId);
        if (question is null)
        {
            throw new DomainException($"La pregunta {questionId} no forma parte del test {test.Id}.");
        }

        if (answerOptionId is long optionId && question.Options.All(option => option.Id != optionId))
        {
            throw new DomainException($"La opcion {optionId} no pertenece a la pregunta {questionId}.");
        }

        attempt.SetAnswer(questionId, answerOptionId);
        return Task.FromResult(true);
    }

    public Task<AnswerSheet?> GetAnswerSheetAsync(
        long attemptId,
        CancellationToken cancellationToken = default)
    {
        AttemptState? attempt = _store.FindAttempt(attemptId);
        if (attempt is null)
        {
            return Task.FromResult<AnswerSheet?>(null);
        }

        GeneratedTest? test = _store.FindTest(attempt.TestId);
        if (test is null)
        {
            return Task.FromResult<AnswerSheet?>(null);
        }

        var sheet = new AnswerSheet(
            attempt.Id,
            test.Id,
            test.ExamCode,
            test.Scoring,
            attempt.BuildRows(test, _store.GetBlockName).ToList());

        return Task.FromResult<AnswerSheet?>(sheet);
    }

    public Task CompleteAsync(
        long attemptId,
        ScoreBreakdown score,
        DateTimeOffset finishedAt,
        CancellationToken cancellationToken = default)
    {
        AttemptState attempt = _store.FindAttempt(attemptId)
            ?? throw new NotFoundException("el intento", attemptId);

        attempt.FinishedAt = finishedAt;
        attempt.Score = score;
        return Task.CompletedTask;
    }

    public Task<PagedResult<AttemptSummary>> GetHistoryAsync(
        string userName,
        string? examCode,
        Paging paging,
        CancellationToken cancellationToken = default)
    {
        List<AttemptSummary> history = _store.Attempts
            .Where(attempt => string.Equals(attempt.UserName, userName, StringComparison.OrdinalIgnoreCase))
            .Where(attempt => examCode is null
                || string.Equals(attempt.ExamCode, examCode, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(attempt => attempt.StartedAt)
            .ThenByDescending(attempt => attempt.Id)
            .Select(ToSummary)
            .ToList();

        List<AttemptSummary> page = history
            .Skip(paging.Offset)
            .Take(paging.PageSize)
            .ToList();

        return Task.FromResult(PagedResult<AttemptSummary>.From(page, paging, history.Count));
    }

    public Task<IReadOnlyList<PerformanceSlice>> GetTopicPerformanceAsync(
        string userName,
        string? examCode,
        CancellationToken cancellationToken = default)
    {
        var rows = new List<AnswerSheetRow>();

        foreach (AttemptState attempt in _store.Attempts)
        {
            if (!string.Equals(attempt.UserName, userName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (examCode is not null
                && !string.Equals(attempt.ExamCode, examCode, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // Solo cuentan los intentos cerrados: incluir uno a medias distorsionaria el plan de
            // repaso con preguntas que el opositor aun no ha llegado a leer.
            if (attempt.FinishedAt is null)
            {
                continue;
            }

            GeneratedTest? test = _store.FindTest(attempt.TestId);
            if (test is null)
            {
                continue;
            }

            rows.AddRange(attempt.BuildRows(test, _store.GetBlockName));
        }

        IReadOnlyList<PerformanceSlice> performance = rows
            .GroupBy(row => $"{row.BlockCode}.{row.TopicNumber}", StringComparer.Ordinal)
            .Select(group =>
            {
                AnswerSheetRow first = group.First();
                int total = group.Count();
                int correct = group.Count(row => row.IsCorrect);
                int incorrect = group.Count(row => row.IsIncorrect);
                int blank = group.Count(row => row.IsBlank);

                return new PerformanceSlice(
                    group.Key,
                    $"Tema {first.TopicNumber}. {first.TopicTitle}",
                    total,
                    correct,
                    incorrect,
                    blank,
                    total == 0 ? 0m : Math.Round(correct / (decimal)total * 100m, 2, MidpointRounding.AwayFromZero))
                {
                    BlockCode = first.BlockCode,
                    TopicNumber = first.TopicNumber,
                };
            })
            .OrderBy(slice => slice.Key, StringComparer.Ordinal)
            .ToList();

        return Task.FromResult(performance);
    }

    private AttemptSummary ToSummary(AttemptState attempt)
    {
        GeneratedTest? test = _store.FindTest(attempt.TestId);
        int totalQuestions = attempt.Score?.TotalQuestions ?? test?.TotalQuestions ?? 0;

        return new AttemptSummary(
            attempt.Id,
            attempt.TestId,
            attempt.TestTitle,
            attempt.ExamCode,
            attempt.Mode,
            attempt.StartedAt,
            attempt.FinishedAt,
            totalQuestions,
            attempt.Score?.Correct ?? 0,
            attempt.Score?.Incorrect ?? 0,
            attempt.Score?.Blank ?? 0,
            attempt.Score?.ScaledScore,
            attempt.Score?.AccuracyPercent);
    }
}
