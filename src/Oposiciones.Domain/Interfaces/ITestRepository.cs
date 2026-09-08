using Oposiciones.Domain.Entities;

namespace Oposiciones.Domain.Interfaces
{
    public interface ITestRepository
    {
        Task<long> GenerateAsync(string title, int syllabusTopicId, byte difficulty, int totalQuestions, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TestDetailRow>> GetTestDetailRowsAsync(long testId, CancellationToken cancellationToken = default);
    }
}
