using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Oposiciones.Application.Contracts;
using Oposiciones.Application.Services;

namespace Oposiciones.Api.Controllers;

/// <summary>Consulta del banco de preguntas y de su cobertura por tema.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/questions")]
[Route("api/questions")]
[Produces("application/json")]
public sealed class QuestionsController : ControllerBase
{
    private readonly IQuestionBankService _bank;

    public QuestionsController(IQuestionBankService bank)
    {
        _bank = bank;
    }

    /// <summary>
    /// Busca preguntas por convocatoria, bloque, tema, dificultad, etiquetas o texto libre.
    /// </summary>
    /// <remarks>
    /// Con <c>includeSolutions=true</c> la respuesta incluye la opcion correcta, la explicacion y
    /// la referencia normativa. Es el modo pensado para revisar y ampliar el banco, no para
    /// alimentar un simulacro.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<QuestionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<QuestionDto>>> Search(
        [FromQuery] string? examCode,
        [FromQuery] string? blockCode,
        [FromQuery] int? topicNumber,
        [FromQuery] int? topicId,
        [FromQuery] int[]? difficulties,
        [FromQuery] string[]? tags,
        [FromQuery] string? search,
        [FromQuery] bool includeSolutions,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var request = new QuestionSearchRequest
        {
            ExamCode = examCode,
            BlockCode = blockCode,
            TopicNumber = topicNumber,
            TopicId = topicId,
            Difficulties = difficulties ?? Array.Empty<int>(),
            Tags = tags ?? Array.Empty<string>(),
            Search = search,
            IncludeSolutions = includeSolutions,
            Page = page,
            PageSize = pageSize,
        };

        return Ok(await _bank.SearchAsync(request, cancellationToken).ConfigureAwait(false));
    }

    /// <summary>Pregunta concreta.</summary>
    [HttpGet("{questionId:long}")]
    [ProducesResponseType(typeof(QuestionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuestionDto>> Get(
        long questionId,
        [FromQuery] bool includeSolution,
        CancellationToken cancellationToken)
        => Ok(await _bank.GetAsync(questionId, includeSolution, cancellationToken).ConfigureAwait(false));

    /// <summary>
    /// Cobertura del banco tema a tema.
    /// </summary>
    /// <remarks>
    /// Devuelve tambien los temas con cero preguntas: es el listado con el que se decide por donde
    /// seguir rellenando el banco.
    /// </remarks>
    [HttpGet("coverage")]
    [ProducesResponseType(typeof(BankCoverageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BankCoverageDto>> GetCoverage(
        [FromQuery] string examCode = "TAI",
        CancellationToken cancellationToken = default)
        => Ok(await _bank.GetCoverageAsync(examCode, cancellationToken).ConfigureAwait(false));
}
