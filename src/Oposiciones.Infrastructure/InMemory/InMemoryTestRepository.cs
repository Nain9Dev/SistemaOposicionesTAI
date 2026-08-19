using Oposiciones.Domain.Abstractions;
using Oposiciones.Domain.Assessments;
using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Scoring;

namespace Oposiciones.Infrastructure.InMemory;

/// <summary>Tests generados durante la vida del proceso.</summary>
public sealed class InMemoryTestRepository : ITestRepository
{
    private readonly InMemoryStore _store;
    private readonly TimeProvider _clock;

    public InMemoryTestRepository(InMemoryStore store, TimeProvider clock)
    {
        _store = store;
        _clock = clock;
    }

    public Task<GeneratedTest> CreateAsync(
        TestBlueprint blueprint,
        string title,
        int seed,
        int durationMinutes,
        IReadOnlyList<Question> questions,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(blueprint);
        ArgumentNullException.ThrowIfNull(questions);

        ExamProfile? exam = _store.Catalog.FindExam(blueprint.ExamCode);
        ScoringPolicy scoring = blueprint.ScoringOverride
            ?? exam?.Scoring
            ?? ScoringPolicy.Default;

        var test = new GeneratedTest
        {
            Id = _store.NextTestId(),
            ExamCode = blueprint.ExamCode,
            Title = title,
            Mode = blueprint.Mode,
            Seed = seed,
            DurationMinutes = durationMinutes,
            Scoring = scoring,
            CreatedAt = _clock.GetUtcNow(),
            Questions = questions,
        };

        _store.SaveTest(test);
        return Task.FromResult(test);
    }

    public Task<GeneratedTest?> GetAsync(long testId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_store.FindTest(testId));
}
