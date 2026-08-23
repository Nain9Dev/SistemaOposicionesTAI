using Dapper;
using Npgsql;
using Oposiciones.Domain.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace Oposiciones.Infrastructure.Repositories
{
    public class AttemptRepository : IAttemptRepository
    {
        private readonly string _connectionString;

        public AttemptRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<long> StartAsync(long testId, string userName)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                return await conn.ExecuteScalarAsync<long>(
                    "AttemptStart",
                    new { TestId = testId, UserName = userName },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> AnswerAsync(long attemptId, long questionId, long answerOptionId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                return await conn.ExecuteScalarAsync<int>(
                    "AttemptAnswerUpsert",
                    new { AttemptId = attemptId, QuestionId = questionId, AnswerOptionId = answerOptionId },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<FinishAttemptResult> FinishAsync(long attemptId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                return await conn.QuerySingleAsync<FinishAttemptResult>(
                    "AttemptFinish",
                    new { AttemptId = attemptId },
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}