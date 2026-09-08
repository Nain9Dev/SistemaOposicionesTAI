using Oposiciones.Domain.Entities;

namespace Oposiciones.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Usuario?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crea el usuario. Devuelve null si el email ya estaba dado de alta: la unicidad se resuelve
    /// en base de datos para evitar la condicion de carrera del "comprobar y despues insertar".
    /// </summary>
    Task<int?> CreateAsync(Usuario usuario, CancellationToken cancellationToken = default);
}
