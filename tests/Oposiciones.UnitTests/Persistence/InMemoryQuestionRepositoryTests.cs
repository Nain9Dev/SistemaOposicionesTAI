using Microsoft.Extensions.Logging.Abstractions;
using Oposiciones.Domain.Abstractions;
using Oposiciones.Domain.Assessments;
using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Common;
using Oposiciones.Infrastructure.Configuration;
using Oposiciones.Infrastructure.Content;
using Oposiciones.Infrastructure.InMemory;

namespace Oposiciones.UnitTests.Persistence;

/// <summary>
/// Comprueba el proveedor en memoria sobre el contenido real: filtros, paginacion y, sobre todo,
/// que la extraccion por semilla sea reproducible, que es la promesa que sostiene el modo examen.
/// </summary>
public class InMemoryQuestionRepositoryTests
{
    private static readonly InMemoryStore Store = BuildStore();

    private static InMemoryStore BuildStore()
    {
        var loader = new ContentLoader(NullLogger<ContentLoader>.Instance);
        ContentCatalog catalog = loader.Load(
            new ContentOptions { RootPath = "content", FailOnInvalidContent = false },
            AppContext.BaseDirectory);

        return new InMemoryStore(catalog);
    }

    private static InMemoryQuestionRepository Repository() => new(Store);

    [Fact]
    public async Task LaExtraccionConLaMismaSemillaEsReproducible()
    {
        var draw = new QuestionDraw { ExamCode = "TAI", Count = 15, Seed = 4242 };

        IReadOnlyList<Question> primera = await Repository().DrawAsync(draw);
        IReadOnlyList<Question> segunda = await Repository().DrawAsync(draw);

        Assert.Equal(
            primera.Select(question => question.ExternalId),
            segunda.Select(question => question.ExternalId));
    }

    [Fact]
    public async Task SemillasDistintas_ExtraenConjuntosDistintos()
    {
        IReadOnlyList<Question> primera = await Repository()
            .DrawAsync(new QuestionDraw { ExamCode = "TAI", Count = 15, Seed = 1 });
        IReadOnlyList<Question> segunda = await Repository()
            .DrawAsync(new QuestionDraw { ExamCode = "TAI", Count = 15, Seed = 2 });

        Assert.NotEqual(
            primera.Select(question => question.ExternalId),
            segunda.Select(question => question.ExternalId));
    }

    [Fact]
    public async Task ElFiltroPorBloqueSoloDevuelvePreguntasDeEseBloque()
    {
        IReadOnlyList<Question> questions = await Repository()
            .DrawAsync(new QuestionDraw { ExamCode = "TAI", BlockCode = "IV", Count = 10, Seed = 5 });

        Assert.NotEmpty(questions);
        Assert.All(questions, question => Assert.Equal("IV", question.BlockCode));
    }

    [Fact]
    public async Task ElFiltroPorTemaSoloDevuelvePreguntasDeEseTema()
    {
        IReadOnlyList<Question> questions = await Repository()
            .DrawAsync(new QuestionDraw { ExamCode = "TAI", TopicNumber = 31, Count = 10, Seed = 5 });

        Assert.NotEmpty(questions);
        Assert.All(questions, question => Assert.Equal(31, question.TopicNumber));
    }

    [Fact]
    public async Task LasPreguntasExcluidasNoSeExtraenNuncaDeNuevo()
    {
        IReadOnlyList<Question> primera = await Repository()
            .DrawAsync(new QuestionDraw { ExamCode = "TAI", Count = 10, Seed = 8 });

        long[] excluidas = primera.Select(question => question.Id).ToArray();

        IReadOnlyList<Question> segunda = await Repository().DrawAsync(new QuestionDraw
        {
            ExamCode = "TAI",
            Count = 10,
            Seed = 8,
            ExcludeQuestionIds = excluidas,
        });

        Assert.All(segunda, question => Assert.DoesNotContain(question.Id, excluidas));
    }

    [Fact]
    public async Task PedirMasPreguntasDeLasDisponibles_DevuelveLasQueHaySinFallar()
    {
        IReadOnlyList<Question> questions = await Repository()
            .DrawAsync(new QuestionDraw { ExamCode = "TAI", TopicNumber = 11, Count = 500, Seed = 3 });

        int disponibles = await Repository()
            .CountAvailableAsync(new QuestionDraw { ExamCode = "TAI", TopicNumber = 11, Count = 500 });

        Assert.Equal(disponibles, questions.Count);
    }

    [Fact]
    public async Task ConvocatoriaDesconocida_NoDevuelveNada()
    {
        IReadOnlyList<Question> questions = await Repository()
            .DrawAsync(new QuestionDraw { ExamCode = "NO-EXISTE", Count = 10, Seed = 1 });

        Assert.Empty(questions);
    }

    [Fact]
    public async Task LaBusquedaPaginaLosResultadosYReportaElTotal()
    {
        var query = new QuestionQuery { ExamCode = "TAI", Paging = Paging.Of(1, 10) };

        PagedResult<Question> page = await Repository().SearchAsync(query);

        Assert.Equal(10, page.Items.Count);
        Assert.True(page.TotalItems > 100);
        Assert.True(page.HasNext);
        Assert.False(page.HasPrevious);
    }

    [Fact]
    public async Task LaBusquedaPorTextoEncuentraElEnunciado()
    {
        var query = new QuestionQuery { ExamCode = "TAI", Search = "soberanía nacional" };

        PagedResult<Question> page = await Repository().SearchAsync(query);

        Assert.NotEmpty(page.Items);
        Assert.All(page.Items, question => Assert.Equal("I", question.BlockCode));
    }

    [Fact]
    public async Task LaCoberturaIncluyeTodosLosTemasDelPrograma()
    {
        IReadOnlyList<TopicCoverage> coverage = await Repository().GetCoverageAsync("TAI");

        Assert.Equal(33, coverage.Count);
        Assert.All(coverage, topic => Assert.True(topic.ActiveQuestionCount > 0));
    }

    [Fact]
    public async Task BarajarOpcionesMantieneLaRespuestaCorrecta()
    {
        Question original = (await Repository()
            .DrawAsync(new QuestionDraw { ExamCode = "TAI", Count = 1, Seed = 99 })).Single();

        string textoCorrecto = original.CorrectOption!.Text;

        Question barajada = original.WithShuffledOptions(new SeededRandom(1234));

        Assert.Equal(original.Options.Count, barajada.Options.Count);
        Assert.Equal(textoCorrecto, barajada.CorrectOption!.Text);
        Assert.Equal(
            Enumerable.Range(1, barajada.Options.Count),
            barajada.Options.Select(option => (int)option.SortOrder));
    }
}
