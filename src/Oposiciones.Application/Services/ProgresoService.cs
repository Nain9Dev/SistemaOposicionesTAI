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
        var resumen = await _progresoRepository.GetEstadisticasResumidasAsync(userId);

        return new EstadisticasDto
        {
            TotalPreguntas = resumen.TotalPreguntas,
            Aciertos = resumen.Aciertos,
            Fallos = resumen.Fallos,
            NotaMedia = resumen.NotaMedia,
            ProgresoPorBloque = resumen.ProgresoPorBloque
        };
    }
}
