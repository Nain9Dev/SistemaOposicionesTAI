namespace Oposiciones.Api.DTOs;

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Respuesta de autenticacion. El JWT viaja exclusivamente en cookie HttpOnly: aqui solo se
/// devuelve el token CSRF, que el cliente debe reenviar en la cabecera X-CSRF-Token.
/// </summary>
public class AuthResponseDto
{
    public string CsrfToken { get; set; } = string.Empty;
    public UserProfileDto User { get; set; } = new();

    /// <summary>Caducidad del token de acceso, para que el cliente programe la renovacion.</summary>
    public DateTime ExpiresAt { get; set; }
}

public class UserProfileDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
