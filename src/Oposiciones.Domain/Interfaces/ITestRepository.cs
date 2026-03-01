using Oposiciones.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Oposiciones.Domain.Interfaces
{
    public interface ITestRepository
    {
        Task<long> GenerateAsync(string title, int syllabusTopicId, byte difficulty, int totalQuestions);
        Task<IEnumerable<TestDetailRow>> GetTestDetailRowsAsync(long testId);
    }
}