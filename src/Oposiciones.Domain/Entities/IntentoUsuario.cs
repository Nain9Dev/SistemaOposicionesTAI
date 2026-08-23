namespace Oposiciones.Domain.Entities;

public class IntentoUsuario
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int Aciertos { get; set; }
    public int Fallos { get; set; }
    public int Total { get; set; }
    public double Nota { get; set; }
    public string Bloque { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
