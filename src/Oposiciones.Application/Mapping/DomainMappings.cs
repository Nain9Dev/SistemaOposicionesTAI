using Oposiciones.Application.Contracts;
using Oposiciones.Domain.Abstractions;
using Oposiciones.Domain.Assessments;
using Oposiciones.Domain.Attempts;
using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Common;
using Oposiciones.Domain.Scoring;

namespace Oposiciones.Application.Mapping;

/// <summary>
/// Conversion entre el dominio y los contratos publicos. Se hace a mano y en un unico sitio:
/// el contrato de la Api puede evolucionar sin arrastrar al dominio, y viceversa.
/// </summary>
public static class DomainMappings
{
    public static OfficialSourceDto? ToDto(this OfficialSource? source) =>
        source is null ? null : new OfficialSourceDto(source.Reference, source.Publication, source.Url);

    public static ScoringDto ToDto(this ScoringPolicy policy) => new(
        Math.Round(policy.CorrectPoints, 4, MidpointRounding.AwayFromZero),
        Math.Round(policy.IncorrectPoints, 4, MidpointRounding.AwayFromZero),
        Math.Round(policy.BlankPoints, 4, MidpointRounding.AwayFromZero),
        policy.ScaleMaxScore,
        policy.PassMark);

    public static ExamFormatDto ToDto(this ExamFormat format) => new(
        format.QuestionCount,
        format.ReserveQuestions,
        format.DurationMinutes,
        format.OptionsPerQuestion);

    public static ExamSummaryDto ToSummaryDto(this ExamProfile exam) => new(
        exam.Code,
        exam.Name,
        exam.Authority,
        string.IsNullOrWhiteSpace(exam.Description) ? null : exam.Description,
        exam.OfficialSource.ToDto(),
        exam.Scoring.ToDto(),
        exam.Format.ToDto(),
        exam.Blocks.Count,
        exam.Blocks.Sum(block => block.Topics.Count));

    public static ExamDetailDto ToDetailDto(this ExamProfile exam) => new(
        exam.Code,
        exam.Name,
        exam.Authority,
        string.IsNullOrWhiteSpace(exam.Description) ? null : exam.Description,
        exam.OfficialSource.ToDto(),
        exam.Scoring.ToDto(),
        exam.Format.ToDto(),
        exam.Blocks.Select(block => block.ToDto()).ToList());

    public static BlockDto ToDto(this SyllabusBlock block) => new(
        block.Id,
        block.Code,
        block.Name,
        block.DisplayOrder,
        block.ExamWeightPercent,
        block.Topics.Select(topic => topic.ToDto()).ToList());

    public static TopicDto ToDto(this SyllabusTopic topic) => new(
        topic.Id,
        topic.ExamCode,
        topic.BlockCode,
        topic.Number,
        topic.Title,
        topic.Slug,
        topic.Keywords);

    /// <summary>
    /// Proyecta una pregunta al contrato publico. Con <paramref name="includeSolution"/> en falso
    /// la respuesta correcta, la explicacion y la fuente no salen del servidor: es la unica
    /// garantia real de que un simulacro no se puede resolver leyendo la respuesta HTTP.
    /// </summary>
    public static QuestionDto ToDto(this Question question, bool includeSolution) => new(
        question.Id,
        question.ExternalId,
        question.ExamCode,
        question.BlockCode,
        question.TopicNumber,
        question.TopicTitle,
        (int)question.Difficulty,
        question.Statement,
        question.Options
            .OrderBy(option => option.SortOrder)
            .Select(option => new OptionDto(option.Id, option.SortOrder, option.Text))
            .ToList(),
        includeSolution ? question.CorrectOption?.Id : null,
        includeSolution ? question.Explanation : null,
        includeSolution ? question.Source.ToDto() : null,
        question.Tags);

    public static TestDto ToDto(this GeneratedTest test, bool includeSolutions) => new(
        test.Id,
        test.ExamCode,
        test.Title,
        test.Mode.ToWireValue(),
        test.Seed,
        test.DurationMinutes,
        test.TotalQuestions,
        test.Scoring.ToDto(),
        test.CreatedAt,
        test.Questions.Select(question => question.ToDto(includeSolutions)).ToList());

    public static ScoreDto ToDto(this ScoreBreakdown score) => new(
        score.TotalQuestions,
        score.Correct,
        score.Incorrect,
        score.Blank,
        score.RawScore,
        score.ScaledScore,
        score.MaxScore,
        score.PassMark,
        score.AccuracyPercent,
        score.Passed);

    public static AttemptDto ToDto(this Attempt attempt) => new(
        attempt.Id,
        attempt.TestId,
        attempt.UserName,
        attempt.ExamCode,
        attempt.TestTitle,
        attempt.Mode.ToWireValue(),
        attempt.StartedAt,
        attempt.FinishedAt,
        attempt.Duration is null ? null : (int)attempt.Duration.Value.TotalSeconds,
        attempt.Score?.ToDto());

    public static PerformanceDto ToDto(this PerformanceSlice slice) => new(
        slice.Key,
        slice.Label,
        slice.TotalQuestions,
        slice.Correct,
        slice.Incorrect,
        slice.Blank,
        slice.AccuracyPercent);

    public static AttemptResultDto ToDto(this AttemptResult result) => new(
        result.AttemptId,
        result.TestId,
        result.ExamCode,
        result.FinishedAt,
        result.Score.ToDto(),
        result.ByBlock.Select(ToDto).ToList(),
        result.ByTopic.Select(ToDto).ToList(),
        result.WeakestTopics().Select(ToDto).ToList());

    public static AttemptSummaryDto ToDto(this AttemptSummary summary) => new(
        summary.AttemptId,
        summary.TestId,
        summary.TestTitle,
        summary.ExamCode,
        summary.Mode.ToWireValue(),
        summary.StartedAt,
        summary.FinishedAt,
        summary.TotalQuestions,
        summary.Correct,
        summary.Incorrect,
        summary.Blank,
        summary.ScaledScore,
        summary.AccuracyPercent);

    public static TopicCoverageDto ToDto(this TopicCoverage coverage) => new(
        coverage.TopicId,
        coverage.BlockCode,
        coverage.BlockName,
        coverage.TopicNumber,
        coverage.TopicTitle,
        coverage.QuestionCount,
        coverage.ActiveQuestionCount,
        coverage.AverageDifficulty);

    public static PagedResponse<TOut> ToResponse<TIn, TOut>(
        this PagedResult<TIn> result,
        Func<TIn, TOut> selector)
    {
        PagedResult<TOut> mapped = result.Map(selector);
        return new PagedResponse<TOut>(
            mapped.Items,
            mapped.Page,
            mapped.PageSize,
            mapped.TotalItems,
            mapped.TotalPages,
            mapped.HasNext);
    }

    /// <summary>Representacion textual del modo usada en el contrato HTTP.</summary>
    public static string ToWireValue(this TestMode mode) => mode switch
    {
        TestMode.Exam => "exam",
        _ => "study",
    };

    /// <summary>Interpreta el modo recibido del cliente; cualquier valor desconocido cae en estudio.</summary>
    public static TestMode ParseMode(string? mode) =>
        string.Equals(mode, "exam", StringComparison.OrdinalIgnoreCase) ? TestMode.Exam : TestMode.Study;
}
