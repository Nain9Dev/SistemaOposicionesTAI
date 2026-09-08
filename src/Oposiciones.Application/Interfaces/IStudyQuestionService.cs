using Oposiciones.Application.DTOs;

namespace Oposiciones.Application.Interfaces;

public interface IStudyQuestionService
{
    /// <summary>
    /// Devuelve preguntas listas para el simulacro. El selector de bloque admite "all",
    /// el indice numerico o el codigo romano del temario.
    /// </summary>
    Task<IReadOnlyList<PreguntaDto>> GetPreguntasAsync(
        string? bloqueSelector,
        int cantidad,
        int? dificultad = null,
        int? temaId = null,
        CancellationToken cancellationToken = default);

    /// <summary>Preguntas disponibles para el filtro, util para avisar antes de generar el test.</summary>
    Task<int> ContarDisponiblesAsync(
        string? bloqueSelector,
        int? dificultad = null,
        int? temaId = null,
        CancellationToken cancellationToken = default);
}
