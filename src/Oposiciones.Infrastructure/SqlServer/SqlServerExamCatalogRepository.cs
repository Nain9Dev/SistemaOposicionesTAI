using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Oposiciones.Domain.Abstractions;
using Oposiciones.Domain.Catalog;

namespace Oposiciones.Infrastructure.SqlServer;

/// <summary>Catalogo leido de SQL Server mediante procedimientos almacenados.</summary>
public sealed class SqlServerExamCatalogRepository : IExamCatalogRepository
{
    private readonly ISqlConnectionFactory _connections;
    private readonly SqlResilience _resilience;

    public SqlServerExamCatalogRepository(ISqlConnectionFactory connections, SqlResilience resilience)
    {
        _connections = connections;
        _resilience = resilience;
    }

    public Task<IReadOnlyList<ExamProfile>> GetExamsAsync(CancellationToken cancellationToken = default) =>
        _resilience.ExecuteAsync(async token =>
        {
            await using SqlConnection connection = _connections.Create();
            IEnumerable<ExamRow> rows = await connection.QueryAsync<ExamRow>(
                SqlCommands.Create("dbo.ExamList", null, _connections, token)).ConfigureAwait(false);

            IReadOnlyList<ExamProfile> exams = rows
                .Select(row => ToProfile(row, Array.Empty<SyllabusBlock>()))
                .ToList();

            return exams;
        }, cancellationToken);

    public Task<ExamProfile?> GetExamAsync(string examCode, CancellationToken cancellationToken = default) =>
        _resilience.ExecuteAsync(async token =>
        {
            await using SqlConnection connection = _connections.Create();
            await using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(
                SqlCommands.Create("dbo.ExamGet", new { Code = examCode }, _connections, token))
                .ConfigureAwait(false);

            ExamRow? exam = (await reader.ReadAsync<ExamRow>().ConfigureAwait(false)).FirstOrDefault();
            if (exam is null)
            {
                return null;
            }

            List<BlockRow> blockRows = (await reader.ReadAsync<BlockRow>().ConfigureAwait(false)).ToList();
            List<TopicRow> topicRows = (await reader.ReadAsync<TopicRow>().ConfigureAwait(false)).ToList();

            ILookup<int, TopicRow> topicsByBlock = topicRows.ToLookup(topic => topic.BlockId);

            List<SyllabusBlock> blocks = blockRows
                .Select(block => ToBlock(block, topicsByBlock[block.Id]))
                .ToList();

            return ToProfile(exam, blocks);
        }, cancellationToken);

    public async Task<IReadOnlyList<SyllabusBlock>> GetBlocksAsync(
        string examCode,
        CancellationToken cancellationToken = default)
    {
        ExamProfile? exam = await GetExamAsync(examCode, cancellationToken).ConfigureAwait(false);
        return exam?.Blocks ?? Array.Empty<SyllabusBlock>();
    }

    public Task<IReadOnlyList<SyllabusTopic>> GetTopicsAsync(
        string examCode,
        string? blockCode = null,
        CancellationToken cancellationToken = default) =>
        _resilience.ExecuteAsync(async token =>
        {
            await using SqlConnection connection = _connections.Create();
            IEnumerable<TopicRow> rows = await connection.QueryAsync<TopicRow>(
                SqlCommands.Create(
                    "dbo.SyllabusTopicList",
                    new { ExamCode = examCode, BlockCode = blockCode },
                    _connections,
                    token))
                .ConfigureAwait(false);

            IReadOnlyList<SyllabusTopic> topics = rows.Select(row => row.ToDomain()).ToList();
            return topics;
        }, cancellationToken);

    private static ExamProfile ToProfile(ExamRow row, IReadOnlyList<SyllabusBlock> blocks) => new()
    {
        Id = row.Id,
        Code = row.Code,
        Name = row.Name,
        Authority = row.Authority,
        Description = row.Description,
        OfficialSource = row.ToSource(),
        Scoring = row.ToScoring(),
        Format = row.ToFormat(),
        IsActive = row.IsActive,
        Blocks = blocks,
    };

    private static SyllabusBlock ToBlock(BlockRow row, IEnumerable<TopicRow> topics) => new()
    {
        Id = row.Id,
        ExamId = row.ExamId,
        ExamCode = row.ExamCode,
        Code = row.Code,
        Name = row.Name,
        DisplayOrder = row.DisplayOrder,
        ExamWeightPercent = row.ExamWeightPercent,
        Topics = topics.OrderBy(topic => topic.Number).Select(topic => topic.ToDomain()).ToList(),
    };
}

/// <summary>
/// Construye los <see cref="CommandDefinition"/> de Dapper con el timeout y el token de
/// cancelacion ya aplicados. Centralizarlo evita que una consulta se quede sin cancelacion por
/// olvido, que es exactamente lo que agota un pool de conexiones bajo carga.
/// </summary>
internal static class SqlCommands
{
    public static CommandDefinition Create(
        string procedureName,
        object? parameters,
        ISqlConnectionFactory factory,
        CancellationToken cancellationToken) =>
        new(
            procedureName,
            parameters,
            commandType: CommandType.StoredProcedure,
            commandTimeout: factory.CommandTimeoutSeconds,
            cancellationToken: cancellationToken);

    /// <summary>Convierte una fecha leida de SQL Server, que llega sin zona, en UTC explicito.</summary>
    public static DateTimeOffset ToUtc(DateTime value) =>
        new(DateTime.SpecifyKind(value, DateTimeKind.Utc));

    public static DateTimeOffset? ToUtc(DateTime? value) =>
        value is null ? null : ToUtc(value.Value);

    /// <summary>Serializa una lista para los parametros que el T-SQL expande con STRING_SPLIT.</summary>
    public static string? ToCsv<T>(IEnumerable<T> values)
    {
        string csv = string.Join(',', values);
        return csv.Length == 0 ? null : csv;
    }
}
