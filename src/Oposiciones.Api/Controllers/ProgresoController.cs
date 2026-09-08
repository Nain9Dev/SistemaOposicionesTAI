using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oposiciones.Api.Extensions;
using Oposiciones.Application.DTOs;
using Oposiciones.Application.Interfaces;

namespace Oposiciones.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProgresoController : ControllerBase
{
    private readonly IProgresoService _progresoService;

    public ProgresoController(IProgresoService progresoService)
    {
        _progresoService = progresoService;
    }

    /// <summary>
    /// Archiva un simulacro. El cuerpo es un DTO y no la entidad de dominio: antes el cliente
    /// podia fijar Id y declarar su propia nota, que se guardaba tal cual.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(IntentoResultadoDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> GuardarIntento([FromBody] IntentoDto intento, CancellationToken cancellationToken)
    {
        if (User.GetUserId() is not { } userId)
        {
            return Unauthorized();
        }

        var resultado = await _progresoService.GuardarIntentoAsync(userId, intento, cancellationToken);

        return CreatedAtAction(nameof(GetHistorial), new { page = 1 }, resultado);
    }

    [HttpGet("historial")]
    [ProducesResponseType(typeof(PagedResult<IntentoResultadoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistorial(
        [FromQuery] int page = PagingDefaults.DefaultPage,
        [FromQuery] int pageSize = PagingDefaults.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        if (User.GetUserId() is not { } userId)
        {
            return Unauthorized();
        }

        // La saturacion de pagina y tamano ocurre en el servicio: page=0 generaba un OFFSET
        // negativo (error de SQL) y pageSize sin tope permitia volcar la tabla entera.
        return Ok(await _progresoService.GetHistorialAsync(userId, page, pageSize, cancellationToken));
    }

    [HttpGet("estadisticas")]
    [ProducesResponseType(typeof(EstadisticasDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEstadisticas(CancellationToken cancellationToken)
    {
        if (User.GetUserId() is not { } userId)
        {
            return Unauthorized();
        }

        return Ok(await _progresoService.GetEstadisticasAsync(userId, cancellationToken));
    }

    /// <summary>Borra el historial del usuario autenticado.</summary>
    [HttpDelete("historial")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> BorrarHistorial(CancellationToken cancellationToken)
    {
        if (User.GetUserId() is not { } userId)
        {
            return Unauthorized();
        }

        var eliminados = await _progresoService.BorrarHistorialAsync(userId, cancellationToken);

        return Ok(new { eliminados });
    }
}
