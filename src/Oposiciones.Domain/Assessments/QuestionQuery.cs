using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Common;

namespace Oposiciones.Domain.Assessments;

/// <summary>
/// Filtro de consulta del banco de preguntas. Un unico contrato para navegar el banco desde la
/// Api, desde la herramienta de importacion o desde los informes.
/// </summary>
public sealed record QuestionQuery
{
    public string? ExamCode { get; init; }

    public string? BlockCode { get; init; }

    public int? TopicNumber { get; init; }

    public int? TopicId { get; init; }

    public IReadOnlyList<Difficulty> Difficulties { get; init; } = Array.Empty<Difficulty>();

    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    /// <summary>Busqueda libre sobre enunciado, explicacion y referencia normativa.</summary>
    public string? Search { get; init; }

    /// <summary>Filtra por estado de publicacion. Nulo devuelve activas e inactivas.</summary>
    public bool? IsActive { get; init; } = true;

    public Paging Paging { get; init; } = Paging.Default;
}

/// <summary>
/// Peticion de extraccion aleatoria y reproducible de preguntas para una seccion del examen.
/// </summary>
public sealed record QuestionDraw
{
    public required string ExamCode { get; init; }

    public string? BlockCode { get; init; }

    public int? TopicNumber { get; init; }

    public int? TopicId { get; init; }

    public IReadOnlyList<Difficulty> Difficulties { get; init; } = Array.Empty<Difficulty>();

    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    public int Count { get; init; }

    /// <summary>Semilla que hace reproducible la extraccion.</summary>
    public int Seed { get; init; }

    public IReadOnlyCollection<long> ExcludeQuestionIds { get; init; } = Array.Empty<long>();
}
