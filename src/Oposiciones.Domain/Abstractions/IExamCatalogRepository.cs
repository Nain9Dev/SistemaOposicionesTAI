using Oposiciones.Domain.Catalog;

namespace Oposiciones.Domain.Abstractions;

/// <summary>
/// Lectura del catalogo: convocatorias, bloques y temas. Se implementa una vez por proveedor de
/// persistencia (SQL Server y en memoria), sin que la capa de aplicacion note cual esta activo.
/// </summary>
public interface IExamCatalogRepository
{
    Task<IReadOnlyList<ExamProfile>> GetExamsAsync(CancellationToken cancellationToken = default);

    /// <summary>Convocatoria con su temario completo, o nulo si el codigo no existe.</summary>
    Task<ExamProfile?> GetExamAsync(string examCode, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SyllabusBlock>> GetBlocksAsync(string examCode, CancellationToken cancellationToken = default);

    /// <summary>Temas de una convocatoria, opcionalmente acotados a un bloque.</summary>
    Task<IReadOnlyList<SyllabusTopic>> GetTopicsAsync(
        string examCode,
        string? blockCode = null,
        CancellationToken cancellationToken = default);
}
