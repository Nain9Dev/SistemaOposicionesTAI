using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.RateLimiting;
using Oposiciones.Api.DTOs;
using Oposiciones.Application.Services;

namespace Oposiciones.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("AuthLimiter")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IDistributedCache _cache;

    public AuthController(AuthService authService, IDistributedCache cache)
    {
        _authService = authService;
        _cache = cache;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var (token, user) = await _authService.LoginAsync(dto.Email, dto.Password);
        if (token == null || user == null) return Unauthorized(new { message = "Email o contraseña incorrectos" });
        
        SetTokenCookie(token);

        var csrfToken = Guid.NewGuid().ToString();
        await _cache.SetStringAsync($"csrf_{user.Id}", csrfToken, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) });

        return Ok(new AuthResponseDto { 
            Token = "", // Ya no se expone al frontend
            CsrfToken = csrfToken,
            User = new UserProfileDto { Id = user.Id, Nombre = user.Nombre, Email = user.Email, Rol = user.Rol }
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { message = "Datos inválidos" });

        var createdUser = await _authService.RegisterAsync(dto.Nombre, dto.Email, dto.Password);
        if (createdUser == null) return Conflict(new { message = "El usuario ya existe" });

        var (token, user) = await _authService.LoginAsync(dto.Email, dto.Password);
        
        var csrfToken = "";
        if (token != null) 
        {
            SetTokenCookie(token);
            csrfToken = Guid.NewGuid().ToString();
            await _cache.SetStringAsync($"csrf_{user!.Id}", csrfToken, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) });
        }

        return Ok(new AuthResponseDto { 
            Token = "", 
            CsrfToken = csrfToken,
            User = new UserProfileDto { Id = user!.Id, Nombre = user.Nombre, Email = user.Email, Rol = user.Rol }
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });
        return Ok(new { message = "Sesión cerrada correctamente" });
    }

    private void SetTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Requerido para SameSite=None
            SameSite = SameSiteMode.None, // Permite cross-site en producción si el frontend y backend están en dominios distintos
            Expires = DateTime.UtcNow.AddHours(24)
        };
        Response.Cookies.Append("access_token", token, cookieOptions);
    }
}
