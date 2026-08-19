using Oposiciones.Domain.Abstractions;
using Oposiciones.Domain.Assessments;
using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Common;

namespace Oposiciones.Infrastructure.InMemory;

/// <summary>Banco de preguntas servido desde el contenido cargado en memoria.</summary>
public sealed class InMemoryQuestionRepository : IQuestionRepository
{
    private readonly InMemoryStore _store;

    public InMemoryQuestionRepository(InMemoryStore store)
    {
        _store = store;
    }

    public Task<PagedResult<Question>> SearchAsync(
        QuestionQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        List<Question> matches = Filter(query).OrderBy(question => question.Id).ToList();

        List<Question> page = matches
            .Skip(query.Paging.Offset)
            .Take(query.Paging.PageSize)
            .ToList();

        return Task.FromResult(PagedResult<Question>.From(page, query.Paging, matches.Count));
    }

    public Task<Question?> GetAsync(long questionId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_store.Catalog.QuestionsById.TryGetValue(questionId, out Question? question)
            ? question
            : null);

    public Task<IReadOnlyList<Question>> DrawAsync(
        QuestionDraw draw,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draw);

        // Se ordena por Id antes de barajar para que el punto de partida sea siempre el mismo:
        // sin ese orden estable la misma semilla daria conjuntos distintos entre ejecuciones.
        List<Question> candidates = Candidates(draw).OrderBy(question => question.Id).ToList();

        var random = new SeededRandom(draw.Seed);
        random.Shuffle(candidates);

        IReadOnlyList<Question> selection = candidates.Take(Math.Max(0, draw.Count)).ToList();
        return Task.FromResult(selection);
    }

    public Task<int> CountAvailableAsync(QuestionDraw draw, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draw);
        return Task.FromResult(Candidates(draw).Count());
    }

    public Task<IReadOnlyList<TopicCoverage>> GetCoverageAsync(
        string examCode,
        CancellationToken cancellationToken = default)
    {
        ExamProfile? exam = _store.Catalog.FindExam(examCode);
        if (exam is null)
        {
            return Task.FromResult<IReadOnlyList<TopicCoverage>>(Array.Empty<TopicCoverage>());
        }

        Dictionary<int, List<Question>> byTopic = _store.Catalog.Questions
            .Where(question => string.Equals(question.ExamCode, exam.Code, StringComparison.OrdinalIgnoreCase))
            .GroupBy(question => question.TopicId)
            .ToDictionary(group => group.Key, group => group.ToList());

        // Se recorre el temario y no las preguntas: asi los temas sin material aparecen con cero,
        // que es justo la informacion que hace falta para saber que queda por rellenar.
        var coverage = new List<TopicCoverage>();
        foreach (SyllabusBlock block in exam.Blocks.OrderBy(block => block.DisplayOrder))
        {
            foreach (SyllabusTopic topic in block.Topics.OrderBy(topic => topic.Number))
            {
                byTopic.TryGetValue(topic.Id, out List<Question>? questions);
                questions ??= new List<Question>();
                List<Question> active = questions.Where(question => question.IsActive).ToList();

                coverage.Add(new TopicCoverage(
                    topic.Id,
                    block.Code,
                    block.Name,
                    topic.Number,
                    topic.Title,
                    questions.Count,
                    active.Count,
                    active.Count == 0
                        ? 0m
                        : Math.Round(active.Average(question => (decimal)question.Difficulty), 2,
                            MidpointRounding.AwayFromZero)));
            }
        }

        return Task.FromResult<IReadOnlyList<TopicCoverage>>(coverage);
    }

    private IEnumerable<Question> Candidates(QuestionDraw draw)
    {
        IEnumerable<Question> questions = _store.Catalog.Questions.Where(question => question.IsActive);

        questions = questions.Where(question =>
            string.Equals(question.ExamCode, draw.ExamCode, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(draw.BlockCode))
        {
            questions = questions.Where(question =>
                string.Equals(question.BlockCode, draw.BlockCode, StringComparison.OrdinalIgnoreCase));
        }

        if (draw.TopicId is int topicId)
        {
            questions = questions.Where(question => question.TopicId == topicId);
        }

        if (draw.TopicNumber is int topicNumber)
        {
            questions = questions.Where(question => question.TopicNumber == topicNumber);
        }

        if (draw.Difficulties.Count > 0)
        {
            questions = questions.Where(question => draw.Difficulties.Contains(question.Difficulty));
        }

        if (draw.Tags.Count > 0)
        {
            questions = questions.Where(question =>
                draw.Tags.All(tag => question.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase)));
        }

        if (draw.ExcludeQuestionIds.Count > 0)
        {
            var excluded = draw.ExcludeQuestionIds.ToHashSet();
            questions = questions.Where(question => !excluded.Contains(question.Id));
        }

        return questions;
    }

    private IEnumerable<Question> Filter(QuestionQuery query)
    {
        IEnumerable<Question> questions = _store.Catalog.Questions;

        if (query.IsActive is bool isActive)
        {
            questions = questions.Where(question => question.IsActive == isActive);
        }

        if (!string.IsNullOrWhiteSpace(query.ExamCode))
        {
            questions = questions.Where(question =>
                string.Equals(question.ExamCode, query.ExamCode, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.BlockCode))
        {
            questions = questions.Where(question =>
                string.Equals(question.BlockCode, query.BlockCode, StringComparison.OrdinalIgnoreCase));
        }

        if (query.TopicId is int topicId)
        {
            questions = questions.Where(question => question.TopicId == topicId);
        }

        if (query.TopicNumber is int topicNumber)
        {
            questions = questions.Where(question => question.TopicNumber == topicNumber);
        }

        if (query.Difficulties.Count > 0)
        {
            questions = questions.Where(question => query.Difficulties.Contains(question.Difficulty));
        }

        if (query.Tags.Count > 0)
        {
            questions = questions.Where(question =>
                query.Tags.All(tag => question.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = query.Search.Trim();
            questions = questions.Where(question =>
                question.Statement.Contains(term, StringComparison.OrdinalIgnoreCase)
                || (question.Explanation?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
                || (question.Source?.Reference.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
                || question.Options.Any(option =>
                    option.Text.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        return questions;
    }
}
