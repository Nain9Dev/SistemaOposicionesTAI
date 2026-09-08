namespace Oposiciones.Application.DTOs;

public sealed class BloqueRendimientoDto
{
    public string Bloque { get; init; } = string.Empty;
    public int Intentos { get; init; }
    public int Aciertos { get; init; }
    public int Fallos { get; init; }
    public int Total { get; init; }
    public double Precision { get; init; }
    public double NotaMedia { get; init; }
}

public sealed class TendenciaPuntoDto
{
    public DateTime Fecha { get; init; }
    public double Nota { get; init; }
}

/// <summary>
/// Panel de rendimiento. Los cinco primeros campos son el contrato historico consumido
/// por el cliente; el resto se anade de forma aditiva.
/// </summary>
public sealed class EstadisticasDto
{
    public int TotalPreguntas { get; init; }
    public int Aciertos { get; init; }
    public int Fallos { get; init; }
    public double NotaMedia { get; init; }
    public Dictionary<string, double> ProgresoPorBloque { get; init; } = new();

    public int Blancos { get; init; }
    public int TotalIntentos { get; init; }
    public double TasaAcierto { get; init; }
    public double MejorNota { get; init; }
    public double UltimaNota { get; init; }
    public DateTime? UltimaActividad { get; init; }

    /// <summary>Bloques ordenados de peor a mejor precision: los primeros son los que hay que repasar.</summary>
    public IReadOnlyList<BloqueRendimientoDto> RendimientoPorBloque { get; init; } = [];

    /// <summary>Bloque con menor precision entre los que tienen datos suficientes.</summary>
    public string? BloqueMasDebil { get; init; }

    /// <summary>Evolucion de la nota en los ultimos simulacros, de mas antiguo a mas reciente.</summary>
    public IReadOnlyList<TendenciaPuntoDto> Tendencia { get; init; } = [];
}
