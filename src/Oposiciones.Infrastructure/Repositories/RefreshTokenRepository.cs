using Dapper;
using Npgsql;
using Oposiciones.Domain.Entities;
using Oposiciones.Domain.Interfaces;

namespace Oposiciones.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly string _connectionString;

    public RefreshTokenRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<int> CreateAsync(RefreshToken refreshToken)
    {
        var sql = @"
            INSERT INTO RefreshTokens (Token, UsuarioId, ExpiresAt, CreatedAt, RevokedAt)
            VALUES (@Token, @UsuarioId, @ExpiresAt, @CreatedAt, @RevokedAt)
            RETURNING Id;
        ";
        
        using var conn = new NpgsqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(sql, refreshToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        var sql = "SELECT * FROM RefreshTokens WHERE Token = @Token";
        using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QuerySingleOrDefaultAsync<RefreshToken>(sql, new { Token = token });
    }

    public async Task RevokeTokenAsync(string token)
    {
        var sql = "UPDATE RefreshTokens SET RevokedAt = @RevokedAt WHERE Token = @Token";
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { Token = token, RevokedAt = DateTime.UtcNow });
    }

    public async Task RevokeAllUserTokensAsync(int usuarioId)
    {
        var sql = "UPDATE RefreshTokens SET RevokedAt = @RevokedAt WHERE UsuarioId = @UsuarioId AND RevokedAt IS NULL";
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { UsuarioId = usuarioId, RevokedAt = DateTime.UtcNow });
    }
}
