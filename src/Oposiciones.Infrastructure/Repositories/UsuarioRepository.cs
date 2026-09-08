using Dapper;
using Oposiciones.Domain.Entities;
using Oposiciones.Domain.Interfaces;
using Oposiciones.Infrastructure.Data;

namespace Oposiciones.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private const string ColumnList = "Id, Nombre, Email, PasswordHash, Rol, FechaRegistro";

    private readonly IDbConnectionFactory _connectionFactory;

    public UsuarioRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int?> CreateAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        // ON CONFLICT delega la unicidad del email en el indice: dos altas simultaneas con el
        // mismo correo no pueden colarse entre el "comprobar" y el "insertar".
        const string sql = """
            INSERT INTO Usuarios (Nombre, Email, PasswordHash, Rol, FechaRegistro)
            VALUES (@Nombre, @Email, @PasswordHash, @Rol, @FechaRegistro)
            ON CONFLICT DO NOTHING
            RETURNING Id;
            """;

        await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
        var id = await conn.ExecuteScalarAsync<int?>(
            new CommandDefinition(sql, usuario, cancellationToken: cancellationToken));

        return id;
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        const string sql = $"SELECT {ColumnList} FROM Usuarios WHERE LOWER(Email) = LOWER(@Email);";

        await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
        return await conn.QuerySingleOrDefaultAsync<Usuario>(
            new CommandDefinition(sql, new { Email = email }, cancellationToken: cancellationToken));
    }

    public async Task<Usuario?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = $"SELECT {ColumnList} FROM Usuarios WHERE Id = @Id;";

        await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
        return await conn.QuerySingleOrDefaultAsync<Usuario>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}
