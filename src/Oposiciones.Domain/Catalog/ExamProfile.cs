using Oposiciones.Domain.Scoring;

namespace Oposiciones.Domain.Catalog;

/// <summary>
/// Convocatoria completa (TAI, GSI, AGE...). Todo el catalogo cuelga de aqui, asi que
/// anadir otra oposicion es anadir otro <see cref="ExamProfile"/>, no tocar el codigo.
/// </summary>
public sealed class ExamProfile
{
    public int Id { get; init; }

    /// <summary>Codigo corto e inmutable de la convocatoria, por ejemplo <c>TAI</c>.</summary>
    public required string Code { get; init; }

    public required string Name { get; init; }

    /// <summary>Organo convocante (INAP, ministerio, comunidad autonoma...).</summary>
    public string Authority { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    /// <summary>Norma oficial que aprueba el programa y las bases de la convocatoria.</summary>
    public OfficialSource? OfficialSource { get; init; }

    /// <summary>Baremo de correccion: puntos por acierto, fallo y respuesta en blanco.</summary>
    public ScoringPolicy Scoring { get; init; } = ScoringPolicy.Default;

    /// <summary>Formato del ejercicio oficial (numero de preguntas, reservas y duracion).</summary>
    public ExamFormat Format { get; init; } = ExamFormat.Default;

    public bool IsActive { get; init; } = true;

    public IReadOnlyList<SyllabusBlock> Blocks { get; init; } = Array.Empty<SyllabusBlock>();

    /// <summary>Recorre el temario completo de la convocatoria en orden de bloque y tema.</summary>
    public IEnumerable<SyllabusTopic> AllTopics() => Blocks.SelectMany(block => block.Topics);
}

/// <summary>
/// Formato del ejercicio tal y como lo fija la convocatoria. Se guarda como dato para que un
/// cambio de bases (mas preguntas, otra duracion) sea una edicion de contenido, no un despliegue.
/// </summary>
public sealed record ExamFormat(
    int QuestionCount,
    int ReserveQuestions,
    int DurationMinutes,
    int OptionsPerQuestion)
{
    public static ExamFormat Default { get; } = new(80, 5, 120, 4);

    /// <summary>Preguntas que se presentan al opositor, incluidas las de reserva.</summary>
    public int TotalPresentedQuestions => QuestionCount + ReserveQuestions;
}

/// <summary>
/// Referencia a la fuente oficial de la que procede un contenido: norma, publicacion y enlace.
/// Es lo que permite que cada pregunta sea verificable contra el BOE o el estandar citado.
/// </summary>
public sealed record OfficialSource(string Reference, string? Publication = null, string? Url = null)
{
    public override string ToString() =>
        Publication is null ? Reference : $"{Reference} ({Publication})";
}
