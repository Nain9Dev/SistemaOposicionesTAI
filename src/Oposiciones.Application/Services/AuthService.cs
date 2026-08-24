using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Oposiciones.Domain.Entities;
using Oposiciones.Domain.Interfaces;
using Oposiciones.Application.Interfaces.Security;
using Oposiciones.Domain.Constants;

namespace Oposiciones.Application.Services;

public class AuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration _config;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUsuarioRepository usuarioRepository, IConfiguration config, IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _config = config;
        _passwordHasher = passwordHasher;
    }

    public async Task<Usuario?> RegisterAsync(string nombre, string email, string password)
    {
        // Verificar si existe
        var existente = await _usuarioRepository.GetByEmailAsync(email);
        if (existente != null) return null;

        var hash = _passwordHasher.HashPassword(password);
        var usuario = new Usuario
        {
            Nombre = nombre,
            Email = email,
            PasswordHash = hash,
            Rol = RoleConstants.User,
            FechaRegistro = DateTime.UtcNow
        };

        var newId = await _usuarioRepository.CreateAsync(usuario);
        usuario.Id = newId;
        return usuario;
    }

    public async Task<(string? token, Usuario? user)> LoginAsync(string email, string password)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(email);
        if (usuario == null) return (null, null);

        if (!_passwordHasher.VerifyPassword(password, usuario.PasswordHash))
        {
            return (null, null);
        }

        return (GenerateJwtToken(usuario), usuario);
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        var jwtKey = _config["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey)) 
            throw new InvalidOperationException("La clave JWT no está configurada.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "OposicionesTAI",
            audience: _config["Jwt:Audience"] ?? "OposicionesTAIUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
