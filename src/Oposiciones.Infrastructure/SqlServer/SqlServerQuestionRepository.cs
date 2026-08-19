using Dapper;
using Microsoft.Data.SqlClient;
using Oposiciones.Domain.Abstractions;
using Oposiciones.Domain.Assessments;
using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Common;

namespace Oposiciones.Infrastructure.SqlServer;

/// <summary>Banco de preguntas sobre SQL Server.</summary>
public sealed class SqlServerQuestionRepository : IQuestionRepository
{
    private readonly ISqlConnectionFactory _connections;
    private readonly SqlResilience _resilience;

    public SqlServerQuestionRepository(ISqlConnectionFactory connections, SqlResilience resilience)
    {
        _connections = connections;
        _resilience = resilience;
    }

    public Task<PagedResult<Question>> SearchAsync(
        QuestionQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        return _resilience.ExecuteAsync(async token =>
        {
            var parameters = new
            {
                ExamCode = query.ExamCode,
                BlockCode = query.BlockCode,
                TopicNumber = query.TopicNumber,
                TopicId = query.TopicId,
                Difficulties = SqlCommands.ToCsv(query.Difficulties.Select(d => (int)d)),
                Tags = SqlCommands.ToCsv(query.Tags),
                Search = query.Search,
                IsActive = query.IsActive,
                Offset = query.Paging.Offset,
                PageSize = query.Paging.PageSize,
            };

            await using SqlConnection connection = _connections.Create();
            await using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(
                SqlCommands.Create("dbo.QuestionSearch", parameters, _connections, token))
                .ConfigureAwait(false);

            IReadOnlyList<Question> questions = await ReadQuestionsAsync(reader).ConfigureAwait(false);
            long total = await reader.ReadFirstAsync<long>().ConfigureAwait(false);

            return PagedResult<Question>.From(questions, query.Paging, total);
        }, cancellationToken);
    }

    public Task<Question?> GetAsync(long questionId, CancellationToken cancellationToken = default) =>
        _resilience.ExecuteAsync(async token =>
        {
            await using SqlConnection connection = _connections.Create();
            await using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(
                SqlCommands.Create("dbo.QuestionGet", new { QuestionId = questionId }, _connections, token))
                .ConfigureAwait(false);

            IReadOnlyList<Question> questions = await ReadQuestionsAsync(reader).ConfigureAwait(false);
            return questions.FirstOrDefault();
        }, cancellationToken);

    public Task<IReadOnlyList<Question>> DrawAsync(
        QuestionDraw draw,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draw);

        return _resilience.ExecuteAsync(async token =>
        {
            await using SqlConnection connection = _connections.Create();
            await using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(
                SqlCommands.Create("dbo.QuestionsDraw", BuildDrawParameters(draw), _connections, token))
                .ConfigureAwait(false);

            return await ReadQuestionsAsync(reader).ConfigureAwait(false);
        }, cancellationToken);
    }

    public Task<int> CountAvailableAsync(QuestionDraw draw, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draw);

        return _resilience.ExecuteAsync(async token =>
        {
            await using SqlConnection connection = _connections.Create();
            long available = await connection.ExecuteScalarAsync<long>(
                SqlCommands.Create(
                    "dbo.QuestionsCountAvailable",
                    new
                    {
                        draw.ExamCode,
                        draw.BlockCode,
                        draw.TopicNumber,
                        draw.TopicId,
                        Difficulties = SqlCommands.ToCsv(draw.Difficulties.Select(d => (int)d)),
                        Tags = SqlCommands.ToCsv(draw.Tags),
                        ExcludeIds = SqlCommands.ToCsv(draw.ExcludeQuestionIds),
                    },
                    _connections,
                    token))
                .ConfigureAwait(false);

            return (int)Math.Min(available, int.MaxValue);
        }, cancellationToken);
    }

    public Task<IReadOnlyList<TopicCoverage>> GetCoverageAsync(
        string examCode,
        CancellationToken cancellationToken = default) =>
        _resilience.ExecuteAsync(async token =>
        {
            await using SqlConnection connection = _connections.Create();
            IEnumerable<TopicCoverage> rows = await connection.QueryAsync<TopicCoverage>(
                SqlCommands.Create(
                    "dbo.QuestionBankCoverage",
                    new { ExamCode = examCode },
                    _connections,
                    token))
                .ConfigureAwait(false);

            IReadOnlyList<TopicCoverage> coverage = rows.ToList();
            return coverage;
        }, cancellationToken);

    private static object BuildDrawParameters(QuestionDraw draw) => new
    {
        draw.ExamCode,
        draw.BlockCode,
        draw.TopicNumber,
        draw.TopicId,
        Difficulties = SqlCommands.ToCsv(draw.Difficulties.Select(d => (int)d)),
        Tags = SqlCommands.ToCsv(draw.Tags),
        ExcludeIds = SqlCommands.ToCsv(draw.ExcludeQuestionIds),
        Count = draw.Count,
        draw.Seed,
    };

    /// <summary>
    /// Lee el par de conjuntos (preguntas, opciones) que devuelven todos los procedimientos del
    /// banco y los recompone en entidades del dominio.
    /// </summary>
    internal static async Task<IReadOnlyList<Question>> ReadQuestionsAsync(SqlMapper.GridReader reader)
    {
        List<QuestionRow> questionRows = (await reader.ReadAsync<QuestionRow>().ConfigureAwait(false)).ToList();
        List<OptionRow> optionRows = (await reader.ReadAsync<OptionRow>().ConfigureAwait(false)).ToList();

        ILookup<long, OptionRow> optionsByQuestion = optionRows.ToLookup(option => option.QuestionId);

        return questionRows
            .Select(row => row.ToDomain(
                optionsByQuestion[row.Id]
                    .OrderBy(option => option.SortOrder)
                    .Select(option => new AnswerOption(
                        option.Id,
                        option.SortOrder,
                        option.OptionText,
                        option.IsCorrect))
                    .ToList()))
            .ToList();
    }
}
