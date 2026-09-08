namespace Oposiciones.Application.DTOs;

/// <summary>
/// Resultado de aplicar el baremo oficial INAP a un simulacro.
/// </summary>
/// <param name="Total">Preguntas presentadas.</param>
/// <param name="Correct">Aciertos.</param>
/// <param name="Wrong">Fallos.</param>
/// <param name="Blank">Preguntas sin responder.</param>
/// <param name="NetPoints">Puntuacion neta antes de escalar a base 10.</param>
/// <param name="MaxPoints">Puntuacion maxima alcanzable.</param>
/// <param name="Grade">Nota final sobre 10, nunca negativa.</param>
public readonly record struct ExamScore(
    int Total,
    int Correct,
    int Wrong,
    int Blank,
    double NetPoints,
    double MaxPoints,
    double Grade);
