using Oposiciones.Domain.Assessments;
using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Common;

namespace Oposiciones.Domain.Abstractions;

/// <summary>Acceso al banco de preguntas.</summary>
public interface IQuestionRepository
{
    Task<PagedResult<Question>> SearchAsync(QuestionQuery query, CancellationToken cancellationToken = default);

    Task<Question?> GetAsync(long questionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extraccion aleatoria reproducible: la misma semilla y los mismos filtros devuelven siempre
    /// el mismo conjunto mientras el banco no cambie.
    /// </summary>
    Task<IReadOnlyList<Question>> DrawAsync(QuestionDraw draw, CancellationToken cancellationToken = default);

    /// <summary>Preguntas disponibles que cumplen los filtros, para avisar antes de generar un test.</summary>
    Task<int> CountAvailableAsync(QuestionDraw draw, CancellationToken cancellationToken = default);

    /// <summary>Cobertura del banco por tema, util para saber que temas faltan por rellenar.</summary>
    Task<IReadOnlyList<TopicCoverage>> GetCoverageAsync(
        string examCode,
        CancellationToken cancellationToken = default);
}

/// <summary>Cuantas preguntas hay cargadas en cada tema y con que reparto de dificultad.</summary>
public sealed record TopicCoverage(
    int TopicId,
    string BlockCode,
    string BlockName,
    int TopicNumber,
    string TopicTitle,
    int QuestionCount,
    int ActiveQuestionCount,
    decimal AverageDifficulty);
