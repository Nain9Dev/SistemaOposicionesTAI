using System.Data;
using Dapper;
using Npgsql;
using Oposiciones.Domain.Entities;
using Oposiciones.Domain.Interfaces;

namespace Oposiciones.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly string _connectionString;

    public UsuarioRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<int> CreateAsync(Usuario usuario)
    {
        var sql = @"
            INSERT INTO Usuarios (Nombre, Email, PasswordHash, Rol, FechaRegistro)
            VALUES (@Nombre, @Email, @PasswordHash, @Rol, @FechaRegistro)
            RETURNING Id;
        ";
        
        using var conn = new NpgsqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(sql, usuario);
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        var sql = "SELECT * FROM Usuarios WHERE Email = @Email";
        using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QuerySingleOrDefaultAsync<Usuario>(sql, new { Email = email });
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM Usuarios WHERE Id = @Id";
        using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QuerySingleOrDefaultAsync<Usuario>(sql, new { Id = id });
    }
}
