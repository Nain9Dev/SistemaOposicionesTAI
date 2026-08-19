using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Oposiciones.Application.Contracts;
using Oposiciones.Application.Services;

namespace Oposiciones.Api.Controllers;

/// <summary>Generacion y consulta de tests.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tests")]
[Route("api/tests")]
[Produces("application/json")]
public sealed class TestsController : ControllerBase
{
    private readonly ITestGenerationService _tests;

    public TestsController(ITestGenerationService tests)
    {
        _tests = tests;
    }

    /// <summary>
    /// Genera un test a partir de una receta declarativa.
    /// </summary>
    /// <remarks>
    /// Con el cuerpo vacio o solo con <c>examCode</c> se genera un simulacro repartido segun el
    /// peso oficial de cada bloque. Indicando <c>sections</c> se controla el reparto hasta el nivel
    /// de tema, por numero exacto de preguntas o por porcentaje.
    /// <para>
    /// Fijar <c>seed</c> reproduce exactamente el mismo test: sirve para compartirlo, repetirlo o
    /// comparar resultados entre dos sesiones de estudio.
    /// </para>
    /// </remarks>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(TestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TestDto>> Generate(
        [FromBody] GenerateTestRequest? request,
        CancellationToken cancellationToken)
    {
        TestDto test = await _tests
            .GenerateAsync(request ?? new GenerateTestRequest(), cancellationToken)
            .ConfigureAwait(false);

        return CreatedAtAction(nameof(GetById), new { testId = test.TestId }, test);
    }

    /// <summary>
    /// Recupera un test. En modo estudio incluye soluciones y explicaciones; en modo examen, no.
    /// </summary>
    [HttpGet("{testId:long}")]
    [ProducesResponseType(typeof(TestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TestDto>> GetById(long testId, CancellationToken cancellationToken)
        => Ok(await _tests.GetAsync(testId, cancellationToken).ConfigureAwait(false));
}
