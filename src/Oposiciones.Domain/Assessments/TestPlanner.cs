using Oposiciones.Domain.Catalog;
using Oposiciones.Domain.Common;

namespace Oposiciones.Domain.Assessments;

/// <summary>
/// Traduce un <see cref="TestBlueprint"/> declarativo en extracciones concretas contra el banco
/// de preguntas. Es logica pura y sin dependencias, de modo que el reparto de un simulacro puede
/// verificarse en pruebas unitarias sin base de datos.
/// </summary>
public static class TestPlanner
{
    /// <summary>
    /// Resuelve el reparto de preguntas del test.
    /// </summary>
    /// <param name="blueprint">Receta solicitada.</param>
    /// <param name="exam">Convocatoria, de la que se toman los pesos oficiales por bloque.</param>
    /// <param name="seed">Semilla efectiva ya resuelta.</param>
    public static IReadOnlyList<PlannedSection> Plan(TestBlueprint blueprint, ExamProfile exam, int seed)
    {
        ArgumentNullException.ThrowIfNull(blueprint);
        ArgumentNullException.ThrowIfNull(exam);

        if (blueprint.TotalQuestions < 1)
        {
            throw new DomainException("El test debe contener al menos una pregunta.");
        }

        IReadOnlyList<BlueprintSection> sections = blueprint.Sections.Count > 0
            ? blueprint.Sections
            : BuildDefaultSections(exam);

        int[] counts = Distribute(sections, blueprint.TotalQuestions);

        var planned = new List<PlannedSection>(sections.Count);
        for (int i = 0; i < sections.Count; i++)
        {
            if (counts[i] <= 0)
            {
                continue;
            }

            BlueprintSection section = sections[i];
            var draw = new QuestionDraw
            {
                ExamCode = blueprint.ExamCode,
                BlockCode = section.BlockCode,
                TopicNumber = section.TopicNumber,
                TopicId = section.TopicId,
                Difficulties = section.Difficulties.Count > 0 ? section.Difficulties : blueprint.Difficulties,
                Tags = section.Tags.Count > 0 ? section.Tags : blueprint.Tags,
                Count = counts[i],
                // Cada seccion desplaza la semilla para no extraer el mismo subconjunto en todas.
                Seed = unchecked(seed + (i * 7919)),
                ExcludeQuestionIds = blueprint.ExcludeQuestionIds,
            };

            planned.Add(new PlannedSection(section, draw));
        }

        if (planned.Count == 0)
        {
            throw new DomainException("El reparto solicitado no asigna preguntas a ninguna seccion.");
        }

        return planned;
    }

    /// <summary>
    /// Reparte <paramref name="total"/> preguntas entre las secciones.
    /// <para>
    /// Las secciones con conteo explicito se respetan tal cual. El resto se reparte por
    /// porcentaje, y los decimales sobrantes se asignan por el metodo del resto mayor para que
    /// la suma cuadre exactamente con el total pedido.
    /// </para>
    /// </summary>
    private static int[] Distribute(IReadOnlyList<BlueprintSection> sections, int total)
    {
        var counts = new int[sections.Count];
        var weighted = new List<int>();
        decimal weightSum = 0m;
        int assigned = 0;

        for (int i = 0; i < sections.Count; i++)
        {
            BlueprintSection section = sections[i];
            if (section.QuestionCount is > 0)
            {
                counts[i] = section.QuestionCount.Value;
                assigned += counts[i];
                continue;
            }

            // Sin conteo ni peso explicito, la seccion participa a partes iguales.
            decimal weight = section.WeightPercent ?? 1m;
            if (weight <= 0m)
            {
                continue;
            }

            weighted.Add(i);
            weightSum += weight;
        }

        if (assigned > total)
        {
            throw new DomainException(
                $"Las secciones con conteo fijo suman {assigned} preguntas y el test solo admite {total}.");
        }

        int remaining = total - assigned;
        if (weighted.Count == 0 || remaining <= 0)
        {
            return counts;
        }

        var remainders = new List<(int Index, decimal Remainder)>(weighted.Count);
        int distributed = 0;

        foreach (int index in weighted)
        {
            decimal weight = sections[index].WeightPercent ?? 1m;
            decimal exact = remaining * weight / weightSum;
            int whole = (int)Math.Floor(exact);

            counts[index] += whole;
            distributed += whole;
            remainders.Add((index, exact - whole));
        }

        // Resto mayor: los huecos que deja el redondeo hacia abajo van a las mayores fracciones.
        foreach ((int index, _) in remainders.OrderByDescending(r => r.Remainder).ThenBy(r => r.Index))
        {
            if (distributed >= remaining)
            {
                break;
            }

            counts[index]++;
            distributed++;
        }

        return counts;
    }

    /// <summary>
    /// Reparto por defecto: una seccion por bloque, ponderada con el peso oficial declarado en la
    /// convocatoria. Reproduce la proporcion real del ejercicio sin pedir nada al cliente.
    /// </summary>
    private static IReadOnlyList<BlueprintSection> BuildDefaultSections(ExamProfile exam)
    {
        if (exam.Blocks.Count == 0)
        {
            return new[] { new BlueprintSection() };
        }

        bool hasWeights = exam.Blocks.Any(block => block.ExamWeightPercent > 0m);

        return exam.Blocks
            .OrderBy(block => block.DisplayOrder)
            .Select(block => new BlueprintSection
            {
                BlockCode = block.Code,
                WeightPercent = hasWeights ? block.ExamWeightPercent : 1m,
            })
            .ToList();
    }
}

/// <summary>Seccion ya resuelta: que pidio el opositor y que extraccion concreta le corresponde.</summary>
public sealed record PlannedSection(BlueprintSection Section, QuestionDraw Draw);
