using Oposiciones.Application.DTOs;

namespace Oposiciones.Application.Interfaces;

public interface IAuthService
{
    /// <summary>Devuelve null si el email ya esta registrado.</summary>
    Task<AuthResult?> RegisterAsync(string nombre, string email, string password, CancellationToken cancellationToken = default);

    /// <summary>Devuelve null si las credenciales no son validas.</summary>
    Task<AuthResult?> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rota el refresh token. Devuelve null si el token no existe, ha caducado o ya fue usado;
    /// en ese ultimo caso se revocan todas las sesiones del usuario por sospecha de robo.
    /// </summary>
    Task<AuthResult?> RefreshTokenAsync(string oldRefreshToken, CancellationToken cancellationToken = default);

    Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
}
