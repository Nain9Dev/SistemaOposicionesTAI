using Microsoft.Extensions.Logging;
using Oposiciones.Application.Common;
using Oposiciones.Application.Contracts;
using Oposiciones.Application.Mapping;
using Oposiciones.Domain.Abstractions;
using Oposiciones.Domain.Assessments;
using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Common;
using Oposiciones.Domain.Scoring;

namespace Oposiciones.Application.Services;

/// <summary>Generacion y consulta de tests.</summary>
public interface ITestGenerationService
{
    Task<TestDto> GenerateAsync(GenerateTestRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupera un test. Las soluciones solo se incluyen en modo estudio; en modo examen el
    /// cliente recibe unicamente los enunciados.
    /// </summary>
    Task<TestDto> GetAsync(long testId, CancellationToken cancellationToken = default);
}

/// <inheritdoc />
public sealed class TestGenerationService : ITestGenerationService
{
    private const int MaxTestQuestions = 200;

    private readonly IExamCatalogRepository _catalog;
    private readonly IQuestionRepository _questions;
    private readonly ITestRepository _tests;
    private readonly ILogger<TestGenerationService> _logger;

    public TestGenerationService(
        IExamCatalogRepository catalog,
        IQuestionRepository questions,
        ITestRepository tests,
        ILogger<TestGenerationService> logger)
    {
        _catalog = catalog;
        _questions = questions;
        _tests = tests;
        _logger = logger;
    }

    public async Task<TestDto> GenerateAsync(
        GenerateTestRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ExamProfile exam = await ResolveExamAsync(request, cancellationToken).ConfigureAwait(false);
        TestBlueprint blueprint = BuildBlueprint(request, exam);

        int seed = blueprint.Seed ?? SeededRandom.NewSeed();
        IReadOnlyList<PlannedSection> plan = TestPlanner.Plan(blueprint, exam, seed);

        List<Question> selected = await DrawQuestionsAsync(blueprint, plan, cancellationToken)
            .ConfigureAwait(false);

        if (selected.Count == 0)
        {
            throw new InsufficientQuestionsException(blueprint.TotalQuestions, 0);
        }

        if (selected.Count < blueprint.TotalQuestions)
        {
            if (!request.AllowPartial)
            {
                throw new InsufficientQuestionsException(blueprint.TotalQuestions, selected.Count);
            }

            _logger.LogInformation(
                "Test de {ExamCode} generado con {Selected} de {Requested} preguntas: el banco aun no cubre el reparto solicitado.",
                exam.Code,
                selected.Count,
                blueprint.TotalQuestions);
        }

        var random = new SeededRandom(seed);
        if (blueprint.ShuffleQuestions)
        {
            random.Shuffle(selected);
        }

        if (blueprint.ShuffleOptions)
        {
            for (int i = 0; i < selected.Count; i++)
            {
                selected[i] = selected[i].WithShuffledOptions(random);
            }
        }

        int durationMinutes = blueprint.DurationMinutes ?? ScaleDuration(exam, selected.Count);
        string title = string.IsNullOrWhiteSpace(blueprint.Title)
            ? BuildDefaultTitle(exam, blueprint, selected.Count)
            : blueprint.Title!.Trim();

        GeneratedTest test = await _tests
            .CreateAsync(blueprint, title, seed, durationMinutes, selected, cancellationToken)
            .ConfigureAwait(false);

        return test.ToDto(includeSolutions: test.Mode == TestMode.Study);
    }

    public async Task<TestDto> GetAsync(long testId, CancellationToken cancellationToken = default)
    {
        GeneratedTest? test = await _tests.GetAsync(testId, cancellationToken).ConfigureAwait(false);
        if (test is null)
        {
            throw new NotFoundException("el test", testId);
        }

        return test.ToDto(includeSolutions: test.Mode == TestMode.Study);
    }

    /// <summary>
    /// Extrae las preguntas seccion a seccion evitando repeticiones y, si alguna se queda corta,
    /// reparte el hueco entre las demas secciones del mismo reparto.
    /// <para>
    /// El relleno nunca sale del ambito solicitado: si el opositor acota el test a un tema
    /// concreto, un banco escaso devuelve menos preguntas de ese tema, jamas preguntas de otro.
    /// Con el reparto por defecto, que cubre todos los bloques, esto equivale a compensar un
    /// bloque escaso con el resto del temario.
    /// </para>
    /// </summary>
    private async Task<List<Question>> DrawQuestionsAsync(
        TestBlueprint blueprint,
        IReadOnlyList<PlannedSection> plan,
        CancellationToken cancellationToken)
    {
        var selected = new List<Question>(blueprint.TotalQuestions);
        var seenIds = new HashSet<long>(blueprint.ExcludeQuestionIds);

        async Task DrawIntoAsync(QuestionDraw draw)
        {
            IReadOnlyList<Question> drawn = await _questions
                .DrawAsync(draw with { ExcludeQuestionIds = seenIds.ToArray() }, cancellationToken)
                .ConfigureAwait(false);

            foreach (Question question in drawn)
            {
                if (seenIds.Add(question.Id))
                {
                    selected.Add(question);
                }
            }
        }

        foreach (PlannedSection section in plan)
        {
            await DrawIntoAsync(section.Draw).ConfigureAwait(false);
        }

        // Segunda pasada: las secciones con material de sobra cubren el hueco de las escasas.
        foreach (PlannedSection section in plan)
        {
            int missing = blueprint.TotalQuestions - selected.Count;
            if (missing <= 0)
            {
                break;
            }

            await DrawIntoAsync(section.Draw with
            {
                Count = missing,
                Seed = unchecked(section.Draw.Seed + 104729),
            }).ConfigureAwait(false);
        }

        return selected;
    }

    private async Task<ExamProfile> ResolveExamAsync(
        GenerateTestRequest request,
        CancellationToken cancellationToken)
    {
        string examCode = string.IsNullOrWhiteSpace(request.ExamCode) ? "TAI" : request.ExamCode.Trim();
        ExamProfile? exam = await _catalog.GetExamAsync(examCode, cancellationToken).ConfigureAwait(false);
        return exam ?? throw new NotFoundException("la convocatoria", examCode);
    }

    private static TestBlueprint BuildBlueprint(GenerateTestRequest request, ExamProfile exam)
    {
        var validation = new ValidationBuilder();

        validation.AddIf(
            request.TotalQuestions is < 1 or > MaxTestQuestions,
            nameof(request.TotalQuestions),
            $"El numero de preguntas debe estar entre 1 y {MaxTestQuestions}.");

        validation.AddIf(
            request.DurationMinutes is < 1 or > 600,
            nameof(request.DurationMinutes),
            "La duracion debe estar entre 1 y 600 minutos.");

        var knownBlocks = exam.Blocks
            .Select(block => block.Code)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var sections = new List<BlueprintSection>(request.Sections.Count);
        for (int i = 0; i < request.Sections.Count; i++)
        {
            SectionRequest section = request.Sections[i];
            string field = $"{nameof(request.Sections)}[{i}]";

            string? blockCode = string.IsNullOrWhiteSpace(section.BlockCode) ? null : section.BlockCode.Trim();
            if (blockCode is not null && !knownBlocks.Contains(blockCode))
            {
                validation.Add(field, $"El bloque '{blockCode}' no pertenece a la convocatoria {exam.Code}.");
                continue;
            }

            validation.AddIf(
                section.QuestionCount is < 1,
                field,
                "El numero de preguntas de la seccion debe ser mayor que cero.");

            validation.AddIf(
                section.WeightPercent is <= 0m,
                field,
                "El peso de la seccion debe ser mayor que cero.");

            sections.Add(new BlueprintSection
            {
                BlockCode = blockCode,
                TopicNumber = section.TopicNumber,
                TopicId = section.TopicId,
                QuestionCount = section.QuestionCount,
                WeightPercent = section.WeightPercent,
                Difficulties = QuestionBankService.ParseDifficulties(section.Difficulties, field),
                Tags = QuestionBankService.NormalizeTags(section.Tags),
            });
        }

        validation.ThrowIfInvalid();

        return new TestBlueprint
        {
            ExamCode = exam.Code,
            Title = request.Title,
            Mode = DomainMappings.ParseMode(request.Mode),
            TotalQuestions = request.TotalQuestions,
            Sections = sections,
            Difficulties = QuestionBankService.ParseDifficulties(request.Difficulties, nameof(request.Difficulties)),
            Tags = QuestionBankService.NormalizeTags(request.Tags),
            ShuffleOptions = request.ShuffleOptions,
            ShuffleQuestions = request.ShuffleQuestions,
            Seed = request.Seed,
            DurationMinutes = request.DurationMinutes,
            ExcludeQuestionIds = request.ExcludeQuestionIds.ToArray(),
        };
    }

    /// <summary>
    /// Ajusta la duracion al tamano real del test manteniendo el ritmo del ejercicio oficial
    /// (en TAI, 120 minutos para 85 preguntas presentadas).
    /// </summary>
    private static int ScaleDuration(ExamProfile exam, int questionCount)
    {
        ExamFormat format = exam.Format;
        int reference = format.TotalPresentedQuestions;
        if (reference <= 0 || format.DurationMinutes <= 0)
        {
            return Math.Max(1, questionCount);
        }

        double minutesPerQuestion = format.DurationMinutes / (double)reference;
        return Math.Max(1, (int)Math.Ceiling(minutesPerQuestion * questionCount));
    }

    private static string BuildDefaultTitle(ExamProfile exam, TestBlueprint blueprint, int questionCount)
    {
        string kind = blueprint.Mode == TestMode.Exam ? "Simulacro" : "Test de estudio";
        string scope = blueprint.Sections.Count switch
        {
            0 => "temario completo",
            1 => blueprint.Sections[0].Describe().ToLowerInvariant(),
            _ => $"{blueprint.Sections.Count} secciones",
        };

        return $"{kind} {exam.Code} - {scope} ({questionCount} preguntas)";
    }
}
