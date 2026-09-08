using Dapper;
using Oposiciones.Domain.Entities;
using Oposiciones.Domain.Interfaces;
using Oposiciones.Infrastructure.Data;

namespace Oposiciones.Infrastructure.Repositories
{
    public class SyllabusRepository : ISyllabusRepository
    {
        /// <summary>
        /// Orden oficial del temario. No se puede usar Id: los bloques se siembran con IDENTITY
        /// y el orden de insercion no esta garantizado, de modo que ordenar por Id devolvia
        /// IV, I, III, II. Tampoco vale ordenar por Code alfabeticamente (I, II, III, IV es
        /// correcto por casualidad, pero IX iria antes que V).
        /// </summary>
        private const string BlockOrder = """
            CASE UPPER(sb.Code)
                WHEN 'I'   THEN 1
                WHEN 'II'  THEN 2
                WHEN 'III' THEN 3
                WHEN 'IV'  THEN 4
                ELSE 99
            END
            """;

        private readonly IDbConnectionFactory _connectionFactory;

        public SyllabusRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<SyllabusBlock>> GetBlocksAsync(CancellationToken cancellationToken = default)
        {
            const string sql = $"""
                SELECT sb.Id, sb.Code, sb.Name
                  FROM SyllabusBlocks sb
                 ORDER BY {BlockOrder}, sb.Code;
                """;

            await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
            var blocks = await conn.QueryAsync<SyllabusBlock>(
                new CommandDefinition(sql, cancellationToken: cancellationToken));

            return blocks.AsList();
        }

        public async Task<IReadOnlyList<SyllabusTopic>> GetTopicsByBlockAsync(int? blockId, CancellationToken cancellationToken = default)
        {
            // Un blockId nulo devuelve el temario completo en lugar de una lista vacia:
            // el cliente necesita poder pintar el arbol entero sin N peticiones.
            const string sql = $"""
                SELECT st.Id, st.BlockId, st.TopicNumber, st.Title
                  FROM SyllabusTopics st
                  JOIN SyllabusBlocks sb ON sb.Id = st.BlockId
                 WHERE (@BlockId::INT IS NULL OR st.BlockId = @BlockId::INT)
                 ORDER BY {BlockOrder}, st.TopicNumber;
                """;

            await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
            var topics = await conn.QueryAsync<SyllabusTopic>(
                new CommandDefinition(sql, new { BlockId = blockId }, cancellationToken: cancellationToken));

            return topics.AsList();
        }
    }
}
