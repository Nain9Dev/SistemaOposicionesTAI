using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;

namespace Oposiciones.Api.Security;

public interface ICsrfTokenService
{
    /// <summary>Emite y almacena el token CSRF asociado a una sesion concreta (jti del JWT).</summary>
    Task<string> IssueAsync(string sessionId, CancellationToken cancellationToken = default);

    Task<bool> ValidateAsync(string sessionId, string? candidateToken, CancellationToken cancellationToken = default);

    Task RevokeAsync(string sessionId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Tokens CSRF con alcance de sesion. La version anterior los indexaba por usuario, de modo
/// que iniciar sesion en un segundo dispositivo invalidaba el token del primero y este
/// empezaba a recibir 403 en cualquier escritura.
/// </summary>
public sealed class CsrfTokenService : ICsrfTokenService
{
    private const string KeyPrefix = "csrf:";
    private const int TokenBytes = 32;
    private static readonly TimeSpan Lifetime = TimeSpan.FromHours(24);

    private readonly IDistributedCache _cache;

    public CsrfTokenService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<string> IssueAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(TokenBytes));

        await _cache.SetStringAsync(
            KeyPrefix + sessionId,
            token,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = Lifetime },
            cancellationToken);

        return token;
    }

    public async Task<bool> ValidateAsync(string sessionId, string? candidateToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(candidateToken))
        {
            return false;
        }

        var stored = await _cache.GetStringAsync(KeyPrefix + sessionId, cancellationToken);
        if (string.IsNullOrEmpty(stored))
        {
            return false;
        }

        // Comparacion en tiempo fijo: una comparacion normal filtra el prefijo correcto por tiempo.
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(stored),
            Encoding.UTF8.GetBytes(candidateToken));
    }

    public Task RevokeAsync(string sessionId, CancellationToken cancellationToken = default) =>
        _cache.RemoveAsync(KeyPrefix + sessionId, cancellationToken);
}
