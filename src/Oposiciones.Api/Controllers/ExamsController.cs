using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Oposiciones.Application.Contracts;
using Oposiciones.Application.Services;

namespace Oposiciones.Api.Controllers;

/// <summary>Catalogo de convocatorias y temarios oficiales.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/exams")]
[Route("api/exams")]
[Produces("application/json")]
public sealed class ExamsController : ControllerBase
{
    private readonly ICatalogService _catalog;

    public ExamsController(ICatalogService catalog)
    {
        _catalog = catalog;
    }

    /// <summary>Convocatorias disponibles con su baremo y el formato de su ejercicio.</summary>
    [HttpGet]
    [OutputCache(PolicyName = "catalogo")]
    [ProducesResponseType(typeof(IReadOnlyList<ExamSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ExamSummaryDto>>> GetExams(CancellationToken cancellationToken)
        => Ok(await _catalog.GetExamsAsync(cancellationToken).ConfigureAwait(false));

    /// <summary>Convocatoria concreta con su temario completo.</summary>
    /// <param name="examCode">Codigo de la convocatoria, por ejemplo <c>TAI</c>.</param>
    /// <param name="cancellationToken">Token de cancelacion.</param>
    [HttpGet("{examCode}")]
    [OutputCache(PolicyName = "catalogo")]
    [ProducesResponseType(typeof(ExamDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExamDetailDto>> GetExam(string examCode, CancellationToken cancellationToken)
        => Ok(await _catalog.GetExamAsync(examCode, cancellationToken).ConfigureAwait(false));

    /// <summary>Bloques del temario de una convocatoria.</summary>
    [HttpGet("{examCode}/blocks")]
    [OutputCache(PolicyName = "catalogo")]
    [ProducesResponseType(typeof(IReadOnlyList<BlockDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<BlockDto>>> GetBlocks(
        string examCode,
        CancellationToken cancellationToken)
        => Ok(await _catalog.GetBlocksAsync(examCode, cancellationToken).ConfigureAwait(false));

    /// <summary>Temas de una convocatoria, opcionalmente acotados a un bloque.</summary>
    /// <param name="examCode">Codigo de la convocatoria.</param>
    /// <param name="blockCode">Codigo del bloque, por ejemplo <c>II</c>. Opcional.</param>
    /// <param name="cancellationToken">Token de cancelacion.</param>
    [HttpGet("{examCode}/topics")]
    [OutputCache(PolicyName = "catalogo")]
    [ProducesResponseType(typeof(IReadOnlyList<TopicDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<TopicDto>>> GetTopics(
        string examCode,
        [FromQuery] string? blockCode,
        CancellationToken cancellationToken)
        => Ok(await _catalog.GetTopicsAsync(examCode, blockCode, cancellationToken).ConfigureAwait(false));
}
