using Dapper;
using Oposiciones.Domain.Interfaces;
using Oposiciones.Infrastructure.Data;

namespace Oposiciones.Infrastructure.Repositories
{
    public class AttemptRepository : IAttemptRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AttemptRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<long> StartAsync(long testId, string userName, CancellationToken cancellationToken = default)
        {
            // El INSERT ... SELECT devuelve 0 filas si el test no existe, de forma que el
            // caso "test inexistente" se distingue sin provocar una violacion de clave ajena.
            const string sql = """
                INSERT INTO Attempts (TestId, UserName)
                SELECT t.Id, @UserName
                  FROM Tests t
                 WHERE t.Id = @TestId
                RETURNING Id;
                """;

            await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
            var attemptId = await conn.ExecuteScalarAsync<long?>(
                new CommandDefinition(sql, new { TestId = testId, UserName = userName }, cancellationToken: cancellationToken));

            return attemptId ?? 0;
        }

        public async Task<bool> AnswerAsync(long attemptId, long questionId, long? answerOptionId, CancellationToken cancellationToken = default)
        {
            // Una sola sentencia comprueba que el intento sigue abierto, que la pregunta pertenece
            // a su test y que la opcion pertenece a la pregunta. Sin estas guardas cualquier
            // cliente podia inyectar respuestas de otro test y falsear la nota.
            const string sql = """
                INSERT INTO AttemptAnswers (AttemptId, QuestionId, AnswerOptionId)
                SELECT a.Id, tq.QuestionId, @AnswerOptionId::BIGINT
                  FROM Attempts a
                  JOIN TestQuestions tq ON tq.TestId = a.TestId AND tq.QuestionId = @QuestionId::BIGINT
                 WHERE a.Id = @AttemptId::BIGINT
                   AND a.FinishedAt IS NULL
                   AND (
                        @AnswerOptionId::BIGINT IS NULL
                     OR EXISTS (
                            SELECT 1
                              FROM AnswerOptions ao
                             WHERE ao.Id = @AnswerOptionId::BIGINT
                               AND ao.QuestionId = tq.QuestionId)
                   )
                ON CONFLICT (AttemptId, QuestionId)
                DO UPDATE SET AnswerOptionId = EXCLUDED.AnswerOptionId,
                              AnsweredAt     = CURRENT_TIMESTAMP;
                """;

            await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
            var affected = await conn.ExecuteAsync(new CommandDefinition(
                sql,
                new { AttemptId = attemptId, QuestionId = questionId, AnswerOptionId = answerOptionId },
                cancellationToken: cancellationToken));

            return affected > 0;
        }

        public async Task<FinishAttemptResult> FinishAsync(long attemptId, CancellationToken cancellationToken = default)
        {
            // AttemptFinish concentra el baremo INAP; se invoca con SELECT porque es FUNCTION.
            const string sql = "SELECT * FROM AttemptFinish(@AttemptId::BIGINT);";

            await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
            return await conn.QuerySingleAsync<FinishAttemptResult>(
                new CommandDefinition(sql, new { AttemptId = attemptId }, cancellationToken: cancellationToken));
        }

        public async Task<string?> GetOwnerAsync(long attemptId, CancellationToken cancellationToken = default)
        {
            const string sql = "SELECT UserName FROM Attempts WHERE Id = @AttemptId;";

            await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
            return await conn.ExecuteScalarAsync<string?>(
                new CommandDefinition(sql, new { AttemptId = attemptId }, cancellationToken: cancellationToken));
        }

        public async Task<bool> IsFinishedAsync(long attemptId, CancellationToken cancellationToken = default)
        {
            const string sql = "SELECT FinishedAt IS NOT NULL FROM Attempts WHERE Id = @AttemptId;";

            await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
            return await conn.ExecuteScalarAsync<bool?>(
                new CommandDefinition(sql, new { AttemptId = attemptId }, cancellationToken: cancellationToken)) ?? false;
        }
    }
}
