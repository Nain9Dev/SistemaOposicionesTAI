using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Oposiciones.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Identificador numerico del usuario. El JWT lo publica en "sub"; ASP.NET Core lo mapea a
    /// NameIdentifier, pero se comprueban ambos por si se desactiva el mapeo de claims entrantes.
    /// </summary>
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var raw = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return int.TryParse(raw, out var id) && id > 0 ? id : null;
    }

    /// <summary>Identificador de sesion (jti). Ancla el token CSRF al dispositivo concreto.</summary>
    public static string? GetSessionId(this ClaimsPrincipal user) =>
        user.FindFirstValue(JwtRegisteredClaimNames.Jti)
        ?? user.FindFirstValue("jti");

    /// <summary>
    /// Clave de propiedad usada en Attempts.UserName. Se deriva del identificador para que no
    /// dependa del nombre visible, que el usuario puede cambiar.
    /// </summary>
    public static string? GetAttemptOwner(this ClaimsPrincipal user) =>
        user.GetUserId() is { } id ? $"u{id}" : null;

    /// <summary>
    /// Caducidad del token de acceso ("exp", segundos desde epoch). Determina cuanto tiempo
    /// hay que mantener la sesion en la lista de revocacion tras cerrar sesion.
    /// </summary>
    public static DateTimeOffset? GetAccessTokenExpiry(this ClaimsPrincipal user)
    {
        var raw = user.FindFirstValue(JwtRegisteredClaimNames.Exp)
                  ?? user.FindFirstValue("exp");

        return long.TryParse(raw, out var seconds)
            ? DateTimeOffset.FromUnixTimeSeconds(seconds)
            : null;
    }
}
