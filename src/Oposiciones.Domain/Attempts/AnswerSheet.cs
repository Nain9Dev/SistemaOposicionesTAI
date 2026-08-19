using Oposiciones.Domain.Scoring;

namespace Oposiciones.Domain.Attempts;

/// <summary>
/// Hoja de respuestas de un intento: una fila por pregunta del test, contestada o no.
/// Es la entrada de la correccion, y contiene ya la clasificacion por bloque y tema junto con el
/// baremo con el que se genero el test, de modo que corregir es una sola lectura y no varias.
/// </summary>
public sealed record AnswerSheet(
    long AttemptId,
    long TestId,
    string ExamCode,
    ScoringPolicy Scoring,
    IReadOnlyList<AnswerSheetRow> Rows)
{
    public int TotalQuestions => Rows.Count;
}

/// <summary>Una pregunta del intento con la opcion marcada y cual era la correcta.</summary>
public sealed record AnswerSheetRow(
    long QuestionId,
    long? SelectedOptionId,
    long? CorrectOptionId,
    string BlockCode,
    string BlockName,
    int TopicNumber,
    string TopicTitle)
{
    public bool IsBlank => SelectedOptionId is null;

    public bool IsCorrect => SelectedOptionId is not null && SelectedOptionId == CorrectOptionId;

    public bool IsIncorrect => SelectedOptionId is not null && SelectedOptionId != CorrectOptionId;
}
