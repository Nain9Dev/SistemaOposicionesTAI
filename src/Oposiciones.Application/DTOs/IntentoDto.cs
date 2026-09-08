namespace Oposiciones.Application.DTOs;

/// <summary>
/// Entrada de guardado de un simulacro. Es un DTO propio y no la entidad de dominio para
/// que el cliente no pueda fijar Id ni UsuarioId (over-posting), y para que la nota final
/// la recalcule siempre el servidor.
/// </summary>
public sealed class IntentoDto
{
    public int Aciertos { get; set; }
    public int Fallos { get; set; }
    public int Total { get; set; }
    public string? Bloque { get; set; }
    public DateTime? Fecha { get; set; }
}

/// <summary>Intento ya persistido, con el desglose calculado por el servidor.</summary>
public sealed class IntentoResultadoDto
{
    public int Id { get; init; }
    public int Aciertos { get; init; }
    public int Fallos { get; init; }
    public int Blancos { get; init; }
    public int Total { get; init; }
    public double Nota { get; init; }
    public string Bloque { get; init; } = string.Empty;
    public DateTime Fecha { get; init; }
}
