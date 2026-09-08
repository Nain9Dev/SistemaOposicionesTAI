using Oposiciones.Domain.Entities;

namespace Oposiciones.Domain.Interfaces;

/// <summary>
/// Criterios de seleccion del motor de estudio. Un bloque nulo significa "todo el temario".
/// </summary>
/// <param name="BlockId">Identificador numerico del bloque, si el cliente lo conoce.</param>
/// <param name="BlockCode">Codigo romano del bloque (I..IV) como alternativa al identificador.</param>
/// <param name="TopicId">Restringe a un tema concreto dentro del bloque.</param>
/// <param name="Difficulty">Dificultad exacta (1..5); nulo acepta cualquiera.</param>
/// <param name="Limit">Numero maximo de preguntas a devolver.</param>
public readonly record struct StudyQuestionFilter(
    int? BlockId,
    string? BlockCode,
    int? TopicId,
    int? Difficulty,
    int Limit);

public interface IStudyQuestionRepository
{
    /// <summary>Devuelve preguntas activas en orden aleatorio, ya agrupadas con sus opciones.</summary>
    Task<IReadOnlyList<StudyQuestion>> GetRandomAsync(StudyQuestionFilter filter, CancellationToken cancellationToken = default);

    /// <summary>Numero de preguntas activas disponibles para el filtro indicado.</summary>
    Task<int> CountAsync(StudyQuestionFilter filter, CancellationToken cancellationToken = default);
}
