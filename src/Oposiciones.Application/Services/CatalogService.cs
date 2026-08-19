using Oposiciones.Application.Contracts;
using Oposiciones.Application.Mapping;
using Oposiciones.Domain.Abstractions;
using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Common;

namespace Oposiciones.Application.Services;

/// <summary>Consulta del temario: convocatorias, bloques y temas.</summary>
public interface ICatalogService
{
    Task<IReadOnlyList<ExamSummaryDto>> GetExamsAsync(CancellationToken cancellationToken = default);

    Task<ExamDetailDto> GetExamAsync(string examCode, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BlockDto>> GetBlocksAsync(string examCode, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TopicDto>> GetTopicsAsync(
        string examCode,
        string? blockCode,
        CancellationToken cancellationToken = default);
}

/// <inheritdoc />
public sealed class CatalogService : ICatalogService
{
    private readonly IExamCatalogRepository _catalog;

    public CatalogService(IExamCatalogRepository catalog)
    {
        _catalog = catalog;
    }

    public async Task<IReadOnlyList<ExamSummaryDto>> GetExamsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ExamProfile> exams = await _catalog.GetExamsAsync(cancellationToken).ConfigureAwait(false);
        return exams.Select(exam => exam.ToSummaryDto()).ToList();
    }

    public async Task<ExamDetailDto> GetExamAsync(string examCode, CancellationToken cancellationToken = default)
    {
        ExamProfile exam = await RequireExamAsync(examCode, cancellationToken).ConfigureAwait(false);
        return exam.ToDetailDto();
    }

    public async Task<IReadOnlyList<BlockDto>> GetBlocksAsync(
        string examCode,
        CancellationToken cancellationToken = default)
    {
        await RequireExamAsync(examCode, cancellationToken).ConfigureAwait(false);
        IReadOnlyList<SyllabusBlock> blocks =
            await _catalog.GetBlocksAsync(examCode, cancellationToken).ConfigureAwait(false);
        return blocks.Select(block => block.ToDto()).ToList();
    }

    public async Task<IReadOnlyList<TopicDto>> GetTopicsAsync(
        string examCode,
        string? blockCode,
        CancellationToken cancellationToken = default)
    {
        await RequireExamAsync(examCode, cancellationToken).ConfigureAwait(false);
        IReadOnlyList<SyllabusTopic> topics =
            await _catalog.GetTopicsAsync(examCode, blockCode, cancellationToken).ConfigureAwait(false);
        return topics.Select(topic => topic.ToDto()).ToList();
    }

    private async Task<ExamProfile> RequireExamAsync(string examCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(examCode))
        {
            throw new DomainException("Debe indicarse el codigo de la convocatoria.");
        }

        ExamProfile? exam = await _catalog.GetExamAsync(examCode, cancellationToken).ConfigureAwait(false);
        return exam ?? throw new NotFoundException("la convocatoria", examCode);
    }
}
