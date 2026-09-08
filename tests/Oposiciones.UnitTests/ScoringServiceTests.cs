using Oposiciones.Application.Services;
using Xunit;

namespace Oposiciones.UnitTests;

/// <summary>
/// Baremo oficial INAP: +1,00 por acierto, -0,33 por fallo, 0,00 en blanco.
/// Es la regla de negocio con mas impacto para el usuario, asi que se fija por contrato.
/// </summary>
public class ScoringServiceTests
{
    private readonly ScoringService _scoring = new();

    [Fact]
    public void Examen_perfecto_puntua_diez()
    {
        var score = _scoring.Evaluate(total: 20, correct: 20, wrong: 0);

        Assert.Equal(10.0d, score.Grade);
        Assert.Equal(0, score.Blank);
    }

    [Fact]
    public void Cada_fallo_descuenta_un_tercio_de_acierto()
    {
        var score = _scoring.Evaluate(total: 10, correct: 8, wrong: 2);

        // (8 * 1,00) - (2 * 0,33) = 7,34 puntos netos sobre 10 posibles.
        Assert.Equal(7.34d, score.NetPoints);
        Assert.Equal(7.34d, score.Grade);
    }

    [Fact]
    public void Las_preguntas_en_blanco_no_penalizan()
    {
        var conBlancos = _scoring.Evaluate(total: 10, correct: 5, wrong: 0);
        var sinBlancos = _scoring.Evaluate(total: 5, correct: 5, wrong: 0);

        Assert.Equal(5, conBlancos.Blank);
        Assert.Equal(5.0d, conBlancos.Grade);
        Assert.Equal(10.0d, sinBlancos.Grade);
    }

    [Fact]
    public void La_nota_nunca_es_negativa()
    {
        var score = _scoring.Evaluate(total: 10, correct: 0, wrong: 10);

        Assert.Equal(0d, score.Grade);
        Assert.True(score.NetPoints < 0, "los puntos netos si pueden ser negativos");
    }

    [Fact]
    public void Un_examen_sin_preguntas_no_produce_NaN()
    {
        // La division por cero devolvia NaN y se propagaba hasta la interfaz.
        var score = _scoring.Evaluate(total: 0, correct: 0, wrong: 0);

        Assert.Equal(0d, score.Grade);
        Assert.False(double.IsNaN(score.Grade));
    }

    [Theory]
    [InlineData(10, 20, 0)]   // mas aciertos que preguntas
    [InlineData(10, 5, 20)]   // aciertos + fallos por encima del total
    [InlineData(10, -5, -5)]  // contadores negativos
    public void Los_contadores_incoherentes_se_saturan_al_total(int total, int correct, int wrong)
    {
        var score = _scoring.Evaluate(total, correct, wrong);

        Assert.InRange(score.Correct, 0, total);
        Assert.InRange(score.Wrong, 0, total);
        Assert.InRange(score.Blank, 0, total);
        Assert.Equal(total, score.Correct + score.Wrong + score.Blank);
        Assert.InRange(score.Grade, 0d, 10d);
    }

    [Fact]
    public void La_penalizacion_publicada_coincide_con_el_baremo()
    {
        Assert.Equal(0.33d, _scoring.PenaltyPerWrongAnswer);
    }
}
