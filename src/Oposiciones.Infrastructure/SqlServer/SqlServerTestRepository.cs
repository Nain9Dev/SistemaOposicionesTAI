using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;
using Oposiciones.Domain.Abstractions;
using Oposiciones.Domain.Assessments;
using Oposiciones.Domain.Catalog;

namespace Oposiciones.Infrastructure.SqlServer;

/// <summary>Tests generados, persistidos en SQL Server.</summary>
public sealed class SqlServerTestRepository : ITestRepository
{
    private static readonly JsonSerializerOptions BlueprintJsonOptions =
        new(JsonSerializerDefaults.Web);

    private readonly ISqlConnectionFactory _connections;
    private readonly SqlResilience _resilience;
    private readonly IExamCatalogRepository _catalog;

    public SqlServerTestRepository(
        ISqlConnectionFactory connections,
        SqlResilience resilience,
        IExamCatalogRepository catalog)
    {
        _connections = connections;
        _resilience = resilience;
        _catalog = catalog;
    }

    public async Task<GeneratedTest> CreateAsync(
        TestBlueprint blueprint,
        string title,
        int seed,
        int durationMinutes,
        IReadOnlyList<Question> questions,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(blueprint);
        ArgumentNullException.ThrowIfNull(questions);

        ExamProfile? exam = await _catalog.GetExamAsync(blueprint.ExamCode, cancellationToken)
            .ConfigureAwait(false);

        Domain.Scoring.ScoringPolicy scoring = blueprint.ScoringOverride
            ?? exam?.Scoring
            ?? Domain.Scoring.ScoringPolicy.Default;

        DataTable questionTable = BuildQuestionTable(questions);
        DataTable optionTable = BuildOptionTable(questions);

        GeneratedTest? created = await _resilience.ExecuteAsync(async token =>
        {
            var parameters = new DynamicParameters();
            parameters.Add("ExamCode", blueprint.ExamCode);
            parameters.Add("Title", title);
            parameters.Add("Mode", (byte)blueprint.Mode);
            parameters.Add("Seed", seed);
            parameters.Add("DurationMinutes", durationMinutes);
            parameters.Add("CorrectPoints", scoring.CorrectPoints);
            parameters.Add("IncorrectPoints", scoring.IncorrectPoints);
            parameters.Add("BlankPoints", scoring.BlankPoints);
            parameters.Add("MaxScore", scoring.ScaleMaxScore);
            parameters.Add("PassMark", scoring.PassMark);
            parameters.Add("BlueprintJson", JsonSerializer.Serialize(blueprint, BlueprintJsonOptions));
            parameters.Add("Questions", questionTable.AsTableValuedParameter("dbo.TestQuestionList"));
            parameters.Add("Options", optionTable.AsTableValuedParameter("dbo.TestQuestionOptionList"));

            await using SqlConnection connection = _connections.Create();
            await using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(
                SqlCommands.Create("dbo.TestCreate", parameters, _connections, token))
                .ConfigureAwait(false);

            return await ReadTestAsync(reader).ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);

        return created
            ?? throw new InvalidOperationException("SQL Server no ha devuelto el test recien creado.");
    }

    public Task<GeneratedTest?> GetAsync(long testId, CancellationToken cancellationToken = default) =>
        _resilience.ExecuteAsync(async token =>
        {
            await using SqlConnection connection = _connections.Create();
            await using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(
                SqlCommands.Create("dbo.TestGet", new { TestId = testId }, _connections, token))
                .ConfigureAwait(false);

            return await ReadTestAsync(reader).ConfigureAwait(false);
        }, cancellationToken);

    private static async Task<GeneratedTest?> ReadTestAsync(SqlMapper.GridReader reader)
    {
        TestRow? header = (await reader.ReadAsync<TestRow>().ConfigureAwait(false)).FirstOrDefault();
        if (header is null)
        {
            return null;
        }

        IReadOnlyList<Question> questions = await SqlServerQuestionRepository
            .ReadQuestionsAsync(reader)
            .ConfigureAwait(false);

        return new GeneratedTest
        {
            Id = header.Id,
            ExamCode = header.ExamCode,
            Title = header.Title,
            Mode = (TestMode)header.Mode,
            Seed = header.Seed,
            DurationMinutes = header.DurationMinutes,
            Scoring = header.ToScoring(),
            CreatedAt = SqlCommands.ToUtc(header.CreatedAt),
            Questions = questions,
        };
    }

    private static DataTable BuildQuestionTable(IReadOnlyList<Question> questions)
    {
        var table = new DataTable();
        table.Columns.Add("QuestionId", typeof(long));
        table.Columns.Add("SortOrder", typeof(int));

        for (int i = 0; i < questions.Count; i++)
        {
            table.Rows.Add(questions[i].Id, i + 1);
        }

        return table;
    }

    /// <summary>
    /// Guarda el orden barajado de las opciones. Sin esto, la revision posterior mostraria las
    /// opciones en otro orden del que vio el opositor y las capturas del examen dejarian de cuadrar.
    /// </summary>
    private static DataTable BuildOptionTable(IReadOnlyList<Question> questions)
    {
        var table = new DataTable();
        table.Columns.Add("QuestionId", typeof(long));
        table.Columns.Add("AnswerOptionId", typeof(long));
        table.Columns.Add("SortOrder", typeof(byte));

        foreach (Question question in questions)
        {
            foreach (AnswerOption option in question.Options)
            {
                table.Rows.Add(question.Id, option.Id, option.SortOrder);
            }
        }

        return table;
    }
}
