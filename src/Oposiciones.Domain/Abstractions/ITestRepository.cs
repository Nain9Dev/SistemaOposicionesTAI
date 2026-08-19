using Oposiciones.Domain.Assessments;
using Oposiciones.Domain.Catalog;

namespace Oposiciones.Domain.Abstractions;

/// <summary>Persistencia de los tests generados.</summary>
public interface ITestRepository
{
    /// <summary>
    /// Guarda un test ya resuelto. Recibe las preguntas en su orden definitivo: la seleccion es
    /// responsabilidad del planificador del dominio, no del almacen.
    /// </summary>
    Task<GeneratedTest> CreateAsync(
        TestBlueprint blueprint,
        string title,
        int seed,
        int durationMinutes,
        IReadOnlyList<Question> questions,
        CancellationToken cancellationToken = default);

    Task<GeneratedTest?> GetAsync(long testId, CancellationToken cancellationToken = default);
}
