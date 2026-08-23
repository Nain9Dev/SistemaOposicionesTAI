using System.Threading.Tasks;
using Oposiciones.Application.DTOs;

namespace Oposiciones.Application.Interfaces;

public interface IProgresoService
{
    Task<EstadisticasDto> GetEstadisticasAsync(int userId);
}
