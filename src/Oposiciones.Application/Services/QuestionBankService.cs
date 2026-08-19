using Oposiciones.Application.Contracts;
using Oposiciones.Application.Mapping;
using Oposiciones.Domain.Abstractions;
using Oposiciones.Domain.Assessments;
using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Common;

namespace Oposiciones.Application.Services;

/// <summary>
/// Consulta y control del banco de preguntas. Incluye el informe de cobertura, que es la
/// herramienta con la que se decide que temas hay que seguir rellenando.
/// </summary>
public interface IQuestionBankService
{
    Task<PagedResponse<QuestionDto>> SearchAsync(
        QuestionSearchRequest request,
        CancellationToken cancellationToken = default);

    Task<QuestionDto> GetAsync(long questionId, bool includeSolution, CancellationToken cancellationToken = default);

    Task<BankCoverageDto> GetCoverageAsync(string examCode, CancellationToken cancellationToken = default);
}

/// <inheritdoc />
public sealed class QuestionBankService : IQuestionBankService
{
    private readonly IQuestionRepository _questions;
    private readonly IExamCatalogRepository _catalog;

    public QuestionBankService(IQuestionRepository questions, IExamCatalogRepository catalog)
    {
        _questions = questions;
        _catalog = catalog;
    }

    public async Task<PagedResponse<QuestionDto>> SearchAsync(
        QuestionSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var query = new QuestionQuery
        {
            ExamCode = Normalize(request.ExamCode),
            BlockCode = Normalize(request.BlockCode),
            TopicNumber = request.TopicNumber,
            TopicId = request.TopicId,
            Difficulties = ParseDifficulties(request.Difficulties, nameof(request.Difficulties)),
            Tags = NormalizeTags(request.Tags),
            Search = Normalize(request.Search),
            Paging = Paging.Of(request.Page, request.PageSize),
        };

        PagedResult<Question> result = await _questions.SearchAsync(query, cancellationToken).ConfigureAwait(false);
        return result.ToResponse(question => question.ToDto(request.IncludeSolutions));
    }

    public async Task<QuestionDto> GetAsync(
        long questionId,
        bool includeSolution,
        CancellationToken cancellationToken = default)
    {
        Question? question = await _questions.GetAsync(questionId, cancellationToken).ConfigureAwait(false);
        if (question is null)
        {
            throw new NotFoundException("la pregunta", questionId);
        }

        return question.ToDto(includeSolution);
    }

    public async Task<BankCoverageDto> GetCoverageAsync(
        string examCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(examCode))
        {
            throw new DomainException("Debe indicarse el codigo de la convocatoria.");
        }

        ExamProfile? exam = await _catalog.GetExamAsync(examCode, cancellationToken).ConfigureAwait(false);
        if (exam is null)
        {
            throw new NotFoundException("la convocatoria", examCode);
        }

        IReadOnlyList<TopicCoverage> coverage =
            await _questions.GetCoverageAsync(exam.Code, cancellationToken).ConfigureAwait(false);

        return new BankCoverageDto(
            exam.Code,
            coverage.Sum(topic => topic.QuestionCount),
            coverage.Sum(topic => topic.ActiveQuestionCount),
            coverage.Count(topic => topic.ActiveQuestionCount > 0),
            coverage.Count(topic => topic.ActiveQuestionCount == 0),
            coverage.Select(topic => topic.ToDto()).ToList());
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    internal static IReadOnlyList<string> NormalizeTags(IReadOnlyList<string> tags) =>
        tags.Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim().ToLowerInvariant())
            .Distinct(StringComparer.Ordinal)
            .ToList();

    /// <summary>
    /// Convierte dificultades numericas a la enumeracion del dominio, rechazando las fuera de rango
    /// en lugar de ignorarlas en silencio: un filtro mal escrito debe avisar, no devolver otra cosa.
    /// </summary>
    internal static IReadOnlyList<Difficulty> ParseDifficulties(IReadOnlyList<int> values, string field)
    {
        if (values.Count == 0)
        {
            return Array.Empty<Difficulty>();
        }

        var validation = new Common.ValidationBuilder();
        var parsed = new List<Difficulty>(values.Count);

        foreach (int value in values)
        {
            if (value is < 1 or > 5)
            {
                validation.Add(field, $"La dificultad '{value}' esta fuera del rango admitido (1 a 5).");
                continue;
            }

            var difficulty = (Difficulty)value;
            if (!parsed.Contains(difficulty))
            {
                parsed.Add(difficulty);
            }
        }

        validation.ThrowIfInvalid();
        return parsed;
    }
}
