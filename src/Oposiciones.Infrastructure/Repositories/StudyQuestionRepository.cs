using Dapper;
using Oposiciones.Domain.Entities;
using Oposiciones.Domain.Interfaces;
using Oposiciones.Infrastructure.Data;

namespace Oposiciones.Infrastructure.Repositories;

public class StudyQuestionRepository : IStudyQuestionRepository
{
    /// <summary>Filtro compartido por el recuento y por la seleccion, para que nunca divergan.</summary>
    private const string FilterClause = """
                   q.IsActive
               AND (@BlockId::INT      IS NULL OR sb.Id = @BlockId::INT)
               AND (@BlockCode::VARCHAR IS NULL OR UPPER(sb.Code) = UPPER(@BlockCode::VARCHAR))
               AND (@TopicId::INT      IS NULL OR st.Id = @TopicId::INT)
               AND (@Difficulty::INT   IS NULL OR q.Difficulty = @Difficulty::INT)
        """;

    private readonly IDbConnectionFactory _connectionFactory;

    public StudyQuestionRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<StudyQuestion>> GetRandomAsync(
        StudyQuestionFilter filter, CancellationToken cancellationToken = default)
    {
        // El orden aleatorio se resuelve sobre los identificadores y despues se recuperan las
        // opciones: asi el ORDER BY random() no se aplica al producto pregunta x opciones.
        const string sql = $"""
            WITH seleccion AS (
                SELECT q.Id, row_number() OVER (ORDER BY random()) AS Posicion
                  FROM Questions q
                  JOIN SyllabusTopics st ON st.Id = q.SyllabusTopicId
                  JOIN SyllabusBlocks sb ON sb.Id = st.BlockId
                 WHERE {FilterClause}
                 ORDER BY random()
                 LIMIT @Limit::INT
            )
            SELECT
                q.Id          AS QuestionId,
                q.Statement,
                q.Explanation,
                q.Difficulty,
                sb.Id         AS BlockId,
                sb.Code       AS BlockCode,
                sb.Name       AS BlockName,
                st.Id         AS TopicId,
                st.Title      AS TopicTitle,
                ao.Id         AS OptionId,
                ao.SortOrder,
                ao.OptionText,
                ao.IsCorrect
              FROM seleccion s
              JOIN Questions q       ON q.Id = s.Id
              JOIN SyllabusTopics st ON st.Id = q.SyllabusTopicId
              JOIN SyllabusBlocks sb ON sb.Id = st.BlockId
              JOIN AnswerOptions ao  ON ao.QuestionId = q.Id
             ORDER BY s.Posicion, ao.SortOrder;
            """;

        await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
        var rows = await conn.QueryAsync<StudyQuestionRow>(
            new CommandDefinition(sql, ToParameters(filter), cancellationToken: cancellationToken));

        return Group(rows);
    }

    public async Task<int> CountAsync(StudyQuestionFilter filter, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT COUNT(*)::INT
              FROM Questions q
              JOIN SyllabusTopics st ON st.Id = q.SyllabusTopicId
              JOIN SyllabusBlocks sb ON sb.Id = st.BlockId
             WHERE {FilterClause};
            """;

        await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
        return await conn.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, ToParameters(filter), cancellationToken: cancellationToken));
    }

    private static object ToParameters(StudyQuestionFilter filter) => new
    {
        filter.BlockId,
        filter.BlockCode,
        filter.TopicId,
        filter.Difficulty,
        filter.Limit
    };

    /// <summary>Agrupa las filas planas conservando el orden aleatorio de la consulta.</summary>
    private static List<StudyQuestion> Group(IEnumerable<StudyQuestionRow> rows)
    {
        var result = new List<StudyQuestion>();
        var options = new List<StudyQuestionOption>();
        StudyQuestionRow? current = null;

        foreach (var row in rows)
        {
            if (current is not null && row.QuestionId != current.QuestionId)
            {
                result.Add(Build(current, options));
                options = [];
            }

            current = row;
            options.Add(new StudyQuestionOption
            {
                Id = row.OptionId,
                SortOrder = row.SortOrder,
                Text = row.OptionText,
                IsCorrect = row.IsCorrect
            });
        }

        if (current is not null)
        {
            result.Add(Build(current, options));
        }

        return result;
    }

    private static StudyQuestion Build(StudyQuestionRow row, List<StudyQuestionOption> options) => new()
    {
        Id = row.QuestionId,
        Statement = row.Statement,
        Explanation = row.Explanation,
        Difficulty = row.Difficulty,
        BlockId = row.BlockId,
        BlockCode = row.BlockCode,
        BlockName = row.BlockName,
        TopicId = row.TopicId,
        TopicTitle = row.TopicTitle,
        Options = options
    };
}
