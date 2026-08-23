using Oposiciones.Domain.Entities;

namespace Oposiciones.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario?> GetByIdAsync(int id);
    Task<int> CreateAsync(Usuario usuario);
}
