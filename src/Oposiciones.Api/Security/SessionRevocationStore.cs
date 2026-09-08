using Microsoft.Extensions.Caching.Distributed;

namespace Oposiciones.Api.Security;

/// <summary>
/// Lista de revocacion de sesiones.
///
/// Un JWT es autocontenido: borrar la cookie en el cliente no invalida el token. Sin esta
/// lista, quien hubiera capturado el token seguia teniendo acceso durante toda su vigencia
/// aunque el usuario hubiese cerrado sesion. Se registra el identificador de sesion (jti)
/// solo hasta que el token caduca, asi que la lista se purga sola.
/// </summary>
public interface ISessionRevocationStore
{
    Task RevokeAsync(string sessionId, DateTimeOffset accessTokenExpiresAt, CancellationToken cancellationToken = default);

    Task<bool> IsRevokedAsync(string sessionId, CancellationToken cancellationToken = default);
}

public sealed class SessionRevocationStore : ISessionRevocationStore
{
    private const string KeyPrefix = "session-revoked:";

    /// <summary>Margen sobre la caducidad para absorber desfases de reloj entre instancias.</summary>
    private static readonly TimeSpan Margin = TimeSpan.FromMinutes(5);

    /// <summary>Tope de seguridad por si llega una caducidad ausente o absurdamente lejana.</summary>
    private static readonly TimeSpan MaxRetention = TimeSpan.FromDays(1);

    private readonly IDistributedCache _cache;

    public SessionRevocationStore(IDistributedCache cache)
    {
        _cache = cache;
    }

    public Task RevokeAsync(string sessionId, DateTimeOffset accessTokenExpiresAt, CancellationToken cancellationToken = default)
    {
        var retention = accessTokenExpiresAt - DateTimeOffset.UtcNow + Margin;

        if (retention <= TimeSpan.Zero)
        {
            // El token ya habia caducado: no hay nada que revocar.
            return Task.CompletedTask;
        }

        return _cache.SetStringAsync(
            KeyPrefix + sessionId,
            "1",
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = retention > MaxRetention ? MaxRetention : retention
            },
            cancellationToken);
    }

    public async Task<bool> IsRevokedAsync(string sessionId, CancellationToken cancellationToken = default) =>
        await _cache.GetStringAsync(KeyPrefix + sessionId, cancellationToken) is not null;
}
