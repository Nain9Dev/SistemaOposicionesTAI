using Microsoft.AspNetCore.Mvc;
using Oposiciones.Api.DTOs;
using Oposiciones.Api.Services;

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
        
        return Ok(new AuthResponseDto { 
            Token = token,
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

        return Ok(new AuthResponseDto { 
            Token = token!,
            User = new UserProfileDto { Id = user!.Id, Nombre = user.Nombre, Email = user.Email, Rol = user.Rol }
        });
    }
}
