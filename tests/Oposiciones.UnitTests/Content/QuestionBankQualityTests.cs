using Microsoft.Extensions.Logging.Abstractions;
using Oposiciones.Domain.Catalog;
using Oposiciones.Infrastructure.Configuration;
using Oposiciones.Infrastructure.Content;

namespace Oposiciones.UnitTests.Content;

/// <summary>
/// Controla la calidad del contenido real del repositorio, no de un contenido de prueba.
/// <para>
/// Es la red de seguridad del banco de preguntas: cada vez que se anaden preguntas nuevas, estas
/// comprobaciones impiden que se cuele una sin respuesta correcta, sin fuente oficial o apuntando
/// a un tema que no existe. Sin esto, un error de contenido solo se descubre en pleno estudio.
/// </para>
/// </summary>
public class QuestionBankQualityTests
{
    private static readonly ContentCatalog Catalog = LoadRealContent();

    private static ContentCatalog LoadRealContent()
    {
        var loader = new ContentLoader(NullLogger<ContentLoader>.Instance);
        var options = new ContentOptions { RootPath = "content", FailOnInvalidContent = false };
        return loader.Load(options, AppContext.BaseDirectory);
    }

    [Fact]
    public void ElContenidoSeCargaSinIncidencias()
    {
        Assert.Empty(Catalog.Issues);
    }

    [Fact]
    public void LaConvocatoriaTaiTieneCuatroBloquesYTreintaYTresTemas()
    {
        ExamProfile exam = Assert.Single(Catalog.Exams);

        Assert.Equal("TAI", exam.Code);
        Assert.Equal(4, exam.Blocks.Count);
        Assert.Equal(33, exam.AllTopics().Count());
        Assert.Equal(new[] { "I", "II", "III", "IV" }, exam.Blocks.Select(block => block.Code));
    }

    [Fact]
    public void LaNumeracionDeTemasEsCorrelativaDeUnoATreintaYTres()
    {
        int[] numeros = Catalog.Exams
            .SelectMany(exam => exam.AllTopics())
            .Select(topic => topic.Number)
            .OrderBy(number => number)
            .ToArray();

        Assert.Equal(Enumerable.Range(1, 33), numeros);
    }

    [Fact]
    public void ElBaremoDeclaradoEsElDelEjercicioOficial()
    {
        ExamProfile exam = Catalog.Exams.Single();

        Assert.Equal(1m, exam.Scoring.CorrectPoints);
        Assert.Equal(50m, exam.Scoring.ScaleMaxScore);
        Assert.Equal(25m, exam.Scoring.PassMark);
        Assert.Equal(80, exam.Format.QuestionCount);
        Assert.Equal(5, exam.Format.ReserveQuestions);
        Assert.Equal(120, exam.Format.DurationMinutes);

        // La penalizacion oficial es un tercio del valor de un acierto.
        Assert.True(Math.Abs(exam.Scoring.IncorrectPoints + (1m / 3m)) < 0.001m);
    }

    [Fact]
    public void CadaPreguntaTieneExactamenteUnaRespuestaCorrecta()
    {
        var defectuosas = Catalog.Questions
            .Where(question => question.Options.Count(option => option.IsCorrect) != 1)
            .Select(question => question.ExternalId)
            .ToList();

        Assert.Empty(defectuosas);
    }

    [Fact]
    public void CadaPreguntaOfreceCuatroOpcionesDeRespuesta()
    {
        // El ejercicio oficial del TAI presenta cuatro alternativas por pregunta.
        var incorrectas = Catalog.Questions
            .Where(question => question.Options.Count != 4)
            .Select(question => $"{question.ExternalId} ({question.Options.Count} opciones)")
            .ToList();

        Assert.Empty(incorrectas);
    }

    [Fact]
    public void CadaPreguntaCitaSuFuenteOficialYExplicaLaRespuesta()
    {
        var sinFuente = Catalog.Questions
            .Where(question => question.Source is null || string.IsNullOrWhiteSpace(question.Source.Reference))
            .Select(question => question.ExternalId)
            .ToList();

        var sinExplicacion = Catalog.Questions
            .Where(question => string.IsNullOrWhiteSpace(question.Explanation))
            .Select(question => question.ExternalId)
            .ToList();

        Assert.Empty(sinFuente);
        Assert.Empty(sinExplicacion);
    }

    [Fact]
    public void LosIdentificadoresExternosSonUnicos()
    {
        var duplicados = Catalog.Questions
            .GroupBy(question => question.ExternalId, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        Assert.Empty(duplicados);
    }

    [Fact]
    public void TodosLosTemasDelProgramaTienenAlMenosUnaPregunta()
    {
        var conPreguntas = Catalog.Questions.Select(question => question.TopicId).ToHashSet();

        var vacios = Catalog.Exams
            .SelectMany(exam => exam.AllTopics())
            .Where(topic => !conPreguntas.Contains(topic.Id))
            .Select(topic => $"Tema {topic.Number}")
            .ToList();

        Assert.Empty(vacios);
    }

    [Fact]
    public void LasDificultadesEstanDentroDelRangoAdmitido()
    {
        Assert.All(Catalog.Questions, question => Assert.InRange((int)question.Difficulty, 1, 5));
    }

    [Fact]
    public void NingunaPreguntaRepiteElTextoDeSusOpciones()
    {
        var repetidas = Catalog.Questions
            .Where(question => question.Options
                .Select(option => option.Text)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count() != question.Options.Count)
            .Select(question => question.ExternalId)
            .ToList();

        Assert.Empty(repetidas);
    }

    [Fact]
    public void ElBancoCubreLosCuatroBloquesConVolumenSuficienteParaGenerarUnTest()
    {
        var porBloque = Catalog.Questions
            .GroupBy(question => question.BlockCode)
            .ToDictionary(group => group.Key, group => group.Count());

        Assert.Equal(4, porBloque.Count);
        Assert.All(porBloque, entry => Assert.True(
            entry.Value >= 20,
            $"El bloque {entry.Key} solo tiene {entry.Value} preguntas."));
    }
}
