namespace Oposiciones.Application.DTOs;

/// <summary>
/// Contrato que consume el motor de estudio del cliente. Los nombres se mantienen en el
/// idioma del contrato publico ya existente para no romper a los clientes desplegados.
/// </summary>
public sealed class PreguntaDto
{
    public long Id { get; init; }
    public string Enunciado { get; init; } = string.Empty;
    public IReadOnlyList<string> Opciones { get; init; } = [];

    /// <summary>Indice (base 0) de la opcion correcta dentro de <see cref="Opciones"/>.</summary>
    public int RespuestaCorrecta { get; init; }

    /// <summary>Codigo del bloque (I..IV). Contrato historico del cliente.</summary>
    public string Bloque { get; init; } = string.Empty;

    public int BloqueId { get; init; }
    public string BloqueNombre { get; init; } = string.Empty;

    public int TemaId { get; init; }
    public string Tema { get; init; } = string.Empty;

    public int Dificultad { get; init; }
    public string? Explicacion { get; init; }
}
