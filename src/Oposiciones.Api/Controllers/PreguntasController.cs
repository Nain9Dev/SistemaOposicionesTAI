using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Oposiciones.Api.DTOs;
using Oposiciones.Api.Security;
using Oposiciones.Application.Common;
using Oposiciones.Application.DTOs;
using Oposiciones.Application.Interfaces;
using Oposiciones.Application.Services;

namespace Oposiciones.Api.Controllers;

/// <summary>
/// Banco de preguntas del motor de estudio.
///
/// El cliente ya invocaba /api/preguntas, pero no existia ningun controlador que lo sirviera:
/// todas las peticiones fallaban y la aplicacion caia siempre al catalogo estatico. Esta es
/// la pieza que conecta el simulacro con la base de datos.
///
/// El acceso es anonimo a proposito: el modo invitado es una funcionalidad del producto y el
/// mismo catalogo se publica como fichero estatico. Queda acotado por limitador de peticiones.
/// </summary>
[ApiController]
[Route("api/preguntas")]
[EnableRateLimiting(RateLimitPolicies.Public)]
[Produces("application/json")]
public class PreguntasController : ControllerBase
{
    private readonly IStudyQuestionService _studyQuestionService;

    public PreguntasController(IStudyQuestionService studyQuestionService)
    {
        _studyQuestionService = studyQuestionService;
    }

    /// <summary>Cuestionario aleatorio de todo el temario o del bloque indicado.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PreguntaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPreguntas([FromQuery] PreguntasQueryDto query, CancellationToken cancellationToken)
    {
        var preguntas = await _studyQuestionService.GetPreguntasAsync(
            query.Bloque,
            query.Cantidad,
            query.Dificultad,
            query.TemaId,
            cancellationToken);

        return Ok(preguntas);
    }

    /// <summary>
    /// Cuestionario restringido a un bloque. Se mantiene como ruta propia porque es la que
    /// consume el cliente desplegado; delega en la misma logica que el listado general.
    /// </summary>
    [HttpGet("bloque/{bloque}")]
    [ProducesResponseType(typeof(IReadOnlyList<PreguntaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPreguntasPorBloque(
        string bloque,
        [FromQuery] int cantidad = StudyQuestionService.DefaultQuestionCount,
        [FromQuery] int? dificultad = null,
        [FromQuery] int? temaId = null,
        CancellationToken cancellationToken = default)
    {
        var preguntas = await _studyQuestionService.GetPreguntasAsync(
            bloque, cantidad, dificultad, temaId, cancellationToken);

        return Ok(preguntas);
    }

    /// <summary>
    /// Preguntas disponibles para el filtro. Permite avisar al usuario antes de empezar en
    /// lugar de generarle un simulacro mas corto de lo que ha pedido sin explicacion.
    /// </summary>
    [HttpGet("disponibilidad")]
    [ProducesResponseType(typeof(DisponibilidadDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDisponibilidad(
        [FromQuery] string? bloque,
        [FromQuery] int? dificultad,
        [FromQuery] int? temaId,
        CancellationToken cancellationToken)
    {
        var disponibles = await _studyQuestionService.ContarDisponiblesAsync(
            bloque, dificultad, temaId, cancellationToken);

        return Ok(new DisponibilidadDto
        {
            Bloque = BlockSelector.Normalize(bloque),
            Disponibles = disponibles,
            MaximoPorSimulacro = StudyQuestionService.MaxQuestionCount
        });
    }
}
