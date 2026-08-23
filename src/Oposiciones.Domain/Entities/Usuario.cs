namespace Oposiciones.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = "User"; // "Admin" o "User"
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}
