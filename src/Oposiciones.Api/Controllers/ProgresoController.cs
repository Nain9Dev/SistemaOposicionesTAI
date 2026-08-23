using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oposiciones.Domain.Entities;
using Oposiciones.Domain.Interfaces;
using System.Security.Claims;

namespace Oposiciones.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProgresoController : ControllerBase
{
    private readonly IProgresoRepository _progresoRepository;

    public ProgresoController(IProgresoRepository progresoRepository)
    {
        _progresoRepository = progresoRepository;
    }

    private int GetCurrentUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(idClaim, out var id) ? id : 0;
    }

    [HttpPost]
    public async Task<IActionResult> GuardarIntento([FromBody] IntentoUsuario intento)
    {
        int userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized();

        intento.UsuarioId = userId;
        intento.Fecha = DateTime.UtcNow;

        var id = await _progresoRepository.AddIntentoAsync(intento);
        intento.Id = id;

        return Ok(intento);
    }

    [HttpGet("historial")]
    public async Task<IActionResult> GetHistorial()
    {
        int userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized();

        var historial = await _progresoRepository.GetHistorialAsync(userId);
        return Ok(historial);
    }

    [HttpGet("estadisticas")]
    public async Task<IActionResult> GetEstadisticas()
    {
        int userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized();

        var historial = (await _progresoRepository.GetHistorialAsync(userId)).ToList();

        if (!historial.Any())
        {
            return Ok(new
            {
                TotalPreguntas = 0,
                Aciertos = 0,
                Fallos = 0,
                NotaMedia = 0.0,
                ProgresoPorBloque = new Dictionary<string, double>()
            });
        }

        var stats = new
        {
            TotalPreguntas = historial.Sum(h => h.Total),
            Aciertos = historial.Sum(h => h.Aciertos),
            Fallos = historial.Sum(h => h.Fallos),
            NotaMedia = historial.Average(h => h.Nota),
            ProgresoPorBloque = historial.GroupBy(h => h.Bloque)
                                         .ToDictionary(g => g.Key, g => g.Average(h => ((double)h.Aciertos / h.Total) * 100))
        };

        return Ok(stats);
    }
}
