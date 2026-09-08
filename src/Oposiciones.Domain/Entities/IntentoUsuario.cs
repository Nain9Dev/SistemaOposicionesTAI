namespace Oposiciones.Domain.Entities;

/// <summary>Resultado consolidado de un simulacro realizado por un usuario registrado.</summary>
public class IntentoUsuario
{
    public const string BloqueTodos = "all";

    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int Aciertos { get; set; }
    public int Fallos { get; set; }
    public int Blancos { get; set; }
    public int Total { get; set; }
    public double Nota { get; set; }
    public string Bloque { get; set; } = BloqueTodos;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    /// <summary>Porcentaje de acierto sobre el total de preguntas del simulacro.</summary>
    public double TasaAcierto => Total > 0 ? Aciertos * 100d / Total : 0d;
}
