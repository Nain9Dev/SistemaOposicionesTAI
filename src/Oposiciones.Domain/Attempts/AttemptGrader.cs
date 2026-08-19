using Oposiciones.Domain.Scoring;

namespace Oposiciones.Domain.Attempts;

/// <summary>
/// Corrige una hoja de respuestas aplicando el baremo de la convocatoria y desglosa el resultado
/// por bloque y por tema. Logica pura: la misma correccion se obtiene con SQL Server, en memoria
/// o en una prueba unitaria.
/// </summary>
public static class AttemptGrader
{
    public static AttemptResult Grade(AnswerSheet sheet, ScoringPolicy policy, DateTimeOffset finishedAt)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(policy);

        int correct = 0;
        int incorrect = 0;
        int blank = 0;

        foreach (AnswerSheetRow row in sheet.Rows)
        {
            if (row.IsBlank)
            {
                blank++;
            }
            else if (row.IsCorrect)
            {
                correct++;
            }
            else
            {
                incorrect++;
            }
        }

        ScoreBreakdown score = policy.Evaluate(correct, incorrect, blank);

        IReadOnlyList<PerformanceSlice> byBlock = Slice(
            sheet.Rows,
            row => row.BlockCode,
            row => string.IsNullOrWhiteSpace(row.BlockName) ? $"Bloque {row.BlockCode}" : row.BlockName,
            topicLevel: false);

        IReadOnlyList<PerformanceSlice> byTopic = Slice(
            sheet.Rows,
            row => $"{row.BlockCode}.{row.TopicNumber}",
            row => $"Tema {row.TopicNumber}. {row.TopicTitle}",
            topicLevel: true);

        return new AttemptResult(
            sheet.AttemptId,
            sheet.TestId,
            sheet.ExamCode,
            finishedAt,
            score,
            byBlock,
            byTopic);
    }

    private static IReadOnlyList<PerformanceSlice> Slice(
        IReadOnlyList<AnswerSheetRow> rows,
        Func<AnswerSheetRow, string> keySelector,
        Func<AnswerSheetRow, string> labelSelector,
        bool topicLevel)
    {
        return rows
            .GroupBy(keySelector, StringComparer.Ordinal)
            .Select(group =>
            {
                AnswerSheetRow first = group.First();
                int total = group.Count();
                int correct = group.Count(row => row.IsCorrect);
                int incorrect = group.Count(row => row.IsIncorrect);
                int blank = group.Count(row => row.IsBlank);
                decimal accuracy = total == 0
                    ? 0m
                    : Math.Round(correct / (decimal)total * 100m, 2, MidpointRounding.AwayFromZero);

                return new PerformanceSlice(
                    group.Key,
                    labelSelector(first),
                    total,
                    correct,
                    incorrect,
                    blank,
                    accuracy)
                {
                    BlockCode = first.BlockCode,
                    TopicNumber = topicLevel ? first.TopicNumber : null,
                };
            })
            .OrderBy(slice => slice.Key, StringComparer.Ordinal)
            .ToList();
    }
}
