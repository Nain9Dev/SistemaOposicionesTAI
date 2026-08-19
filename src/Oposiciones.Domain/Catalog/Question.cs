using Oposiciones.Domain.Common;

namespace Oposiciones.Domain.Catalog;

/// <summary>Dificultad declarada de una pregunta, de 1 (basica) a 5 (muy dificil).</summary>
public enum Difficulty
{
    Basic = 1,
    Easy = 2,
    Medium = 3,
    Hard = 4,
    Expert = 5,
}

/// <summary>
/// Pregunta tipo test con sus opciones, su explicacion y la fuente oficial que la respalda.
/// La fuente no es decorativa: es el criterio con el que se revisa y se defiende cada respuesta.
/// </summary>
public sealed class Question
{
    public long Id { get; init; }

    /// <summary>
    /// Clave estable definida en el fichero de contenido (por ejemplo <c>TAI-B1-T1-001</c>).
    /// Permite reimportar el banco sin duplicar preguntas ni romper referencias.
    /// </summary>
    public required string ExternalId { get; init; }

    public int TopicId { get; init; }

    public string ExamCode { get; init; } = string.Empty;

    public string BlockCode { get; init; } = string.Empty;

    public int TopicNumber { get; init; }

    public string TopicTitle { get; init; } = string.Empty;

    public Difficulty Difficulty { get; init; } = Difficulty.Medium;

    public required string Statement { get; init; }

    /// <summary>Justificacion de la respuesta correcta, mostrada en modo estudio.</summary>
    public string? Explanation { get; init; }

    public OfficialSource? Source { get; init; }

    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    public bool IsActive { get; init; } = true;

    public IReadOnlyList<AnswerOption> Options { get; init; } = Array.Empty<AnswerOption>();

    public AnswerOption? CorrectOption => Options.FirstOrDefault(option => option.IsCorrect);

    /// <summary>
    /// Devuelve una copia con las opciones barajadas de forma determinista y renumeradas.
    /// Evita que el opositor memorice la posicion de la respuesta en lugar del contenido.
    /// </summary>
    public Question WithShuffledOptions(IShuffler shuffler)
    {
        ArgumentNullException.ThrowIfNull(shuffler);
        if (Options.Count <= 1)
        {
            return this;
        }

        List<AnswerOption> shuffled = shuffler.Shuffled(Options);
        var renumbered = new List<AnswerOption>(shuffled.Count);
        for (int i = 0; i < shuffled.Count; i++)
        {
            renumbered.Add(shuffled[i] with { SortOrder = (byte)(i + 1) });
        }

        return Clone(renumbered);
    }

    private Question Clone(IReadOnlyList<AnswerOption> options) => new()
    {
        Id = Id,
        ExternalId = ExternalId,
        TopicId = TopicId,
        ExamCode = ExamCode,
        BlockCode = BlockCode,
        TopicNumber = TopicNumber,
        TopicTitle = TopicTitle,
        Difficulty = Difficulty,
        Statement = Statement,
        Explanation = Explanation,
        Source = Source,
        Tags = Tags,
        IsActive = IsActive,
        Options = options,
    };
}

/// <summary>Opcion de respuesta. <see cref="IsCorrect"/> nunca viaja al cliente en modo examen.</summary>
public sealed record AnswerOption(long Id, byte SortOrder, string Text, bool IsCorrect);
