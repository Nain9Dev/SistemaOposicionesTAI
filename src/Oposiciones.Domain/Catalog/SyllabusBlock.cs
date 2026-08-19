namespace Oposiciones.Domain.Catalog;

/// <summary>Bloque del temario (I, II, III, IV en TAI) con su peso previsto en el examen.</summary>
public sealed class SyllabusBlock
{
    public int Id { get; init; }

    public int ExamId { get; init; }

    public string ExamCode { get; init; } = string.Empty;

    /// <summary>Codigo del bloque en numeracion romana, tal y como aparece en el programa oficial.</summary>
    public required string Code { get; init; }

    public required string Name { get; init; }

    public int DisplayOrder { get; init; }

    /// <summary>
    /// Peso orientativo del bloque en el ejercicio oficial. Se usa para repartir preguntas
    /// cuando se genera un simulacro sin indicar un reparto explicito.
    /// </summary>
    public decimal ExamWeightPercent { get; init; }

    public IReadOnlyList<SyllabusTopic> Topics { get; init; } = Array.Empty<SyllabusTopic>();
}
