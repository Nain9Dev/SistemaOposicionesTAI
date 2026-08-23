using System.Collections.Generic;

namespace Oposiciones.Application.DTOs;

public class EstadisticasDto
{
    public int TotalPreguntas { get; set; }
    public int Aciertos { get; set; }
    public int Fallos { get; set; }
    public double NotaMedia { get; set; }
    public Dictionary<string, double> ProgresoPorBloque { get; set; } = new();
}
