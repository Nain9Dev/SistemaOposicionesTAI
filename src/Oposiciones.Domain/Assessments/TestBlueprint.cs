using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Scoring;

namespace Oposiciones.Domain.Assessments;

/// <summary>Modo de ejecucion del test, que decide que informacion ve el opositor mientras responde.</summary>
public enum TestMode
{
    /// <summary>Modo estudio: correccion inmediata y explicacion visible tras cada respuesta.</summary>
    Study = 0,

    /// <summary>Modo examen: sin correccion hasta el final y con temporizador.</summary>
    Exam = 1,
}

/// <summary>
/// Receta declarativa de un test. Sustituye a la generacion rigida de "un tema y una dificultad":
/// aqui se describe que se quiere y el planificador resuelve como repartir las preguntas.
/// <para>
/// Con secciones vacias se reparte por el peso de cada bloque en la convocatoria; con secciones
/// se puede fijar el reparto exacto (por conteo o por porcentaje) hasta el nivel de tema.
/// </para>
/// </summary>
public sealed record TestBlueprint
{
    /// <summary>Convocatoria sobre la que se genera el test.</summary>
    public required string ExamCode { get; init; }

    public string? Title { get; init; }

    public TestMode Mode { get; init; } = TestMode.Study;

    public int TotalQuestions { get; init; } = 20;

    /// <summary>Reparto explicito. Si esta vacio se usa el peso oficial de cada bloque.</summary>
    public IReadOnlyList<BlueprintSection> Sections { get; init; } = Array.Empty<BlueprintSection>();

    /// <summary>Dificultades admitidas globalmente. Vacio significa cualquier dificultad.</summary>
    public IReadOnlyList<Difficulty> Difficulties { get; init; } = Array.Empty<Difficulty>();

    /// <summary>Etiquetas tematicas exigidas globalmente (por ejemplo <c>ens</c> o <c>tcp-ip</c>).</summary>
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    /// <summary>Baraja el orden de las opciones dentro de cada pregunta.</summary>
    public bool ShuffleOptions { get; init; } = true;

    /// <summary>Baraja el orden de las preguntas entre secciones.</summary>
    public bool ShuffleQuestions { get; init; } = true;

    /// <summary>
    /// Semilla del generador. Fijarla reproduce exactamente el mismo examen, lo que permite
    /// compartirlo, repetirlo o depurarlo. Si es nula se genera una nueva.
    /// </summary>
    public int? Seed { get; init; }

    /// <summary>Duracion en minutos. Si es nula se toma la del formato oficial de la convocatoria.</summary>
    public int? DurationMinutes { get; init; }

    /// <summary>Baremo alternativo. Si es nulo se usa el de la convocatoria.</summary>
    public ScoringPolicy? ScoringOverride { get; init; }

    /// <summary>Preguntas que no deben repetirse (por ejemplo, las ya vistas hoy).</summary>
    public IReadOnlyCollection<long> ExcludeQuestionIds { get; init; } = Array.Empty<long>();
}

/// <summary>
/// Porcion de un <see cref="TestBlueprint"/>. Cada seccion acota el origen de sus preguntas y
/// cuantas aporta, bien por conteo exacto (<see cref="QuestionCount"/>) o por porcentaje
/// (<see cref="WeightPercent"/>) sobre el total del test.
/// </summary>
public sealed record BlueprintSection
{
    public string? BlockCode { get; init; }

    public int? TopicNumber { get; init; }

    public int? TopicId { get; init; }

    public int? QuestionCount { get; init; }

    public decimal? WeightPercent { get; init; }

    public IReadOnlyList<Difficulty> Difficulties { get; init; } = Array.Empty<Difficulty>();

    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    /// <summary>Etiqueta legible de la seccion, util al mostrar el reparto resultante.</summary>
    public string Describe()
    {
        if (TopicId is not null)
        {
            return $"Tema #{TopicId}";
        }

        if (BlockCode is not null && TopicNumber is not null)
        {
            return $"Bloque {BlockCode} - Tema {TopicNumber}";
        }

        if (BlockCode is not null)
        {
            return $"Bloque {BlockCode}";
        }

        return TopicNumber is not null ? $"Tema {TopicNumber}" : "Temario completo";
    }
}
