using Dapper;
using Oposiciones.Domain.Entities;
using Oposiciones.Domain.Interfaces;
using Oposiciones.Infrastructure.Data;

namespace Oposiciones.Infrastructure.Repositories
{
    public class TestRepository : ITestRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TestRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<long> GenerateAsync(
            string title,
            int syllabusTopicId,
            byte difficulty,
            int totalQuestions,
            CancellationToken cancellationToken = default)
        {
            // TestGenerate es una FUNCTION de PostgreSQL, no un PROCEDURE: con
            // CommandType.StoredProcedure Npgsql emite CALL y la invocacion falla.
            // Se llama con SELECT y se fija el tipo de cada argumento.
            const string sql = """
                SELECT TestGenerate(
                    @Title::VARCHAR,
                    @SyllabusTopicId::INT,
                    @Difficulty::SMALLINT,
                    @TotalQuestions::INT);
                """;

            var parameters = new
            {
                Title = title,
                SyllabusTopicId = syllabusTopicId,
                Difficulty = (short)difficulty,
                TotalQuestions = totalQuestions
            };

            await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
            return await conn.ExecuteScalarAsync<long>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        }

        public async Task<IReadOnlyList<TestDetailRow>> GetTestDetailRowsAsync(long testId, CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT
                    t.Id        AS TestId,
                    t.Title,
                    q.Id        AS QuestionId,
                    q.Statement,
                    ao.Id       AS OptionId,
                    ao.SortOrder,
                    ao.OptionText
                  FROM Tests t
                  JOIN TestQuestions tq ON tq.TestId = t.Id
                  JOIN Questions q      ON q.Id = tq.QuestionId
                  JOIN AnswerOptions ao ON ao.QuestionId = q.Id
                 WHERE t.Id = @TestId
                 ORDER BY tq.SortOrder, ao.SortOrder;
                """;

            await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
            var rows = await conn.QueryAsync<TestDetailRow>(
                new CommandDefinition(sql, new { TestId = testId }, cancellationToken: cancellationToken));

            return rows.AsList();
        }
    }
}
