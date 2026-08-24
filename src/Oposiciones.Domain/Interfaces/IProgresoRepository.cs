using Oposiciones.Domain.Entities;

namespace Oposiciones.Domain.Interfaces;

public interface IProgresoRepository
{
    Task<(IEnumerable<IntentoUsuario> Items, int TotalCount)> GetHistorialAsync(int usuarioId, int page, int pageSize);
    Task<int> AddIntentoAsync(IntentoUsuario intento);
    Task<EstadisticasResumen> GetEstadisticasResumidasAsync(int usuarioId);
}
