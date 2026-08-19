namespace Oposiciones.Domain.Catalog;

/// <summary>Tema concreto del programa oficial dentro de un bloque.</summary>
public sealed class SyllabusTopic
{
    public int Id { get; init; }

    public int BlockId { get; init; }

    public string BlockCode { get; init; } = string.Empty;

    public string ExamCode { get; init; } = string.Empty;

    /// <summary>Numero del tema en el programa oficial (1..33 en TAI).</summary>
    public int Number { get; init; }

    public required string Title { get; init; }

    /// <summary>Identificador estable y legible, util para enlaces y para importar contenido.</summary>
    public string Slug { get; init; } = string.Empty;

    /// <summary>Normas o estandares de referencia del tema.</summary>
    public IReadOnlyList<string> Keywords { get; init; } = Array.Empty<string>();
}
