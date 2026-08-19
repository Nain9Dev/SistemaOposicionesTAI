using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Scoring;

namespace Oposiciones.Infrastructure.SqlServer;

/// <summary>
/// Filas planas devueltas por los procedimientos almacenados. Se mantienen separadas del dominio
/// porque Dapper necesita tipos con propiedades asignables, mientras que las entidades del dominio
/// son inmutables.
/// </summary>
internal sealed class ExamRow
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Authority { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? SourceReference { get; set; }

    public string? SourcePublication { get; set; }

    public string? SourceUrl { get; set; }

    public decimal CorrectPoints { get; set; }

    public decimal IncorrectPoints { get; set; }

    public decimal BlankPoints { get; set; }

    public decimal MaxScore { get; set; }

    public decimal PassMark { get; set; }

    public int QuestionCount { get; set; }

    public int ReserveQuestions { get; set; }

    public int DurationMinutes { get; set; }

    public int OptionsPerQuestion { get; set; }

    public bool IsActive { get; set; }

    public OfficialSource? ToSource() =>
        string.IsNullOrWhiteSpace(SourceReference)
            ? null
            : new OfficialSource(SourceReference, SourcePublication, SourceUrl);

    public ScoringPolicy ToScoring() =>
        new(CorrectPoints, IncorrectPoints, BlankPoints, MaxScore, PassMark);

    public ExamFormat ToFormat() =>
        new(QuestionCount, ReserveQuestions, DurationMinutes, OptionsPerQuestion);
}

internal sealed class BlockRow
{
    public int Id { get; set; }

    public int ExamId { get; set; }

    public string ExamCode { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public decimal ExamWeightPercent { get; set; }
}

internal sealed class TopicRow
{
    public int Id { get; set; }

    public int BlockId { get; set; }

    public string BlockCode { get; set; } = string.Empty;

    public string ExamCode { get; set; } = string.Empty;

    public int Number { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Keywords { get; set; } = string.Empty;

    public SyllabusTopic ToDomain() => new()
    {
        Id = Id,
        BlockId = BlockId,
        BlockCode = BlockCode,
        ExamCode = ExamCode,
        Number = Number,
        Title = Title,
        Slug = Slug,
        Keywords = SplitList(Keywords),
    };

    internal static IReadOnlyList<string> SplitList(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? Array.Empty<string>()
            : value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

internal sealed class QuestionRow
{
    public long Id { get; set; }

    public string ExternalId { get; set; } = string.Empty;

    public int TopicId { get; set; }

    public string ExamCode { get; set; } = string.Empty;

    public string BlockCode { get; set; } = string.Empty;

    public int TopicNumber { get; set; }

    public string TopicTitle { get; set; } = string.Empty;

    public byte Difficulty { get; set; }

    public string Statement { get; set; } = string.Empty;

    public string? Explanation { get; set; }

    public string? SourceReference { get; set; }

    public string? SourcePublication { get; set; }

    public string? SourceUrl { get; set; }

    public bool IsActive { get; set; }

    public string? Tags { get; set; }

    public Question ToDomain(IReadOnlyList<AnswerOption> options) => new()
    {
        Id = Id,
        ExternalId = ExternalId,
        TopicId = TopicId,
        ExamCode = ExamCode,
        BlockCode = BlockCode,
        TopicNumber = TopicNumber,
        TopicTitle = TopicTitle,
        Difficulty = (Difficulty)Difficulty,
        Statement = Statement,
        Explanation = Explanation,
        Source = string.IsNullOrWhiteSpace(SourceReference)
            ? null
            : new OfficialSource(SourceReference, SourcePublication, SourceUrl),
        Tags = TopicRow.SplitList(Tags),
        IsActive = IsActive,
        Options = options,
    };
}

internal sealed class OptionRow
{
    public long QuestionId { get; set; }

    public long Id { get; set; }

    public byte SortOrder { get; set; }

    public string OptionText { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}

internal sealed class TestRow
{
    public long Id { get; set; }

    public string ExamCode { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public byte Mode { get; set; }

    public int Seed { get; set; }

    public int DurationMinutes { get; set; }

    public int TotalQuestions { get; set; }

    public decimal CorrectPoints { get; set; }

    public decimal IncorrectPoints { get; set; }

    public decimal BlankPoints { get; set; }

    public decimal MaxScore { get; set; }

    public decimal PassMark { get; set; }

    public DateTime CreatedAt { get; set; }

    public ScoringPolicy ToScoring() =>
        new(CorrectPoints, IncorrectPoints, BlankPoints, MaxScore, PassMark);
}

internal sealed class AttemptRow
{
    public long Id { get; set; }

    public long TestId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string ExamCode { get; set; } = string.Empty;

    public string TestTitle { get; set; } = string.Empty;

    public byte Mode { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }

    public int? TotalQuestions { get; set; }

    public int? CorrectCount { get; set; }

    public int? IncorrectCount { get; set; }

    public int? BlankCount { get; set; }

    public decimal? RawScore { get; set; }

    public decimal? ScaledScore { get; set; }

    public decimal? MaxScore { get; set; }

    public decimal? PassMark { get; set; }

    public decimal? AccuracyPercent { get; set; }

    public bool? Passed { get; set; }
}

internal sealed class AnswerSheetHeaderRow
{
    public long AttemptId { get; set; }

    public long TestId { get; set; }

    public string ExamCode { get; set; } = string.Empty;

    public decimal CorrectPoints { get; set; }

    public decimal IncorrectPoints { get; set; }

    public decimal BlankPoints { get; set; }

    public decimal MaxScore { get; set; }

    public decimal PassMark { get; set; }

    public ScoringPolicy ToScoring() =>
        new(CorrectPoints, IncorrectPoints, BlankPoints, MaxScore, PassMark);
}

internal sealed class AnswerSheetRowDto
{
    public long QuestionId { get; set; }

    public long? SelectedOptionId { get; set; }

    public long? CorrectOptionId { get; set; }

    public string BlockCode { get; set; } = string.Empty;

    public string BlockName { get; set; } = string.Empty;

    public int TopicNumber { get; set; }

    public string TopicTitle { get; set; } = string.Empty;
}

internal sealed class AttemptSummaryRow
{
    public long AttemptId { get; set; }

    public long TestId { get; set; }

    public string TestTitle { get; set; } = string.Empty;

    public string ExamCode { get; set; } = string.Empty;

    public byte Mode { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }

    public int TotalQuestions { get; set; }

    public int Correct { get; set; }

    public int Incorrect { get; set; }

    public int Blank { get; set; }

    public decimal? ScaledScore { get; set; }

    public decimal? AccuracyPercent { get; set; }
}

internal sealed class PerformanceRow
{
    public string Key { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public string BlockCode { get; set; } = string.Empty;

    public int TopicNumber { get; set; }

    public int TotalQuestions { get; set; }

    public int Correct { get; set; }

    public int Incorrect { get; set; }

    public int Blank { get; set; }

    public decimal AccuracyPercent { get; set; }
}
