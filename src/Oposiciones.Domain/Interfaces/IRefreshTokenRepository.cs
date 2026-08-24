using Oposiciones.Domain.Entities;

namespace Oposiciones.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<int> CreateAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task RevokeTokenAsync(string token);
    Task RevokeAllUserTokensAsync(int usuarioId);
}
