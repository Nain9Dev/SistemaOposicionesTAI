using System.Text.Json.Serialization;

namespace Oposiciones.Infrastructure.Content;

/// <summary>
/// Modelos de los ficheros JSON de contenido. Se mantienen separados de las entidades del dominio
/// a proposito: el formato de autoria puede evolucionar (anadir campos, renombrar) sin arrastrar
/// al modelo interno, y al reves.
/// </summary>
public sealed class ExamFile
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Authority { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public SourceFile? Source { get; set; }

    public ScoringFile? Scoring { get; set; }

    public FormatFile? Format { get; set; }

    public List<BlockFile> Blocks { get; set; } = new();
}

/// <summary>Referencia oficial de un contenido.</summary>
public sealed class SourceFile
{
    public string Reference { get; set; } = string.Empty;

    public string? Publication { get; set; }

    public string? Url { get; set; }
}

/// <summary>Baremo declarado en el fichero de convocatoria.</summary>
public sealed class ScoringFile
{
    public decimal CorrectPoints { get; set; } = 1m;

    public decimal IncorrectPoints { get; set; } = -1m / 3m;

    public decimal BlankPoints { get; set; }

    public decimal MaxScore { get; set; } = 50m;

    public decimal PassMark { get; set; } = 25m;
}

/// <summary>Formato del ejercicio declarado en el fichero de convocatoria.</summary>
public sealed class FormatFile
{
    public int QuestionCount { get; set; } = 80;

    public int ReserveQuestions { get; set; } = 5;

    public int DurationMinutes { get; set; } = 120;

    public int OptionsPerQuestion { get; set; } = 4;
}

/// <summary>Bloque del temario.</summary>
public sealed class BlockFile
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public decimal ExamWeightPercent { get; set; }

    public List<TopicFile> Topics { get; set; } = new();
}

/// <summary>Tema del temario.</summary>
public sealed class TopicFile
{
    public int Number { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Slug { get; set; }

    public List<string> Keywords { get; set; } = new();
}

/// <summary>Fichero del banco de preguntas.</summary>
public sealed class QuestionFile
{
    public string ExamCode { get; set; } = string.Empty;

    public List<QuestionEntry> Questions { get; set; } = new();
}

/// <summary>
/// Pregunta en formato de autoria. Se declara la respuesta correcta por indice
/// (<see cref="CorrectIndex"/>, base 0) porque es lo que menos errores induce al escribir a mano.
/// </summary>
public sealed class QuestionEntry
{
    /// <summary>Identificador estable, por ejemplo <c>TAI-B1-T1-001</c>.</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    public string BlockCode { get; set; } = string.Empty;

    public int TopicNumber { get; set; }

    public int Difficulty { get; set; } = 3;

    public string Statement { get; set; } = string.Empty;

    public List<string> Options { get; set; } = new();

    public int CorrectIndex { get; set; } = -1;

    public string? Explanation { get; set; }

    public SourceFile? Source { get; set; }

    public List<string> Tags { get; set; } = new();

    public bool IsActive { get; set; } = true;
}
