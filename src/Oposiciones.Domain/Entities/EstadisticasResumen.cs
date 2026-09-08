namespace Oposiciones.Domain.Entities;

/// <summary>Rendimiento agregado de un bloque del temario.</summary>
public class BloqueRendimiento
{
    public string Bloque { get; set; } = string.Empty;
    public int Intentos { get; set; }
    public int Aciertos { get; set; }
    public int Fallos { get; set; }
    public int Total { get; set; }
    public double Precision { get; set; }
    public double NotaMedia { get; set; }
}

/// <summary>
/// Resumen de progreso del usuario. Los campos historicos se mantienen para no romper el
/// contrato del cliente; el detalle adicional se expone de forma aditiva.
/// </summary>
public class EstadisticasResumen
{
    public int TotalPreguntas { get; set; }
    public int Aciertos { get; set; }
    public int Fallos { get; set; }
    public int Blancos { get; set; }
    public double NotaMedia { get; set; }

    public int TotalIntentos { get; set; }
    public double MejorNota { get; set; }
    public double UltimaNota { get; set; }
    public DateTime? UltimaActividad { get; set; }

    /// <summary>Precision (0-100) por bloque. Contrato historico consumido por el cliente.</summary>
    public Dictionary<string, double> ProgresoPorBloque { get; set; } = new();

    /// <summary>Desglose completo por bloque, ordenado de peor a mejor precision.</summary>
    public List<BloqueRendimiento> RendimientoPorBloque { get; set; } = new();

    /// <summary>Porcentaje global de acierto sobre preguntas presentadas.</summary>
    public double TasaAcierto => TotalPreguntas > 0 ? Aciertos * 100d / TotalPreguntas : 0d;
}
