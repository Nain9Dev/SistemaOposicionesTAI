using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Oposiciones.Api.DTOs;
using Oposiciones.Api.Extensions;
using Oposiciones.Api.Security;
using Oposiciones.Application.DTOs;
using Oposiciones.Application.Interfaces;

namespace Oposiciones.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting(RateLimitPolicies.Auth)]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICsrfTokenService _csrfTokenService;
    private readonly ISessionRevocationStore _sessionRevocationStore;
    private readonly AuthCookieWriter _cookieWriter;

    public AuthController(
        IAuthService authService,
        ICsrfTokenService csrfTokenService,
        ISessionRevocationStore sessionRevocationStore,
        AuthCookieWriter cookieWriter)
    {
        _authService = authService;
        _csrfTokenService = csrfTokenService;
        _sessionRevocationStore = sessionRevocationStore;
        _cookieWriter = cookieWriter;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(dto.Email, dto.Password, cancellationToken);
        if (result is null)
        {
            // Mismo mensaje para email inexistente y contrasena incorrecta: no se filtra
            // que direcciones estan dadas de alta.
            return Unauthorized(new { message = "Email o contrasena incorrectos." });
        }

        return Ok(await EstablishSessionAsync(result, cancellationToken));
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(dto.Nombre, dto.Email, dto.Password, cancellationToken);
        if (result is null)
        {
            return Conflict(new { message = "Ya existe una cuenta con ese email." });
        }

        return Ok(await EstablishSessionAsync(result, cancellationToken));
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(AuthCookieOptions.RefreshTokenCookie, out var oldRefreshToken)
            || string.IsNullOrWhiteSpace(oldRefreshToken))
        {
            return Unauthorized(new { message = "No hay sesion que renovar." });
        }

        var result = await _authService.RefreshTokenAsync(oldRefreshToken, cancellationToken);
        if (result is null)
        {
            // Token invalido, caducado o reutilizado: se limpian las cookies para que el
            // cliente no reintente en bucle con una credencial muerta.
            _cookieWriter.Clear(Response);
            return Unauthorized(new { message = "La sesion ha expirado. Inicie sesion de nuevo." });
        }

        return Ok(await EstablishSessionAsync(result, cancellationToken));
    }

    /// <summary>Perfil de la sesion activa. Permite al cliente rehidratar sin exponer el JWT.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public IActionResult Me()
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        return Ok(new UserProfileDto
        {
            Id = userId.Value,
            Nombre = User.Identity?.Name ?? string.Empty,
            Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty,
            Rol = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        if (Request.Cookies.TryGetValue(AuthCookieOptions.RefreshTokenCookie, out var refreshToken)
            && !string.IsNullOrWhiteSpace(refreshToken))
        {
            await _authService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);
        }

        if (User.GetSessionId() is { } sessionId)
        {
            await _csrfTokenService.RevokeAsync(sessionId, cancellationToken);

            // Borrar la cookie no invalida el JWT, que es autocontenido y sigue vigente hasta
            // su caducidad. La sesion se anota en la lista de revocacion para que un token
            // copiado antes del cierre deje de servir de inmediato.
            await _sessionRevocationStore.RevokeAsync(
                sessionId,
                User.GetAccessTokenExpiry() ?? DateTimeOffset.UtcNow.AddHours(1),
                cancellationToken);
        }

        _cookieWriter.Clear(Response);

        return Ok(new { message = "Sesion cerrada correctamente." });
    }

    private async Task<AuthResponseDto> EstablishSessionAsync(AuthResult result, CancellationToken cancellationToken)
    {
        _cookieWriter.WriteAccessToken(Response, result.AccessToken, result.AccessTokenExpiresAt);
        _cookieWriter.WriteRefreshToken(Response, result.RefreshToken, result.RefreshTokenExpiresAt);

        var csrfToken = await _csrfTokenService.IssueAsync(result.SessionId, cancellationToken);

        return new AuthResponseDto
        {
            CsrfToken = csrfToken,
            ExpiresAt = result.AccessTokenExpiresAt,
            User = new UserProfileDto
            {
                Id = result.User.Id,
                Nombre = result.User.Nombre,
                Email = result.User.Email,
                Rol = result.User.Rol
            }
        };
    }
}
