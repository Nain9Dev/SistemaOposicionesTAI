using Oposiciones.Domain.Assessments;
using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Common;

namespace Oposiciones.UnitTests.Assessments;

/// <summary>
/// El planificador reparte las preguntas del test. Un reparto que no sume el total pedido produce
/// examenes mas cortos o mas largos de lo solicitado, asi que se comprueba en varios escenarios.
/// </summary>
public class TestPlannerTests
{
    private static ExamProfile BuildExam() => new()
    {
        Code = "TAI",
        Name = "Convocatoria de prueba",
        Blocks = new List<SyllabusBlock>
        {
            new() { Code = "I", Name = "Bloque I", DisplayOrder = 1, ExamWeightPercent = 27.27m },
            new() { Code = "II", Name = "Bloque II", DisplayOrder = 2, ExamWeightPercent = 15.15m },
            new() { Code = "III", Name = "Bloque III", DisplayOrder = 3, ExamWeightPercent = 27.27m },
            new() { Code = "IV", Name = "Bloque IV", DisplayOrder = 4, ExamWeightPercent = 30.31m },
        },
    };

    [Fact]
    public void SinSecciones_RepartePorElPesoOficialDeCadaBloque()
    {
        var blueprint = new TestBlueprint { ExamCode = "TAI", TotalQuestions = 80 };

        IReadOnlyList<PlannedSection> plan = TestPlanner.Plan(blueprint, BuildExam(), seed: 1);

        Assert.Equal(4, plan.Count);
        Assert.Equal(80, plan.Sum(section => section.Draw.Count));

        // 27,27 % de 80 son 21,8 preguntas: el bloque I debe quedar en su entorno.
        int bloqueI = plan.Single(section => section.Draw.BlockCode == "I").Draw.Count;
        Assert.InRange(bloqueI, 21, 22);
    }

    [Fact]
    public void ElRepartoSiempreSumaElTotalPedido_AunConDecimales()
    {
        // 7 preguntas entre cuatro bloques ponderados obliga a repartir restos.
        var blueprint = new TestBlueprint { ExamCode = "TAI", TotalQuestions = 7 };

        IReadOnlyList<PlannedSection> plan = TestPlanner.Plan(blueprint, BuildExam(), seed: 1);

        Assert.Equal(7, plan.Sum(section => section.Draw.Count));
    }

    [Fact]
    public void SeccionesConConteoExplicito_SeRespetanTalCual()
    {
        var blueprint = new TestBlueprint
        {
            ExamCode = "TAI",
            TotalQuestions = 30,
            Sections = new List<BlueprintSection>
            {
                new() { BlockCode = "I", QuestionCount = 10 },
                new() { BlockCode = "IV", QuestionCount = 20 },
            },
        };

        IReadOnlyList<PlannedSection> plan = TestPlanner.Plan(blueprint, BuildExam(), seed: 1);

        Assert.Equal(10, plan.Single(section => section.Draw.BlockCode == "I").Draw.Count);
        Assert.Equal(20, plan.Single(section => section.Draw.BlockCode == "IV").Draw.Count);
    }

    [Fact]
    public void ConteosFijosMasPeso_ElRestoSeRepartePorPorcentaje()
    {
        var blueprint = new TestBlueprint
        {
            ExamCode = "TAI",
            TotalQuestions = 20,
            Sections = new List<BlueprintSection>
            {
                new() { BlockCode = "I", QuestionCount = 8 },
                new() { BlockCode = "II", WeightPercent = 50m },
                new() { BlockCode = "III", WeightPercent = 50m },
            },
        };

        IReadOnlyList<PlannedSection> plan = TestPlanner.Plan(blueprint, BuildExam(), seed: 1);

        Assert.Equal(20, plan.Sum(section => section.Draw.Count));
        Assert.Equal(8, plan.Single(section => section.Draw.BlockCode == "I").Draw.Count);
        Assert.Equal(6, plan.Single(section => section.Draw.BlockCode == "II").Draw.Count);
        Assert.Equal(6, plan.Single(section => section.Draw.BlockCode == "III").Draw.Count);
    }

    [Fact]
    public void ConteosFijosQueSuperanElTotal_SeRechazan()
    {
        var blueprint = new TestBlueprint
        {
            ExamCode = "TAI",
            TotalQuestions = 10,
            Sections = new List<BlueprintSection>
            {
                new() { BlockCode = "I", QuestionCount = 8 },
                new() { BlockCode = "II", QuestionCount = 8 },
            },
        };

        Assert.Throws<DomainException>(() => TestPlanner.Plan(blueprint, BuildExam(), seed: 1));
    }

    [Fact]
    public void CadaSeccionRecibeUnaSemillaDistinta()
    {
        var blueprint = new TestBlueprint { ExamCode = "TAI", TotalQuestions = 40 };

        IReadOnlyList<PlannedSection> plan = TestPlanner.Plan(blueprint, BuildExam(), seed: 99);

        // Si todas compartieran semilla, secciones con el mismo filtro extraerian lo mismo.
        int distintas = plan.Select(section => section.Draw.Seed).Distinct().Count();
        Assert.Equal(plan.Count, distintas);
    }

    [Fact]
    public void TotalMenorQueUno_SeRechaza()
    {
        var blueprint = new TestBlueprint { ExamCode = "TAI", TotalQuestions = 0 };

        Assert.Throws<DomainException>(() => TestPlanner.Plan(blueprint, BuildExam(), seed: 1));
    }

    [Fact]
    public void ConvocatoriaSinPesos_ReparteAPartesIguales()
    {
        var exam = new ExamProfile
        {
            Code = "OTRA",
            Name = "Sin pesos",
            Blocks = new List<SyllabusBlock>
            {
                new() { Code = "A", Name = "A", DisplayOrder = 1 },
                new() { Code = "B", Name = "B", DisplayOrder = 2 },
            },
        };

        IReadOnlyList<PlannedSection> plan = TestPlanner.Plan(
            new TestBlueprint { ExamCode = "OTRA", TotalQuestions = 10 },
            exam,
            seed: 1);

        Assert.All(plan, section => Assert.Equal(5, section.Draw.Count));
    }
}
