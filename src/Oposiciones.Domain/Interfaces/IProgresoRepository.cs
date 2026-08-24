using Oposiciones.Domain.Entities;

namespace Oposiciones.Domain.Interfaces;

public interface IProgresoRepository
{
    Task<IEnumerable<IntentoUsuario>> GetHistorialAsync(int usuarioId);
    Task<int> AddIntentoAsync(IntentoUsuario intento);
    Task<EstadisticasResumen> GetEstadisticasResumidasAsync(int usuarioId);
}
