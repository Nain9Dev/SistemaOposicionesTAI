namespace Oposiciones.Application.Contracts;

/// <summary>Opcion de respuesta tal y como se envia al cliente (nunca revela si es correcta).</summary>
public sealed record OptionDto(long Id, byte SortOrder, string Text);

/// <summary>
/// Pregunta enviada al cliente. <see cref="CorrectOptionId"/>, <see cref="Explanation"/> y
/// <see cref="Source"/> solo se rellenan cuando esta permitido ver la solucion: en modo estudio
/// o una vez cerrado el intento. En modo examen viajan en nulo.
/// </summary>
public sealed record QuestionDto(
    long Id,
    string ExternalId,
    string ExamCode,
    string BlockCode,
    int TopicNumber,
    string TopicTitle,
    int Difficulty,
    string Statement,
    IReadOnlyList<OptionDto> Options,
    long? CorrectOptionId,
    string? Explanation,
    OfficialSourceDto? Source,
    IReadOnlyList<string> Tags);

/// <summary>Test generado y listo para responder.</summary>
public sealed record TestDto(
    long TestId,
    string ExamCode,
    string Title,
    string Mode,
    int Seed,
    int DurationMinutes,
    int TotalQuestions,
    ScoringDto Scoring,
    DateTimeOffset CreatedAt,
    IReadOnlyList<QuestionDto> Questions);

/// <summary>
/// Peticion de generacion de un test. Todo es opcional salvo la convocatoria: sin mas datos se
/// genera un simulacro repartido segun el peso oficial de cada bloque.
/// </summary>
public sealed record GenerateTestRequest
{
    /// <summary>Codigo de convocatoria, por ejemplo <c>TAI</c>.</summary>
    public string ExamCode { get; init; } = "TAI";

    public string? Title { get; init; }

    /// <summary><c>study</c> (correccion inmediata) o <c>exam</c> (simulacro cronometrado).</summary>
    public string Mode { get; init; } = "study";

    public int TotalQuestions { get; init; } = 20;

    /// <summary>Reparto explicito por bloque o tema. Vacio reparte por peso oficial.</summary>
    public IReadOnlyList<SectionRequest> Sections { get; init; } = Array.Empty<SectionRequest>();

    /// <summary>Dificultades admitidas (1 a 5). Vacio admite todas.</summary>
    public IReadOnlyList<int> Difficulties { get; init; } = Array.Empty<int>();

    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    /// <summary>Semilla para reproducir exactamente el mismo test.</summary>
    public int? Seed { get; init; }

    public int? DurationMinutes { get; init; }

    public bool ShuffleOptions { get; init; } = true;

    public bool ShuffleQuestions { get; init; } = true;

    /// <summary>Preguntas a excluir, util para no repetir las del ultimo test.</summary>
    public IReadOnlyList<long> ExcludeQuestionIds { get; init; } = Array.Empty<long>();

    /// <summary>
    /// Permite generar el test aunque el banco todavia no tenga preguntas suficientes para el total
    /// pedido. Es el comportamiento por defecto porque el banco se rellena de forma progresiva;
    /// poniendolo en falso el sistema rechaza el test en lugar de devolverlo incompleto.
    /// </summary>
    public bool AllowPartial { get; init; } = true;
}

/// <summary>Porcion del reparto de un test.</summary>
public sealed record SectionRequest
{
    public string? BlockCode { get; init; }

    public int? TopicNumber { get; init; }

    public int? TopicId { get; init; }

    /// <summary>Numero exacto de preguntas de esta seccion.</summary>
    public int? QuestionCount { get; init; }

    /// <summary>Peso relativo de la seccion cuando no se fija un conteo exacto.</summary>
    public decimal? WeightPercent { get; init; }

    public IReadOnlyList<int> Difficulties { get; init; } = Array.Empty<int>();

    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();
}

/// <summary>Filtros de busqueda sobre el banco de preguntas.</summary>
public sealed record QuestionSearchRequest
{
    public string? ExamCode { get; init; }

    public string? BlockCode { get; init; }

    public int? TopicNumber { get; init; }

    public int? TopicId { get; init; }

    public IReadOnlyList<int> Difficulties { get; init; } = Array.Empty<int>();

    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    /// <summary>Texto libre buscado en enunciado, explicacion y referencia normativa.</summary>
    public string? Search { get; init; }

    public bool IncludeSolutions { get; init; }

    public int? Page { get; init; }

    public int? PageSize { get; init; }
}

/// <summary>Envoltorio estandar de las respuestas paginadas de la Api.</summary>
public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    long TotalItems,
    int TotalPages,
    bool HasNext);
