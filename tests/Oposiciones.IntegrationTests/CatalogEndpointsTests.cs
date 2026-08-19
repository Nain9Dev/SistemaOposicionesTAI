using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Oposiciones.Application.Contracts;

namespace Oposiciones.IntegrationTests;

/// <summary>Endpoints de catalogo y banco de preguntas contra la Api real.</summary>
[Collection("api")]
public class CatalogEndpointsTests
{
    private readonly HttpClient _client;

    public CatalogEndpointsTests(OposicionesApiFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ListaLasConvocatoriasDisponibles()
    {
        var exams = await _client.GetFromJsonAsync<List<ExamSummaryDto>>("/api/exams");

        Assert.NotNull(exams);
        ExamSummaryDto tai = Assert.Single(exams!);
        Assert.Equal("TAI", tai.Code);
        Assert.Equal(4, tai.BlockCount);
        Assert.Equal(33, tai.TopicCount);
        Assert.Equal(80, tai.Format.QuestionCount);
        Assert.Equal(25m, tai.Scoring.PassMark);
    }

    [Fact]
    public async Task DevuelveElTemarioCompletoDeLaConvocatoria()
    {
        var exam = await _client.GetFromJsonAsync<ExamDetailDto>("/api/exams/TAI");

        Assert.NotNull(exam);
        Assert.Equal(4, exam!.Blocks.Count);
        Assert.Equal(33, exam.Blocks.Sum(block => block.Topics.Count));
        Assert.NotNull(exam.Source);
        Assert.Contains("BOE", exam.Source!.Publication, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ElCodigoDeConvocatoriaNoDistingueMayusculas()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/exams/tai");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UnaConvocatoriaInexistente_DevuelveProblemDetails404()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/exams/NO-EXISTE");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        using JsonDocument problem = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(404, problem.RootElement.GetProperty("status").GetInt32());
        Assert.True(problem.RootElement.TryGetProperty("traceId", out _));
    }

    [Fact]
    public async Task FiltraLosTemasPorBloque()
    {
        var topics = await _client.GetFromJsonAsync<List<TopicDto>>("/api/exams/TAI/topics?blockCode=II");

        Assert.NotNull(topics);
        Assert.Equal(5, topics!.Count);
        Assert.All(topics, topic => Assert.Equal("II", topic.BlockCode));
    }

    [Fact]
    public async Task LasRutasDeTemarioDeLaVersionAnteriorSiguenRespondiendo()
    {
        var blocks = await _client.GetFromJsonAsync<List<BlockDto>>("/api/syllabus/blocks");

        Assert.NotNull(blocks);
        Assert.Equal(4, blocks!.Count);

        int blockId = blocks[0].Id;
        var topics = await _client.GetFromJsonAsync<List<TopicDto>>($"/api/syllabus/topics?blockId={blockId}");

        Assert.NotNull(topics);
        Assert.NotEmpty(topics!);
    }

    [Fact]
    public async Task LaRutaVersionadaYLaNoVersionadaDevuelvenLoMismo()
    {
        var conVersion = await _client.GetFromJsonAsync<List<ExamSummaryDto>>("/api/v1/exams");
        var sinVersion = await _client.GetFromJsonAsync<List<ExamSummaryDto>>("/api/exams");

        Assert.Equal(sinVersion!.Count, conVersion!.Count);
        Assert.Equal(sinVersion[0].Code, conVersion[0].Code);
    }

    [Fact]
    public async Task LaCoberturaDelBancoCubreLos33Temas()
    {
        var coverage = await _client.GetFromJsonAsync<BankCoverageDto>("/api/questions/coverage?examCode=TAI");

        Assert.NotNull(coverage);
        Assert.Equal(33, coverage!.Topics.Count);
        Assert.Equal(0, coverage.TopicsWithoutQuestions);
        Assert.True(coverage.ActiveQuestions >= 100);
    }

    [Fact]
    public async Task LaBusquedaDelBancoOcultaLasSolucionesSalvoQueSePidan()
    {
        var sinSoluciones = await _client.GetFromJsonAsync<PagedResponse<QuestionDto>>(
            "/api/questions?examCode=TAI&pageSize=5");

        Assert.NotNull(sinSoluciones);
        Assert.All(sinSoluciones!.Items, question => Assert.Null(question.CorrectOptionId));

        var conSoluciones = await _client.GetFromJsonAsync<PagedResponse<QuestionDto>>(
            "/api/questions?examCode=TAI&pageSize=5&includeSolutions=true");

        Assert.All(conSoluciones!.Items, question =>
        {
            Assert.NotNull(question.CorrectOptionId);
            Assert.NotNull(question.Source);
        });
    }

    [Fact]
    public async Task LaPaginacionSeAcotaAlMaximoAdmitido()
    {
        var page = await _client.GetFromJsonAsync<PagedResponse<QuestionDto>>(
            "/api/questions?examCode=TAI&page=0&pageSize=100000");

        Assert.NotNull(page);
        Assert.Equal(1, page!.Page);
        Assert.True(page.PageSize <= 200);
    }

    [Fact]
    public async Task UnaDificultadFueraDeRango_SeRechazaCon400()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/questions?examCode=TAI&difficulties=9");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using JsonDocument problem = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(problem.RootElement.TryGetProperty("errors", out _));
    }

    [Fact]
    public async Task LaSondaDeDisponibilidadInformaDelProveedorActivo()
    {
        HttpResponseMessage live = await _client.GetAsync("/health/live");
        Assert.Equal(HttpStatusCode.OK, live.StatusCode);

        HttpResponseMessage ready = await _client.GetAsync("/health/ready");
        Assert.Equal(HttpStatusCode.OK, ready.StatusCode);

        using JsonDocument report = JsonDocument.Parse(await ready.Content.ReadAsStringAsync());
        Assert.Equal("Healthy", report.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task CadaRespuestaDevuelveElIdentificadorDeCorrelacion()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/exams");
        request.Headers.Add("X-Correlation-Id", "prueba-12345");

        HttpResponseMessage response = await _client.SendAsync(request);

        Assert.Equal("prueba-12345", response.Headers.GetValues("X-Correlation-Id").Single());
    }
}
