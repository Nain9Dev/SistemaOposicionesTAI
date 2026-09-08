using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oposiciones.Api.DTOs;
using Oposiciones.Api.Extensions;
using Oposiciones.Application.Interfaces;
using Oposiciones.Domain.Interfaces;

namespace Oposiciones.Api.Controllers;

/// <summary>
/// Ciclo de vida de un intento sobre un test generado.
///
/// Antes era completamente anonimo y aceptaba el propietario como texto en el cuerpo, de modo
/// que cualquiera podia responder o cerrar el intento de otra persona. Ahora el propietario se
/// deriva del token y el servicio comprueba la pertenencia en cada operacion.
/// </summary>
[Authorize]
[ApiController]
[Route("api/attempts")]
[Produces("application/json")]
public class AttemptsController : ControllerBase
{
    private readonly IAttemptService _attemptService;

    public AttemptsController(IAttemptService attemptService)
    {
        _attemptService = attemptService;
    }

    [HttpPost("start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Start([FromBody] StartAttemptRequestDto request, CancellationToken cancellationToken)
    {
        if (User.GetAttemptOwner() is not { } owner)
        {
            return Unauthorized();
        }

        var attemptId = await _attemptService.StartAsync(request.TestId, owner, cancellationToken);

        return Ok(new { attemptId });
    }

    [HttpPost("{attemptId:long}/answer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Answer(
        long attemptId,
        [FromBody] AnswerRequestDto request,
        CancellationToken cancellationToken)
    {
        if (User.GetAttemptOwner() is not { } owner)
        {
            return Unauthorized();
        }

        await _attemptService.RegisterAnswerAsync(
            attemptId, owner, request.QuestionId, request.AnswerOptionId, cancellationToken);

        return Ok(new { ok = true });
    }

    [HttpPost("{attemptId:long}/finish")]
    [ProducesResponseType(typeof(FinishAttemptResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Finish(long attemptId, CancellationToken cancellationToken)
    {
        if (User.GetAttemptOwner() is not { } owner)
        {
            return Unauthorized();
        }

        return Ok(await _attemptService.FinishAsync(attemptId, owner, cancellationToken));
    }
}
