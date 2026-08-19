using Dapper;
using Microsoft.Data.SqlClient;
using Oposiciones.Domain.Abstractions;
using Oposiciones.Domain.Assessments;
using Oposiciones.Domain.Attempts;
using Oposiciones.Domain.Common;
using Oposiciones.Domain.Scoring;

namespace Oposiciones.Infrastructure.SqlServer;

/// <summary>Intentos persistidos en SQL Server.</summary>
public sealed class SqlServerAttemptRepository : IAttemptRepository
{
    private readonly ISqlConnectionFactory _connections;
    private readonly SqlResilience _resilience;

    public SqlServerAttemptRepository(ISqlConnectionFactory connections, SqlResilience resilience)
    {
        _connections = connections;
        _resilience = resilience;
    }

    public async Task<Attempt> StartAsync(
        long testId,
        string userName,
        CancellationToken cancellationToken = default)
    {
        Attempt? attempt = await _resilience.ExecuteAsync(async token =>
        {
            await using SqlConnection connection = _connections.Create();
            AttemptRow? row = await connection.QueryFirstOrDefaultAsync<AttemptRow>(
                SqlCommands.Create(
                    "dbo.AttemptStart",
                    new { TestId = testId, UserName = userName },
                    _connections,
                    token))
                .ConfigureAwait(false);

            return row is null ? null : ToDomain(row);
        }, cancellationToken).ConfigureAwait(false);

        return attempt
            ?? throw new InvalidOperationException("SQL Server no ha devuelto el intento recien creado.");
    }

    public Task<Attempt?> GetAsync(long attemptId, CancellationToken cancellationToken = default) =>
        _resilience.ExecuteAsync(async token =>
        {
            await using SqlConnection connection = _connections.Create();
            AttemptRow? row = await connection.QueryFirstOrDefaultAsync<AttemptRow>(
                SqlCommands.Create("dbo.AttemptGet", new { AttemptId = attemptId }, _connections, token))
                .ConfigureAwait(false);

            return row is null ? null : ToDomain(row);
        }, cancellationToken);

    public Task<bool> AnswerAsync(
        long attemptId,
        long questionId,
        long? answerOptionId,
        CancellationToken cancellationToken = default) =>
        _resilience.ExecuteAsync(async token =>
        {
            await using SqlConnection connection = _connections.Create();
            int result = await connection.ExecuteScalarAsync<int>(
                SqlCommands.Create(
                    "dbo.AttemptAnswerUpsert",
                    new { AttemptId = attemptId, QuestionId = questionId, AnswerOptionId = answerOptionId },
                    _connections,
                    token))
                .ConfigureAwait(false);

            return result == 1;
        }, cancellationToken);

    public Task<AnswerSheet?> GetAnswerSheetAsync(
        long attemptId,
        CancellationToken cancellationToken = default) =>
        _resilience.ExecuteAsync(async token =>
        {
            await using SqlConnection connection = _connections.Create();
            await using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(
                SqlCommands.Create("dbo.AttemptAnswerSheet", new { AttemptId = attemptId }, _connections, token))
                .ConfigureAwait(false);

            AnswerSheetHeaderRow? header =
                (await reader.ReadAsync<AnswerSheetHeaderRow>().ConfigureAwait(false)).FirstOrDefault();
            if (header is null)
            {
                return null;
            }

            List<AnswerSheetRowDto> rows =
                (await reader.ReadAsync<AnswerSheetRowDto>().ConfigureAwait(false)).ToList();

            return new AnswerSheet(
                header.AttemptId,
                header.TestId,
                header.ExamCode,
                header.ToScoring(),
                rows.Select(row => new AnswerSheetRow(
                    row.QuestionId,
                    row.SelectedOptionId,
                    row.CorrectOptionId,
                    row.BlockCode,
                    row.BlockName,
                    row.TopicNumber,
                    row.TopicTitle)).ToList());
        }, cancellationToken);

    public Task CompleteAsync(
        long attemptId,
        ScoreBreakdown score,
        DateTimeOffset finishedAt,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(score);

        return _resilience.ExecuteAsync(async token =>
        {
            await using SqlConnection connection = _connections.Create();
            await connection.ExecuteAsync(
                SqlCommands.Create(
                    "dbo.AttemptComplete",
                    new
                    {
                        AttemptId = attemptId,
                        FinishedAt = finishedAt.UtcDateTime,
                        score.TotalQuestions,
                        CorrectCount = score.Correct,
                        IncorrectCount = score.Incorrect,
                        BlankCount = score.Blank,
                        score.RawScore,
                        score.ScaledScore,
                        score.MaxScore,
                        score.PassMark,
                        score.AccuracyPercent,
                        score.Passed,
                    },
                    _connections,
                    token))
                .ConfigureAwait(false);
        }, cancellationToken);
    }

    public Task<PagedResult<AttemptSummary>> GetHistoryAsync(
        string userName,
        string? examCode,
        Paging paging,
        CancellationToken cancellationToken = default) =>
        _resilience.ExecuteAsync(async token =>
        {
            await using SqlConnection connection = _connections.Create();
            await using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(
                SqlCommands.Create(
                    "dbo.AttemptHistory",
                    new
                    {
                        UserName = userName,
                        ExamCode = examCode,
                        Offset = paging.Offset,
                        PageSize = paging.PageSize,
                    },
                    _connections,
                    token))
                .ConfigureAwait(false);

            List<AttemptSummaryRow> rows =
                (await reader.ReadAsync<AttemptSummaryRow>().ConfigureAwait(false)).ToList();
            long total = await reader.ReadFirstAsync<long>().ConfigureAwait(false);

            List<AttemptSummary> summaries = rows
                .Select(row => new AttemptSummary(
                    row.AttemptId,
                    row.TestId,
                    row.TestTitle,
                    row.ExamCode,
                    (TestMode)row.Mode,
                    SqlCommands.ToUtc(row.StartedAt),
                    SqlCommands.ToUtc(row.FinishedAt),
                    row.TotalQuestions,
                    row.Correct,
                    row.Incorrect,
                    row.Blank,
                    row.ScaledScore,
                    row.AccuracyPercent))
                .ToList();

            return PagedResult<AttemptSummary>.From(summaries, paging, total);
        }, cancellationToken);

    public Task<IReadOnlyList<PerformanceSlice>> GetTopicPerformanceAsync(
        string userName,
        string? examCode,
        CancellationToken cancellationToken = default) =>
        _resilience.ExecuteAsync(async token =>
        {
            await using SqlConnection connection = _connections.Create();
            IEnumerable<PerformanceRow> rows = await connection.QueryAsync<PerformanceRow>(
                SqlCommands.Create(
                    "dbo.AttemptTopicPerformance",
                    new { UserName = userName, ExamCode = examCode },
                    _connections,
                    token))
                .ConfigureAwait(false);

            IReadOnlyList<PerformanceSlice> performance = rows
                .Select(row => new PerformanceSlice(
                    row.Key,
                    row.Label,
                    row.TotalQuestions,
                    row.Correct,
                    row.Incorrect,
                    row.Blank,
                    row.AccuracyPercent)
                {
                    BlockCode = row.BlockCode,
                    TopicNumber = row.TopicNumber,
                })
                .ToList();

            return performance;
        }, cancellationToken);

    private static Attempt ToDomain(AttemptRow row) => new()
    {
        Id = row.Id,
        TestId = row.TestId,
        UserName = row.UserName,
        ExamCode = row.ExamCode,
        TestTitle = row.TestTitle,
        Mode = (TestMode)row.Mode,
        StartedAt = SqlCommands.ToUtc(row.StartedAt),
        FinishedAt = SqlCommands.ToUtc(row.FinishedAt),
        Score = row.FinishedAt is null
            ? null
            : new ScoreBreakdown(
                row.TotalQuestions ?? 0,
                row.CorrectCount ?? 0,
                row.IncorrectCount ?? 0,
                row.BlankCount ?? 0,
                row.RawScore ?? 0m,
                row.ScaledScore ?? 0m,
                row.MaxScore ?? 0m,
                row.PassMark ?? 0m,
                row.AccuracyPercent ?? 0m,
                row.Passed ?? false),
    };
}
