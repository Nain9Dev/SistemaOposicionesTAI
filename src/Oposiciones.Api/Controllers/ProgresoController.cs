using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oposiciones.Domain.Entities;
using Oposiciones.Domain.Interfaces;
using Oposiciones.Application.Interfaces;
using System.Security.Claims;
using System;
using System.Threading.Tasks;

namespace Oposiciones.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProgresoController : ControllerBase
{
    private readonly IProgresoRepository _progresoRepository;
    private readonly IProgresoService _progresoService;

    public ProgresoController(IProgresoRepository progresoRepository, IProgresoService progresoService)
    {
        _progresoRepository = progresoRepository;
        _progresoService = progresoService;
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
        if (intento.Fecha == default)
        {
            intento.Fecha = DateTime.UtcNow;
        }

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

        var stats = await _progresoService.GetEstadisticasAsync(userId);
        return Ok(stats);
    }
}
