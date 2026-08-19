using System.Net;
using System.Net.Http.Json;
using Oposiciones.Application.Contracts;

namespace Oposiciones.IntegrationTests;

/// <summary>
/// Recorre el ciclo completo del opositor: generar un test, responderlo, corregirlo y revisarlo.
/// Es la prueba que garantiza que las piezas encajan de verdad y no solo por separado.
/// </summary>
[Collection("api")]
public class ExamFlowTests
{
    private readonly HttpClient _client;

    public ExamFlowTests(OposicionesApiFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _client = factory.CreateClient();
    }

    private async Task<TestDto> GenerateAsync(object request)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/tests/generate", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<TestDto>())!;
    }

    private async Task<long> CorrectOptionAsync(long questionId)
    {
        var question = await _client.GetFromJsonAsync<QuestionDto>(
            $"/api/questions/{questionId}?includeSolution=true");
        return question!.CorrectOptionId!.Value;
    }

    [Fact]
    public async Task SinCuerpoDePeticion_GeneraUnSimulacroPorDefecto()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync<object?>("/api/tests/generate", null);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        TestDto test = (await response.Content.ReadFromJsonAsync<TestDto>())!;

        Assert.Equal("TAI", test.ExamCode);
        Assert.Equal(20, test.TotalQuestions);
        Assert.Equal("study", test.Mode);
    }

    [Fact]
    public async Task ElModoExamenNoRevelaLasSolucionesYElModoEstudioSi()
    {
        TestDto examen = await GenerateAsync(new { examCode = "TAI", mode = "exam", totalQuestions = 10 });
        Assert.All(examen.Questions, question =>
        {
            Assert.Null(question.CorrectOptionId);
            Assert.Null(question.Explanation);
        });

        TestDto estudio = await GenerateAsync(new { examCode = "TAI", mode = "study", totalQuestions = 10 });
        Assert.All(estudio.Questions, question =>
        {
            Assert.NotNull(question.CorrectOptionId);
            Assert.NotNull(question.Explanation);
        });
    }

    [Fact]
    public async Task LaMismaSemillaGeneraExactamenteElMismoTest()
    {
        var peticion = new { examCode = "TAI", mode = "exam", totalQuestions = 25, seed = 987654 };

        TestDto primero = await GenerateAsync(peticion);
        TestDto segundo = await GenerateAsync(peticion);

        Assert.Equal(
            primero.Questions.Select(question => question.ExternalId),
            segundo.Questions.Select(question => question.ExternalId));

        // El orden de las opciones tambien debe reproducirse, no solo el conjunto de preguntas.
        Assert.Equal(
            primero.Questions[0].Options.Select(option => option.Text),
            segundo.Questions[0].Options.Select(option => option.Text));
    }

    [Fact]
    public async Task ElRepartoPorSeccionesSeRespetaAlGenerar()
    {
        TestDto test = await GenerateAsync(new
        {
            examCode = "TAI",
            mode = "study",
            totalQuestions = 12,
            sections = new object[]
            {
                new { blockCode = "I", questionCount = 4 },
                new { blockCode = "IV", questionCount = 8 },
            },
        });

        Assert.Equal(12, test.TotalQuestions);
        Assert.Equal(4, test.Questions.Count(question => question.BlockCode == "I"));
        Assert.Equal(8, test.Questions.Count(question => question.BlockCode == "IV"));
    }

    [Fact]
    public async Task PuedeAcotarseUnTestAUnUnicoTema()
    {
        TestDto test = await GenerateAsync(new
        {
            examCode = "TAI",
            totalQuestions = 5,
            sections = new object[] { new { blockCode = "I", topicNumber = 1 } },
        });

        Assert.NotEmpty(test.Questions);
        Assert.All(test.Questions, question =>
        {
            Assert.Equal("I", question.BlockCode);
            Assert.Equal(1, question.TopicNumber);
        });
    }

    [Fact]
    public async Task UnBloqueInexistente_SeRechazaCon400()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/tests/generate", new
        {
            examCode = "TAI",
            sections = new object[] { new { blockCode = "ZZ" } },
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SinPermitirTestsParciales_ElBancoInsuficienteDevuelve409()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/tests/generate", new
        {
            examCode = "TAI",
            totalQuestions = 100,
            allowPartial = false,
            sections = new object[] { new { blockCode = "II", topicNumber = 11 } },
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task CicloCompleto_LaNotaAplicaElBaremoOficial()
    {
        TestDto test = await GenerateAsync(new { examCode = "TAI", mode = "exam", totalQuestions = 12, seed = 4321 });

        HttpResponseMessage started = await _client.PostAsJsonAsync(
            "/api/attempts/start",
            new { testId = test.TestId, userName = "integracion" });
        Assert.Equal(HttpStatusCode.Created, started.StatusCode);

        AttemptDto attempt = (await started.Content.ReadFromJsonAsync<AttemptDto>())!;

        // 6 aciertos, 3 fallos y 3 preguntas sin contestar.
        for (int i = 0; i < 9; i++)
        {
            QuestionDto question = test.Questions[i];
            long correct = await CorrectOptionAsync(question.Id);
            long chosen = i < 6
                ? correct
                : question.Options.First(option => option.Id != correct).Id;

            HttpResponseMessage answered = await _client.PostAsJsonAsync(
                $"/api/attempts/{attempt.AttemptId}/answer",
                new { questionId = question.Id, answerOptionId = chosen });

            Assert.Equal(HttpStatusCode.OK, answered.StatusCode);
        }

        var result = await _client.PostAsJsonAsync<object?>(
            $"/api/attempts/{attempt.AttemptId}/finish", null);
        AttemptResultDto correction = (await result.Content.ReadFromJsonAsync<AttemptResultDto>())!;

        Assert.Equal(12, correction.Score.TotalQuestions);
        Assert.Equal(6, correction.Score.Correct);
        Assert.Equal(3, correction.Score.Incorrect);
        Assert.Equal(3, correction.Score.Blank);

        // 6 - 3/3 = 5 puntos brutos sobre 12; escalado sobre 50 son 20,833.
        Assert.Equal(5m, correction.Score.RawScore);
        Assert.Equal(20.833m, correction.Score.ScaledScore);
        Assert.False(correction.Score.Passed);
        Assert.NotEmpty(correction.ByBlock);
    }

    [Fact]
    public async Task LaRevisionConSolucionesSoloEstaDisponibleTrasFinalizar()
    {
        TestDto test = await GenerateAsync(new { examCode = "TAI", mode = "exam", totalQuestions = 5 });

        HttpResponseMessage started = await _client.PostAsJsonAsync(
            "/api/attempts/start",
            new { testId = test.TestId, userName = "revision" });
        AttemptDto attempt = (await started.Content.ReadFromJsonAsync<AttemptDto>())!;

        HttpResponseMessage prematura = await _client.GetAsync($"/api/attempts/{attempt.AttemptId}/review");
        Assert.Equal(HttpStatusCode.BadRequest, prematura.StatusCode);

        await _client.PostAsJsonAsync<object?>($"/api/attempts/{attempt.AttemptId}/finish", null);

        var revision = await _client.GetFromJsonAsync<TestDto>($"/api/attempts/{attempt.AttemptId}/review");
        Assert.All(revision!.Questions, question =>
        {
            Assert.NotNull(question.CorrectOptionId);
            Assert.NotNull(question.Explanation);
            Assert.NotNull(question.Source);
        });
    }

    [Fact]
    public async Task FinalizarDosVeces_DevuelveLaMismaCorreccion()
    {
        TestDto test = await GenerateAsync(new { examCode = "TAI", totalQuestions = 5 });
        HttpResponseMessage started = await _client.PostAsJsonAsync(
            "/api/attempts/start",
            new { testId = test.TestId, userName = "idempotente" });
        AttemptDto attempt = (await started.Content.ReadFromJsonAsync<AttemptDto>())!;

        var primera = await (await _client.PostAsJsonAsync<object?>(
            $"/api/attempts/{attempt.AttemptId}/finish", null)).Content.ReadFromJsonAsync<AttemptResultDto>();
        var segunda = await (await _client.PostAsJsonAsync<object?>(
            $"/api/attempts/{attempt.AttemptId}/finish", null)).Content.ReadFromJsonAsync<AttemptResultDto>();

        Assert.Equal(primera!.Score.ScaledScore, segunda!.Score.ScaledScore);
        Assert.Equal(primera.FinishedAt, segunda.FinishedAt);
    }

    [Fact]
    public async Task UnIntentoFinalizadoNoAdmiteMasRespuestas()
    {
        TestDto test = await GenerateAsync(new { examCode = "TAI", totalQuestions = 3 });
        HttpResponseMessage started = await _client.PostAsJsonAsync(
            "/api/attempts/start",
            new { testId = test.TestId, userName = "cerrado" });
        AttemptDto attempt = (await started.Content.ReadFromJsonAsync<AttemptDto>())!;

        await _client.PostAsJsonAsync<object?>($"/api/attempts/{attempt.AttemptId}/finish", null);

        HttpResponseMessage tardia = await _client.PostAsJsonAsync(
            $"/api/attempts/{attempt.AttemptId}/answer",
            new { questionId = test.Questions[0].Id, answerOptionId = test.Questions[0].Options[0].Id });

        Assert.Equal(HttpStatusCode.BadRequest, tardia.StatusCode);
    }

    [Fact]
    public async Task UnaOpcionQueNoPerteneceALaPregunta_SeRechaza()
    {
        TestDto test = await GenerateAsync(new { examCode = "TAI", totalQuestions = 3 });
        HttpResponseMessage started = await _client.PostAsJsonAsync(
            "/api/attempts/start",
            new { testId = test.TestId, userName = "tramposo" });
        AttemptDto attempt = (await started.Content.ReadFromJsonAsync<AttemptDto>())!;

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            $"/api/attempts/{attempt.AttemptId}/answer",
            new { questionId = test.Questions[0].Id, answerOptionId = 999_999_999L });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DejarUnaPreguntaEnBlancoEsUnaRespuestaValida()
    {
        TestDto test = await GenerateAsync(new { examCode = "TAI", totalQuestions = 3 });
        HttpResponseMessage started = await _client.PostAsJsonAsync(
            "/api/attempts/start",
            new { testId = test.TestId, userName = "enblanco" });
        AttemptDto attempt = (await started.Content.ReadFromJsonAsync<AttemptDto>())!;

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            $"/api/attempts/{attempt.AttemptId}/answer",
            new { questionId = test.Questions[0].Id, answerOptionId = (long?)null });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ElHistorialYElPlanDeRepasoRecogenLosIntentosCerrados()
    {
        const string usuario = "historial-nain";

        TestDto test = await GenerateAsync(new { examCode = "TAI", totalQuestions = 6, seed = 555 });
        HttpResponseMessage started = await _client.PostAsJsonAsync(
            "/api/attempts/start",
            new { testId = test.TestId, userName = usuario });
        AttemptDto attempt = (await started.Content.ReadFromJsonAsync<AttemptDto>())!;

        foreach (QuestionDto question in test.Questions)
        {
            long correct = await CorrectOptionAsync(question.Id);
            await _client.PostAsJsonAsync(
                $"/api/attempts/{attempt.AttemptId}/answer",
                new { questionId = question.Id, answerOptionId = correct });
        }

        await _client.PostAsJsonAsync<object?>($"/api/attempts/{attempt.AttemptId}/finish", null);

        var historial = await _client.GetFromJsonAsync<PagedResponse<AttemptSummaryDto>>(
            $"/api/attempts/history?userName={usuario}");

        Assert.Equal(1, historial!.TotalItems);
        Assert.Equal(6, historial.Items[0].Correct);

        var plan = await _client.GetFromJsonAsync<StudyPlanDto>(
            $"/api/attempts/study-plan?userName={usuario}&examCode=TAI");

        Assert.Equal(100m, plan!.OverallAccuracyPercent);

        // Con todo acertado no hay nada que reforzar.
        Assert.Empty(plan.Recommendations);
    }

    [Fact]
    public async Task UnIntentoInexistente_Devuelve404()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/attempts/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
