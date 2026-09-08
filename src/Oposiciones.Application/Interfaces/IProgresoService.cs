using Oposiciones.Application.DTOs;

namespace Oposiciones.Application.Interfaces;

public interface IProgresoService
{
    Task<EstadisticasDto> GetEstadisticasAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persiste un simulacro. La nota y los blancos los recalcula el servidor a partir de
    /// aciertos, fallos y total: el cliente no puede declarar su propia calificacion.
    /// </summary>
    Task<IntentoResultadoDto> GuardarIntentoAsync(int userId, IntentoDto intento, CancellationToken cancellationToken = default);

    Task<PagedResult<IntentoResultadoDto>> GetHistorialAsync(
        int userId, int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>Borra el historial del usuario. Devuelve el numero de intentos eliminados.</summary>
    Task<int> BorrarHistorialAsync(int userId, CancellationToken cancellationToken = default);
}
