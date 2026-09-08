using Oposiciones.Domain.Entities;

namespace Oposiciones.Domain.Interfaces;

public interface IProgresoRepository
{
    Task<(IReadOnlyList<IntentoUsuario> Items, int TotalCount)> GetHistorialAsync(
        int usuarioId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<int> AddIntentoAsync(IntentoUsuario intento, CancellationToken cancellationToken = default);

    Task<EstadisticasResumen> GetEstadisticasResumidasAsync(int usuarioId, CancellationToken cancellationToken = default);

    /// <summary>Ultimas notas ordenadas de mas antigua a mas reciente, para dibujar la tendencia.</summary>
    Task<IReadOnlyList<IntentoUsuario>> GetUltimosIntentosAsync(
        int usuarioId, int limit, CancellationToken cancellationToken = default);

    Task<int> DeleteHistorialAsync(int usuarioId, CancellationToken cancellationToken = default);
}
