using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Oposiciones.Domain.Catalog;
using Oposiciones.Infrastructure.Configuration;
using Oposiciones.Infrastructure.Content;

namespace Oposiciones.UnitTests.Content;

/// <summary>
/// El cargador es la puerta de entrada del contenido. Estas pruebas verifican que rechaza lo que
/// esta mal en lugar de dejarlo pasar, que es lo que evita que un banco corrupto llegue a servirse.
/// </summary>
public class ContentLoaderTests : IDisposable
{
    private readonly string _root;

    public ContentLoaderTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "oposiciones-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(_root, "exams"));
        Directory.CreateDirectory(Path.Combine(_root, "questions"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }

        GC.SuppressFinalize(this);
    }

    private void WriteExam(object exam) => Write(Path.Combine("exams", "exam.json"), exam);

    private void WriteQuestions(object bank) => Write(Path.Combine("questions", "bank.json"), bank);

    private void Write(string relativePath, object payload) =>
        File.WriteAllText(
            Path.Combine(_root, relativePath),
            JsonSerializer.Serialize(payload, new JsonSerializerOptions(JsonSerializerDefaults.Web)));

    private ContentCatalog Load(bool failOnInvalid = false)
    {
        var loader = new ContentLoader(NullLogger<ContentLoader>.Instance);
        return loader.Load(
            new ContentOptions { RootPath = _root, FailOnInvalidContent = failOnInvalid },
            AppContext.BaseDirectory);
    }

    private static object MinimalExam() => new
    {
        code = "TEST",
        name = "Convocatoria de prueba",
        blocks = new[]
        {
            new
            {
                code = "I",
                name = "Bloque unico",
                displayOrder = 1,
                examWeightPercent = 100,
                topics = new[] { new { number = 1, title = "Tema unico" } },
            },
        },
    };

    private static object Question(string id, int correctIndex = 0, int topicNumber = 1) => new
    {
        id,
        blockCode = "I",
        topicNumber,
        difficulty = 2,
        statement = $"Enunciado de {id}",
        options = new[] { "A", "B", "C", "D" },
        correctIndex,
        explanation = "Porque si",
    };

    [Fact]
    public void CargaCorrectamenteUnContenidoValido()
    {
        WriteExam(MinimalExam());
        WriteQuestions(new { examCode = "TEST", questions = new[] { Question("Q-001") } });

        ContentCatalog catalog = Load();

        Assert.Empty(catalog.Issues);
        Assert.Single(catalog.Exams);
        Question question = Assert.Single(catalog.Questions);
        Assert.Equal("Q-001", question.ExternalId);
        Assert.True(question.Options.Single(option => option.IsCorrect).SortOrder == 1);
    }

    [Fact]
    public void LosIdentificadoresSonDeterministasEntreCargas()
    {
        WriteExam(MinimalExam());
        WriteQuestions(new { examCode = "TEST", questions = new[] { Question("Q-002"), Question("Q-001") } });

        long[] primera = Load().Questions.Select(question => question.Id).ToArray();
        long[] segunda = Load().Questions.Select(question => question.Id).ToArray();

        Assert.Equal(primera, segunda);
    }

    [Fact]
    public void RechazaUnaPreguntaCuyoTemaNoExisteEnElTemario()
    {
        WriteExam(MinimalExam());
        WriteQuestions(new { examCode = "TEST", questions = new[] { Question("Q-001", topicNumber: 99) } });

        ContentCatalog catalog = Load();

        Assert.Empty(catalog.Questions);
        Assert.Contains(catalog.Issues, issue => issue.Contains("no existe el tema", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void RechazaUnIndiceDeRespuestaCorrectaFueraDeRango()
    {
        WriteExam(MinimalExam());
        WriteQuestions(new { examCode = "TEST", questions = new[] { Question("Q-001", correctIndex: 7) } });

        ContentCatalog catalog = Load();

        Assert.Empty(catalog.Questions);
        Assert.Contains(catalog.Issues, issue => issue.Contains("correctIndex", StringComparison.Ordinal));
    }

    [Fact]
    public void RechazaIdentificadoresDuplicados()
    {
        WriteExam(MinimalExam());
        WriteQuestions(new { examCode = "TEST", questions = new[] { Question("Q-001"), Question("Q-001") } });

        ContentCatalog catalog = Load();

        Assert.Single(catalog.Questions);
        Assert.Contains(catalog.Issues, issue => issue.Contains("duplicado", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void RechazaOpcionesDeRespuestaRepetidas()
    {
        WriteExam(MinimalExam());
        WriteQuestions(new
        {
            examCode = "TEST",
            questions = new[]
            {
                new
                {
                    id = "Q-001",
                    blockCode = "I",
                    topicNumber = 1,
                    difficulty = 2,
                    statement = "Enunciado",
                    options = new[] { "A", "A", "C", "D" },
                    correctIndex = 0,
                },
            },
        });

        ContentCatalog catalog = Load();

        Assert.Empty(catalog.Questions);
        Assert.Contains(catalog.Issues, issue => issue.Contains("repetidas", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ConFailOnInvalidContent_AbortaLaCargaEnLugarDeServirDatosIncompletos()
    {
        WriteExam(MinimalExam());
        WriteQuestions(new { examCode = "TEST", questions = new[] { Question("Q-001", correctIndex: 9) } });

        Assert.Throws<InvalidOperationException>(() => Load(failOnInvalid: true));
    }

    [Fact]
    public void UnJsonMalFormado_SeReportaSinTumbarElResto()
    {
        WriteExam(MinimalExam());
        WriteQuestions(new { examCode = "TEST", questions = new[] { Question("Q-001") } });
        File.WriteAllText(Path.Combine(_root, "questions", "roto.json"), "{ esto no es json");

        ContentCatalog catalog = Load();

        Assert.Single(catalog.Questions);
        Assert.Contains(catalog.Issues, issue => issue.Contains("JSON invalido", StringComparison.Ordinal));
    }

    [Fact]
    public void GeneraSlugAsciiCuandoNoSeDeclaraExpresamente()
    {
        WriteExam(new
        {
            code = "TEST",
            name = "Convocatoria",
            blocks = new[]
            {
                new
                {
                    code = "I",
                    name = "Bloque",
                    displayOrder = 1,
                    examWeightPercent = 100,
                    topics = new[] { new { number = 1, title = "Administración electrónica y protección de datos" } },
                },
            },
        });
        WriteQuestions(new { examCode = "TEST", questions = Array.Empty<object>() });

        SyllabusTopic topic = Load().Exams.Single().AllTopics().Single();

        Assert.Equal("administracion-electronica-y-proteccion-de-datos", topic.Slug);
    }

    [Theory]
    [InlineData("Diseño de sistemas", "diseno-de-sistemas")]
    [InlineData("La Constitución Española de 1978", "la-constitucion-espanola-de-1978")]
    [InlineData("Redes TCP/IP: direccionamiento", "redes-tcp-ip-direccionamiento")]
    [InlineData("   ", "")]
    public void ElSlugSiempreEsAsciiYSinSeparadoresSobrantes(string title, string expected)
    {
        Assert.Equal(expected, ContentLoader.Slugify(title));
    }

    [Fact]
    public void CarpetaInexistente_DevuelveCatalogoVacioSinExcepcion()
    {
        var loader = new ContentLoader(NullLogger<ContentLoader>.Instance);

        ContentCatalog catalog = loader.Load(
            new ContentOptions { RootPath = Path.Combine(_root, "no-existe") },
            AppContext.BaseDirectory);

        Assert.Empty(catalog.Exams);
        Assert.Empty(catalog.Questions);
    }
}
