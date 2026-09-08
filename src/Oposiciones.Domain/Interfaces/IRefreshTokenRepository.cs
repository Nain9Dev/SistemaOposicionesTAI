using Oposiciones.Domain.Entities;

namespace Oposiciones.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<int> CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>Revoca el token y deja constancia del que lo sustituye, para detectar reutilizaciones.</summary>
    Task RevokeTokenAsync(string token, string? replacedByToken = null, CancellationToken cancellationToken = default);

    Task RevokeAllUserTokensAsync(int usuarioId, CancellationToken cancellationToken = default);

    /// <summary>Elimina tokens caducados o revocados hace tiempo. Devuelve las filas purgadas.</summary>
    Task<int> PurgeExpiredAsync(DateTime olderThanUtc, CancellationToken cancellationToken = default);
}
