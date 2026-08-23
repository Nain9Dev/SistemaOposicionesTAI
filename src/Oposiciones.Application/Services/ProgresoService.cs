using System.Linq;
using System.Threading.Tasks;
using Oposiciones.Application.DTOs;
using Oposiciones.Application.Interfaces;
using Oposiciones.Domain.Interfaces;

namespace Oposiciones.Application.Services;

public class ProgresoService : IProgresoService
{
    private readonly IProgresoRepository _progresoRepository;

    public ProgresoService(IProgresoRepository progresoRepository)
    {
        _progresoRepository = progresoRepository;
    }

    public async Task<EstadisticasDto> GetEstadisticasAsync(int userId)
    {
        var historial = (await _progresoRepository.GetHistorialAsync(userId)).ToList();

        if (!historial.Any())
        {
            return new EstadisticasDto();
        }

        return new EstadisticasDto
        {
            TotalPreguntas = historial.Sum(h => h.Total),
            Aciertos = historial.Sum(h => h.Aciertos),
            Fallos = historial.Sum(h => h.Fallos),
            NotaMedia = historial.Average(h => h.Nota),
            ProgresoPorBloque = historial.GroupBy(h => h.Bloque)
                                         .ToDictionary(g => g.Key, g => g.Average(h => ((double)h.Aciertos / h.Total) * 100))
        };
    }
}
