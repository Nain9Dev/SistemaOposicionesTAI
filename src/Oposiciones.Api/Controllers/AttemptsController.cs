using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Oposiciones.Application.Contracts;
using Oposiciones.Application.Services;

namespace Oposiciones.Api.Controllers;

/// <summary>Ciclo de vida de los intentos y analitica de rendimiento del opositor.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/attempts")]
[Route("api/attempts")]
[Produces("application/json")]
public sealed class AttemptsController : ControllerBase
{
    private readonly IAttemptService _attempts;

    public AttemptsController(IAttemptService attempts)
    {
        _attempts = attempts;
    }

    /// <summary>Inicia un intento sobre un test ya generado.</summary>
    [HttpPost("start")]
    [ProducesResponseType(typeof(AttemptDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AttemptDto>> Start(
        [FromBody] StartAttemptRequest request,
        CancellationToken cancellationToken)
    {
        AttemptDto attempt = await _attempts.StartAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(Get), new { attemptId = attempt.AttemptId }, attempt);
    }

    /// <summary>Estado de un intento.</summary>
    [HttpGet("{attemptId:long}")]
    [ProducesResponseType(typeof(AttemptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AttemptDto>> Get(long attemptId, CancellationToken cancellationToken)
        => Ok(await _attempts.GetAsync(attemptId, cancellationToken).ConfigureAwait(false));

    /// <summary>
    /// Registra la respuesta a una pregunta. Enviar <c>answerOptionId</c> nulo deja la pregunta en
    /// blanco, que con el baremo oficial es una decision de examen y no un dato ausente.
    /// </summary>
    [HttpPost("{attemptId:long}/answer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Answer(
        long attemptId,
        [FromBody] SubmitAnswerRequest request,
        CancellationToken cancellationToken)
    {
        bool ok = await _attempts.AnswerAsync(attemptId, request, cancellationToken).ConfigureAwait(false);
        return Ok(new { ok });
    }

    /// <summary>
    /// Cierra el intento y devuelve la correccion con el desglose por bloque y por tema.
    /// </summary>
    /// <remarks>
    /// Es idempotente: volver a llamar sobre un intento ya cerrado devuelve la misma correccion sin
    /// alterar la fecha de finalizacion registrada.
    /// </remarks>
    [HttpPost("{attemptId:long}/finish")]
    [ProducesResponseType(typeof(AttemptResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AttemptResultDto>> Finish(long attemptId, CancellationToken cancellationToken)
        => Ok(await _attempts.FinishAsync(attemptId, cancellationToken).ConfigureAwait(false));

    /// <summary>Test con soluciones y explicaciones, disponible solo tras finalizar el intento.</summary>
    [HttpGet("{attemptId:long}/review")]
    [ProducesResponseType(typeof(TestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TestDto>> Review(long attemptId, CancellationToken cancellationToken)
        => Ok(await _attempts.GetReviewAsync(attemptId, cancellationToken).ConfigureAwait(false));

    /// <summary>Historial paginado de intentos de un usuario.</summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(PagedResponse<AttemptSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<AttemptSummaryDto>>> History(
        [FromQuery] string userName = "demo",
        [FromQuery] string? examCode = null,
        [FromQuery] int? page = null,
        [FromQuery] int? pageSize = null,
        CancellationToken cancellationToken = default)
        => Ok(await _attempts
            .GetHistoryAsync(userName, examCode, page, pageSize, cancellationToken)
            .ConfigureAwait(false));

    /// <summary>
    /// Plan de repaso derivado del historial: temas por debajo del umbral de acierto y cuantas
    /// preguntas conviene hacer de cada uno.
    /// </summary>
    /// <remarks>
    /// La salida encaja directamente en <c>POST /api/tests/generate</c>: cada recomendacion se
    /// traduce en una seccion con su bloque, su tema y su numero de preguntas.
    /// </remarks>
    [HttpGet("study-plan")]
    [ProducesResponseType(typeof(StudyPlanDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<StudyPlanDto>> StudyPlan(
        [FromQuery] string userName = "demo",
        [FromQuery] string? examCode = null,
        [FromQuery] int maxRecommendations = 5,
        CancellationToken cancellationToken = default)
        => Ok(await _attempts
            .GetStudyPlanAsync(userName, examCode, maxRecommendations, cancellationToken)
            .ConfigureAwait(false));
}
