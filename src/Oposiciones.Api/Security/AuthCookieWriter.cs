using Microsoft.Extensions.Options;

namespace Oposiciones.Api.Security;

/// <summary>Opciones de emision de cookies de sesion.</summary>
public sealed class AuthCookieOptions
{
    public const string SectionName = "AuthCookies";

    public const string AccessTokenCookie = "access_token";
    public const string RefreshTokenCookie = "refresh_token";

    /// <summary>
    /// Cuando el cliente vive en otro dominio hacen falta SameSite=None y Secure. En desarrollo
    /// sobre http://localhost esa combinacion hace que el navegador descarte la cookie, asi que
    /// se conmuta segun el entorno en lugar de fijarla siempre.
    /// </summary>
    public bool CrossSite { get; set; }

    /// <summary>Dominio de la cookie. Vacio deja que el navegador use el host de la respuesta.</summary>
    public string? Domain { get; set; }
}

/// <summary>
/// Punto unico de escritura y borrado de las cookies de sesion: si los atributos de emision y
/// los de borrado no coinciden exactamente, el navegador ignora el Delete y la sesion sobrevive.
/// </summary>
public sealed class AuthCookieWriter
{
    private readonly AuthCookieOptions _options;

    public AuthCookieWriter(IOptions<AuthCookieOptions> options)
    {
        _options = options.Value;
    }

    public void WriteAccessToken(HttpResponse response, string token, DateTimeOffset expiresAt) =>
        response.Cookies.Append(AuthCookieOptions.AccessTokenCookie, token, Build(expiresAt, "/"));

    public void WriteRefreshToken(HttpResponse response, string token, DateTimeOffset expiresAt) =>
        response.Cookies.Append(AuthCookieOptions.RefreshTokenCookie, token, Build(expiresAt, "/"));

    public void Clear(HttpResponse response)
    {
        response.Cookies.Delete(AuthCookieOptions.AccessTokenCookie, Build(null, "/"));
        response.Cookies.Delete(AuthCookieOptions.RefreshTokenCookie, Build(null, "/"));
    }

    private CookieOptions Build(DateTimeOffset? expiresAt, string path) => new()
    {
        HttpOnly = true,
        Secure = _options.CrossSite,
        SameSite = _options.CrossSite ? SameSiteMode.None : SameSiteMode.Lax,
        Path = path,
        Domain = string.IsNullOrWhiteSpace(_options.Domain) ? null : _options.Domain,
        Expires = expiresAt,
        IsEssential = true
    };
}
