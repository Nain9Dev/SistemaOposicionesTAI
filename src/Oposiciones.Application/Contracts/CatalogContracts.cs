namespace Oposiciones.Application.Contracts;

/// <summary>Referencia normativa u oficial de un contenido.</summary>
public sealed record OfficialSourceDto(string Reference, string? Publication, string? Url);

/// <summary>Baremo de correccion publicado, para que el cliente pueda explicar la nota.</summary>
public sealed record ScoringDto(
    decimal CorrectPoints,
    decimal IncorrectPoints,
    decimal BlankPoints,
    decimal MaxScore,
    decimal PassMark);

/// <summary>Formato del ejercicio oficial.</summary>
public sealed record ExamFormatDto(
    int QuestionCount,
    int ReserveQuestions,
    int DurationMinutes,
    int OptionsPerQuestion);

/// <summary>Convocatoria en formato resumido, para el selector de oposicion.</summary>
public sealed record ExamSummaryDto(
    string Code,
    string Name,
    string Authority,
    string? Description,
    OfficialSourceDto? Source,
    ScoringDto Scoring,
    ExamFormatDto Format,
    int BlockCount,
    int TopicCount);

/// <summary>Convocatoria con su temario completo.</summary>
public sealed record ExamDetailDto(
    string Code,
    string Name,
    string Authority,
    string? Description,
    OfficialSourceDto? Source,
    ScoringDto Scoring,
    ExamFormatDto Format,
    IReadOnlyList<BlockDto> Blocks);

/// <summary>Bloque del temario con sus temas.</summary>
public sealed record BlockDto(
    int Id,
    string Code,
    string Name,
    int DisplayOrder,
    decimal ExamWeightPercent,
    IReadOnlyList<TopicDto> Topics);

/// <summary>Tema del programa oficial.</summary>
public sealed record TopicDto(
    int Id,
    string ExamCode,
    string BlockCode,
    int Number,
    string Title,
    string Slug,
    IReadOnlyList<string> Keywords);

/// <summary>Cobertura del banco de preguntas por tema: senala que temas quedan por rellenar.</summary>
public sealed record TopicCoverageDto(
    int TopicId,
    string BlockCode,
    string BlockName,
    int TopicNumber,
    string TopicTitle,
    int QuestionCount,
    int ActiveQuestionCount,
    decimal AverageDifficulty);

/// <summary>Resumen de cobertura de una convocatoria completa.</summary>
public sealed record BankCoverageDto(
    string ExamCode,
    int TotalQuestions,
    int ActiveQuestions,
    int TopicsWithQuestions,
    int TopicsWithoutQuestions,
    IReadOnlyList<TopicCoverageDto> Topics);
