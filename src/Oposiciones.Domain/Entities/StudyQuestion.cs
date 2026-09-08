namespace Oposiciones.Domain.Entities;

/// <summary>
/// Pregunta lista para el motor de estudio: enunciado, opciones ordenadas y contexto de temario.
/// Es un modelo de lectura; la persistencia vive repartida entre Questions y AnswerOptions.
/// </summary>
public sealed class StudyQuestion
{
    public long Id { get; init; }
    public string Statement { get; init; } = string.Empty;
    public string? Explanation { get; init; }
    public int Difficulty { get; init; }

    public int BlockId { get; init; }
    public string BlockCode { get; init; } = string.Empty;
    public string BlockName { get; init; } = string.Empty;

    public int TopicId { get; init; }
    public string TopicTitle { get; init; } = string.Empty;

    public IReadOnlyList<StudyQuestionOption> Options { get; init; } = [];

    /// <summary>
    /// Posicion (base 0) de la opcion correcta dentro de <see cref="Options"/>.
    /// Devuelve -1 si la pregunta no tiene solucion marcada, para que el llamante pueda descartarla.
    /// </summary>
    public int CorrectOptionIndex
    {
        get
        {
            for (var i = 0; i < Options.Count; i++)
            {
                if (Options[i].IsCorrect)
                {
                    return i;
                }
            }

            return -1;
        }
    }

    public bool IsAnswerable => Options.Count >= 2 && CorrectOptionIndex >= 0;
}

public sealed class StudyQuestionOption
{
    public long Id { get; init; }
    public short SortOrder { get; init; }
    public string Text { get; init; } = string.Empty;
    public bool IsCorrect { get; init; }
}

/// <summary>Fila plana devuelta por la consulta de Dapper antes de agrupar por pregunta.</summary>
public sealed class StudyQuestionRow
{
    public long QuestionId { get; init; }
    public string Statement { get; init; } = string.Empty;
    public string? Explanation { get; init; }
    public short Difficulty { get; init; }

    public int BlockId { get; init; }
    public string BlockCode { get; init; } = string.Empty;
    public string BlockName { get; init; } = string.Empty;

    public int TopicId { get; init; }
    public string TopicTitle { get; init; } = string.Empty;

    public long OptionId { get; init; }
    public short SortOrder { get; init; }
    public string OptionText { get; init; } = string.Empty;
    public bool IsCorrect { get; init; }
}
