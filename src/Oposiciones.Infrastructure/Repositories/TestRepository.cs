using Dapper;
using Npgsql;
using Oposiciones.Domain.Interfaces;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using Oposiciones.Domain.Entities;

namespace Oposiciones.Infrastructure.Repositories
{
    public class TestRepository : ITestRepository
    {
        private readonly string _connectionString;

        public TestRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<long> GenerateAsync(string title, int syllabusTopicId, byte difficulty, int totalQuestions)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var parameters = new
                {
                    Title = title,
                    SyllabusTopicId = syllabusTopicId,
                    Difficulty = difficulty,
                    TotalQuestions = totalQuestions
                };

                return await conn.ExecuteScalarAsync<long>(
                    "TestGenerate",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
        }
        public async Task<IEnumerable<TestDetailRow>> GetTestDetailRowsAsync(long testId)
        {
            const string sql = @"
            SELECT
              t.Id AS TestId,
              t.Title,
              q.Id AS QuestionId,
              q.Statement,
              ao.Id AS OptionId,
              ao.SortOrder,
              ao.OptionText
            FROM Tests t
            JOIN TestQuestions tq ON tq.TestId = t.Id
            JOIN Questions q ON q.Id = tq.QuestionId
            JOIN AnswerOptions ao ON ao.QuestionId = q.Id
            WHERE t.Id = @TestId
            ORDER BY tq.SortOrder, ao.SortOrder;";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                return await conn.QueryAsync<TestDetailRow>(sql, new { TestId = testId });
            }
        }
    }
}