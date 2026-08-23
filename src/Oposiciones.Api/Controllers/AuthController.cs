using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oposiciones.Api.DTOs;
using Oposiciones.Application.Services;

namespace Oposiciones.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var (token, user) = await _authService.LoginAsync(dto.Email, dto.Password);
        if (token == null || user == null) return Unauthorized(new { message = "Email o contraseña incorrectos" });
        
        SetTokenCookie(token);

        return Ok(new AuthResponseDto { 
            Token = "", // Ya no se expone al frontend
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
        
        if (token != null) SetTokenCookie(token);

        return Ok(new AuthResponseDto { 
            Token = "", 
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
