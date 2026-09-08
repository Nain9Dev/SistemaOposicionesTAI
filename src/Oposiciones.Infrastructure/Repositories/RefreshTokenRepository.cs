using Dapper;
using Oposiciones.Domain.Entities;
using Oposiciones.Domain.Interfaces;
using Oposiciones.Infrastructure.Data;

namespace Oposiciones.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private const string ColumnList = "Id, Token, UsuarioId, ExpiresAt, CreatedAt, RevokedAt, ReplacedByToken";

    private readonly IDbConnectionFactory _connectionFactory;

    public RefreshTokenRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO RefreshTokens (Token, UsuarioId, ExpiresAt, CreatedAt, RevokedAt, ReplacedByToken)
            VALUES (@Token, @UsuarioId, @ExpiresAt, @CreatedAt, @RevokedAt, @ReplacedByToken)
            RETURNING Id;
            """;

        await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
        return await conn.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, refreshToken, cancellationToken: cancellationToken));
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        const string sql = $"SELECT {ColumnList} FROM RefreshTokens WHERE Token = @Token;";

        await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
        return await conn.QuerySingleOrDefaultAsync<RefreshToken>(
            new CommandDefinition(sql, new { Token = token }, cancellationToken: cancellationToken));
    }

    public async Task RevokeTokenAsync(string token, string? replacedByToken = null, CancellationToken cancellationToken = default)
    {
        // Solo se revoca una vez: si ya estaba revocado se conserva la marca original.
        const string sql = """
            UPDATE RefreshTokens
               SET RevokedAt = @RevokedAt,
                   ReplacedByToken = COALESCE(@ReplacedByToken, ReplacedByToken)
             WHERE Token = @Token
               AND RevokedAt IS NULL;
            """;

        await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
        await conn.ExecuteAsync(new CommandDefinition(
            sql,
            new { Token = token, RevokedAt = DateTime.UtcNow, ReplacedByToken = replacedByToken },
            cancellationToken: cancellationToken));
    }

    public async Task RevokeAllUserTokensAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE RefreshTokens
               SET RevokedAt = @RevokedAt
             WHERE UsuarioId = @UsuarioId
               AND RevokedAt IS NULL;
            """;

        await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
        await conn.ExecuteAsync(new CommandDefinition(
            sql,
            new { UsuarioId = usuarioId, RevokedAt = DateTime.UtcNow },
            cancellationToken: cancellationToken));
    }

    public async Task<int> PurgeExpiredAsync(DateTime olderThanUtc, CancellationToken cancellationToken = default)
    {
        const string sql = """
            DELETE FROM RefreshTokens
             WHERE ExpiresAt < @Threshold
                OR (RevokedAt IS NOT NULL AND RevokedAt < @Threshold);
            """;

        await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
        return await conn.ExecuteAsync(new CommandDefinition(
            sql,
            new { Threshold = DateTime.SpecifyKind(olderThanUtc, DateTimeKind.Utc) },
            cancellationToken: cancellationToken));
    }
}
