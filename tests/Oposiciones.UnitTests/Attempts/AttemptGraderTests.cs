using Oposiciones.Domain.Attempts;
using Oposiciones.Domain.Scoring;

namespace Oposiciones.UnitTests.Attempts;

/// <summary>
/// La correccion no solo produce una nota: produce el desglose por bloque y tema que el opositor
/// usa para decidir que repasar. Se comprueban ambas cosas.
/// </summary>
public class AttemptGraderTests
{
    private static AnswerSheetRow Row(string block, int topic, long? selected, long? correct) =>
        new(
            QuestionId: topic * 100 + (selected ?? 0),
            SelectedOptionId: selected,
            CorrectOptionId: correct,
            BlockCode: block,
            BlockName: $"Bloque {block}",
            TopicNumber: topic,
            TopicTitle: $"Tema {topic}");

    [Fact]
    public void CuentaAciertosFallosYBlancosPorSeparado()
    {
        var sheet = new AnswerSheet(1, 10, "TAI", ScoringPolicy.Default, new List<AnswerSheetRow>
        {
            Row("I", 1, selected: 1, correct: 1),
            Row("I", 1, selected: 2, correct: 3),
            Row("I", 1, selected: null, correct: 4),
        });

        AttemptResult result = AttemptGrader.Grade(sheet, sheet.Scoring, DateTimeOffset.UnixEpoch);

        Assert.Equal(1, result.Score.Correct);
        Assert.Equal(1, result.Score.Incorrect);
        Assert.Equal(1, result.Score.Blank);
        Assert.Equal(3, result.Score.TotalQuestions);
    }

    [Fact]
    public void DesglosaElRendimientoPorBloqueYPorTema()
    {
        var sheet = new AnswerSheet(1, 10, "TAI", ScoringPolicy.Default, new List<AnswerSheetRow>
        {
            Row("I", 1, selected: 1, correct: 1),
            Row("I", 1, selected: 9, correct: 1),
            Row("I", 2, selected: 1, correct: 1),
            Row("IV", 30, selected: 9, correct: 1),
        });

        AttemptResult result = AttemptGrader.Grade(sheet, sheet.Scoring, DateTimeOffset.UnixEpoch);

        PerformanceSlice bloqueI = result.ByBlock.Single(slice => slice.Key == "I");
        Assert.Equal(3, bloqueI.TotalQuestions);
        Assert.Equal(2, bloqueI.Correct);
        Assert.Equal(66.67m, bloqueI.AccuracyPercent);

        Assert.Equal(3, result.ByTopic.Count);
        PerformanceSlice tema1 = result.ByTopic.Single(slice => slice.Key == "I.1");
        Assert.Equal(50m, tema1.AccuracyPercent);
        Assert.Equal("I", tema1.BlockCode);
        Assert.Equal(1, tema1.TopicNumber);
    }

    [Fact]
    public void LosTemasMasDebiles_SeOrdenanPorPeorAcierto()
    {
        var sheet = new AnswerSheet(1, 10, "TAI", ScoringPolicy.Default, new List<AnswerSheetRow>
        {
            Row("I", 1, selected: 1, correct: 1),
            Row("II", 10, selected: 9, correct: 1),
            Row("III", 20, selected: 9, correct: 1),
            Row("III", 20, selected: 1, correct: 1),
        });

        AttemptResult result = AttemptGrader.Grade(sheet, sheet.Scoring, DateTimeOffset.UnixEpoch);

        List<PerformanceSlice> debiles = result.WeakestTopics().ToList();

        Assert.Equal("II.10", debiles[0].Key);
        Assert.Equal(0m, debiles[0].AccuracyPercent);
        Assert.Equal("III.20", debiles[1].Key);
    }

    [Fact]
    public void UnaPreguntaSinRespuestaCorrectaRegistrada_NoCuentaComoAcierto()
    {
        // Salvaguarda ante datos incompletos: sin opcion correcta conocida, marcar cualquier
        // opcion debe contar como fallo y nunca como acierto.
        var sheet = new AnswerSheet(1, 10, "TAI", ScoringPolicy.Default, new List<AnswerSheetRow>
        {
            Row("I", 1, selected: 5, correct: null),
        });

        AttemptResult result = AttemptGrader.Grade(sheet, sheet.Scoring, DateTimeOffset.UnixEpoch);

        Assert.Equal(0, result.Score.Correct);
        Assert.Equal(1, result.Score.Incorrect);
    }

    [Fact]
    public void ConservaLaFechaDeCierreYLaConvocatoria()
    {
        var finishedAt = new DateTimeOffset(2026, 5, 23, 10, 30, 0, TimeSpan.Zero);
        var sheet = new AnswerSheet(7, 70, "TAI", ScoringPolicy.Default, new List<AnswerSheetRow>
        {
            Row("I", 1, selected: 1, correct: 1),
        });

        AttemptResult result = AttemptGrader.Grade(sheet, sheet.Scoring, finishedAt);

        Assert.Equal(7, result.AttemptId);
        Assert.Equal(70, result.TestId);
        Assert.Equal("TAI", result.ExamCode);
        Assert.Equal(finishedAt, result.FinishedAt);
    }
}
