using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Oposiciones.Application.Contracts;
using Oposiciones.Application.Services;

namespace Oposiciones.Api.Controllers;

/// <summary>
/// Rutas de temario de la primera version de la Api.
/// <para>
/// Se mantienen para no romper a los clientes ya desplegados. El catalogo completo, con soporte
/// multi-convocatoria, vive en <c>/api/exams</c>; estas rutas asumen la convocatoria TAI.
/// </para>
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/syllabus")]
[Produces("application/json")]
public sealed class SyllabusController : ControllerBase
{
    private const string DefaultExamCode = "TAI";

    private readonly ICatalogService _catalog;

    public SyllabusController(ICatalogService catalog)
    {
        _catalog = catalog;
    }

    /// <summary>Bloques del temario.</summary>
    [HttpGet("blocks")]
    [OutputCache(PolicyName = "catalogo")]
    [ProducesResponseType(typeof(IReadOnlyList<BlockDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BlockDto>>> GetBlocks(
        [FromQuery] string examCode = DefaultExamCode,
        CancellationToken cancellationToken = default)
        => Ok(await _catalog.GetBlocksAsync(examCode, cancellationToken).ConfigureAwait(false));

    /// <summary>
    /// Temas de un bloque. Admite el identificador numerico del bloque de la version anterior y
    /// tambien su codigo (<c>I</c>, <c>II</c>, ...), que es el criterio estable entre despliegues.
    /// </summary>
    [HttpGet("topics")]
    [OutputCache(PolicyName = "catalogo")]
    [ProducesResponseType(typeof(IReadOnlyList<TopicDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TopicDto>>> GetTopics(
        [FromQuery] int? blockId,
        [FromQuery] string? blockCode,
        [FromQuery] string examCode = DefaultExamCode,
        CancellationToken cancellationToken = default)
    {
        if (blockCode is null && blockId is int id)
        {
            IReadOnlyList<BlockDto> blocks =
                await _catalog.GetBlocksAsync(examCode, cancellationToken).ConfigureAwait(false);
            blockCode = blocks.FirstOrDefault(block => block.Id == id)?.Code;
        }

        return Ok(await _catalog.GetTopicsAsync(examCode, blockCode, cancellationToken).ConfigureAwait(false));
    }
}
