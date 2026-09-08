namespace Oposiciones.Application.DTOs;

/// <summary>Perfil publico del usuario. Nunca transporta el hash de la contrasena.</summary>
public sealed record AuthenticatedUser(int Id, string Nombre, string Email, string Rol);

/// <summary>
/// Credenciales emitidas tras un login, un registro o una rotacion de refresh token.
/// </summary>
/// <param name="AccessToken">JWT de acceso, entregado siempre en cookie HttpOnly.</param>
/// <param name="RefreshToken">Token de rotacion, tambien en cookie HttpOnly.</param>
/// <param name="SessionId">Identificador (jti) del JWT: ancla el token CSRF a esta sesion.</param>
/// <param name="AccessTokenExpiresAt">Caducidad del JWT, para alinear la cookie.</param>
/// <param name="RefreshTokenExpiresAt">Caducidad del refresh token.</param>
/// <param name="User">Perfil del usuario autenticado.</param>
public sealed record AuthResult(
    string AccessToken,
    string RefreshToken,
    string SessionId,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt,
    AuthenticatedUser User);
