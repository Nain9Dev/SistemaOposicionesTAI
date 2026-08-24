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
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IConfiguration _config;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUsuarioRepository usuarioRepository, IRefreshTokenRepository refreshTokenRepository, IConfiguration config, IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _refreshTokenRepository = refreshTokenRepository;
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

    public async Task<(string? token, string? refreshToken, Usuario? user)> LoginAsync(string email, string password)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(email);
        if (usuario == null) return (null, null, null);

        if (!_passwordHasher.VerifyPassword(password, usuario.PasswordHash))
        {
            return (null, null, null);
        }

        var jwt = GenerateJwtToken(usuario);
        var refreshToken = await GenerateRefreshTokenAsync(usuario.Id);

        return (jwt, refreshToken, usuario);
    }

    public async Task<(string? token, string? refreshToken, Usuario? user)> RefreshTokenAsync(string oldRefreshToken)
    {
        var rt = await _refreshTokenRepository.GetByTokenAsync(oldRefreshToken);
        if (rt == null || !rt.IsActive) return (null, null, null);

        var usuario = await _usuarioRepository.GetByIdAsync(rt.UsuarioId);
        if (usuario == null) return (null, null, null);

        // Revocar el token usado y generar uno nuevo
        await _refreshTokenRepository.RevokeTokenAsync(oldRefreshToken);

        var newJwt = GenerateJwtToken(usuario);
        var newRefreshToken = await GenerateRefreshTokenAsync(usuario.Id);

        return (newJwt, newRefreshToken, usuario);
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        await _refreshTokenRepository.RevokeTokenAsync(token);
    }

    private async Task<string> GenerateRefreshTokenAsync(int usuarioId)
    {
        var token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
        var rt = new RefreshToken
        {
            Token = token,
            UsuarioId = usuarioId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.CreateAsync(rt);
        return token;
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
