using Oposiciones.Domain.Scoring;

namespace Oposiciones.UnitTests.Scoring;

/// <summary>
/// El baremo es la pieza que decide la nota del opositor, de modo que se prueba contra los
/// numeros del ejercicio oficial y no solo con casos sinteticos.
/// </summary>
public class ScoringPolicyTests
{
    [Fact]
    public void BaremoOficial_AplicaUnTercioDePenalizacionPorFallo()
    {
        ScoreBreakdown score = ScoringPolicy.Default.Evaluate(correct: 60, incorrect: 12, blank: 8);

        // 60 aciertos - 12 fallos / 3 = 56 puntos brutos sobre un maximo de 80.
        Assert.Equal(56m, score.RawScore);
        Assert.Equal(80, score.TotalQuestions);

        // La nota se escala sobre 50: 56/80 * 50 = 35.
        Assert.Equal(35m, score.ScaledScore);
        Assert.True(score.Passed);
    }

    [Fact]
    public void RespuestasEnBlanco_NoPenalizan()
    {
        ScoreBreakdown todoEnBlanco = ScoringPolicy.Default.Evaluate(correct: 0, incorrect: 0, blank: 80);

        Assert.Equal(0m, todoEnBlanco.RawScore);
        Assert.Equal(0m, todoEnBlanco.ScaledScore);
        Assert.False(todoEnBlanco.Passed);
    }

    [Fact]
    public void DejarUnaPreguntaEnBlanco_EsMejorQueFallarla()
    {
        ScoreBreakdown enBlanco = ScoringPolicy.Default.Evaluate(correct: 40, incorrect: 0, blank: 40);
        ScoreBreakdown fallando = ScoringPolicy.Default.Evaluate(correct: 40, incorrect: 40, blank: 0);

        Assert.True(enBlanco.ScaledScore > fallando.ScaledScore);
    }

    [Fact]
    public void NotaNegativa_SeAcotaACero()
    {
        // Contestar al azar puede dar bruto negativo; la nota publicada nunca baja de cero.
        ScoreBreakdown score = ScoringPolicy.Default.Evaluate(correct: 5, incorrect: 75, blank: 0);

        Assert.Equal(0m, score.RawScore);
        Assert.Equal(0m, score.ScaledScore);
        Assert.False(score.Passed);
    }

    [Fact]
    public void JustoEnElAprobado_SeConsideraSuperado()
    {
        // 40 aciertos sobre 80 sin fallos: 40/80 * 50 = 25, que es exactamente la nota de corte.
        ScoreBreakdown score = ScoringPolicy.Default.Evaluate(correct: 40, incorrect: 0, blank: 40);

        Assert.Equal(25m, score.ScaledScore);
        Assert.True(score.Passed);
    }

    [Fact]
    public void BaremoSinPenalizacion_IgnoraLosFallos()
    {
        ScoreBreakdown score = ScoringPolicy.NoPenalty.Evaluate(correct: 15, incorrect: 5, blank: 0);

        Assert.Equal(15m, score.RawScore);
        Assert.Equal(75m, score.ScaledScore);
        Assert.Equal(75m, score.AccuracyPercent);
    }

    [Fact]
    public void EjercicioVacio_NoRompeElCalculo()
    {
        ScoreBreakdown score = ScoringPolicy.Default.Evaluate(0, 0, 0);

        Assert.Equal(0, score.TotalQuestions);
        Assert.Equal(0m, score.ScaledScore);
        Assert.Equal(0m, score.AccuracyPercent);
    }

    [Fact]
    public void BaremoPersonalizado_SeAplicaTalCualSeConfigura()
    {
        // Una convocatoria distinta puede penalizar un cuarto y calificar sobre 100.
        var policy = new ScoringPolicy(
            CorrectPoints: 1m,
            IncorrectPoints: -0.25m,
            BlankPoints: 0m,
            ScaleMaxScore: 100m,
            PassMark: 50m);

        ScoreBreakdown score = policy.Evaluate(correct: 50, incorrect: 20, blank: 30);

        Assert.Equal(45m, score.RawScore);
        Assert.Equal(45m, score.ScaledScore);
        Assert.False(score.Passed);
    }

    [Theory]
    [InlineData(-1, 0, 0)]
    [InlineData(0, -1, 0)]
    [InlineData(0, 0, -1)]
    public void RecuentosNegativos_SeRechazan(int correct, int incorrect, int blank)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => ScoringPolicy.Default.Evaluate(correct, incorrect, blank));
    }
}
