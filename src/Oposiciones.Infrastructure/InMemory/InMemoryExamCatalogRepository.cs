using Oposiciones.Domain.Abstractions;
using Oposiciones.Domain.Catalog;

namespace Oposiciones.Infrastructure.InMemory;

/// <summary>Catalogo servido desde el contenido JSON cargado en memoria.</summary>
public sealed class InMemoryExamCatalogRepository : IExamCatalogRepository
{
    private readonly InMemoryStore _store;

    public InMemoryExamCatalogRepository(InMemoryStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<ExamProfile>> GetExamsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(_store.Catalog.Exams);

    public Task<ExamProfile?> GetExamAsync(string examCode, CancellationToken cancellationToken = default) =>
        Task.FromResult(_store.Catalog.FindExam(examCode));

    public Task<IReadOnlyList<SyllabusBlock>> GetBlocksAsync(
        string examCode,
        CancellationToken cancellationToken = default)
    {
        ExamProfile? exam = _store.Catalog.FindExam(examCode);
        IReadOnlyList<SyllabusBlock> blocks = exam?.Blocks ?? Array.Empty<SyllabusBlock>();
        return Task.FromResult(blocks);
    }

    public Task<IReadOnlyList<SyllabusTopic>> GetTopicsAsync(
        string examCode,
        string? blockCode = null,
        CancellationToken cancellationToken = default)
    {
        ExamProfile? exam = _store.Catalog.FindExam(examCode);
        if (exam is null)
        {
            return Task.FromResult<IReadOnlyList<SyllabusTopic>>(Array.Empty<SyllabusTopic>());
        }

        IEnumerable<SyllabusBlock> blocks = exam.Blocks;
        if (!string.IsNullOrWhiteSpace(blockCode))
        {
            blocks = blocks.Where(block =>
                string.Equals(block.Code, blockCode.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        IReadOnlyList<SyllabusTopic> topics = blocks
            .SelectMany(block => block.Topics)
            .OrderBy(topic => topic.Number)
            .ToList();

        return Task.FromResult(topics);
    }
}
