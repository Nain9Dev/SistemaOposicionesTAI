using Dapper;
using Npgsql;
using Oposiciones.Domain.Entities;
using Oposiciones.Domain.Interfaces;

namespace Oposiciones.Infrastructure.Repositories;

public class ProgresoRepository : IProgresoRepository
{
    private readonly string _connectionString;

    public ProgresoRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<int> AddIntentoAsync(IntentoUsuario intento)
    {
        var sql = @"
            INSERT INTO IntentosUsuario (UsuarioId, Aciertos, Fallos, Total, Nota, Bloque, Fecha)
            VALUES (@UsuarioId, @Aciertos, @Fallos, @Total, @Nota, @Bloque, @Fecha)
            RETURNING Id;
        ";
        
        using var conn = new NpgsqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(sql, intento);
    }

    public async Task<IEnumerable<IntentoUsuario>> GetHistorialAsync(int usuarioId)
    {
        var sql = "SELECT * FROM IntentosUsuario WHERE UsuarioId = @UsuarioId ORDER BY Fecha DESC";
        using var conn = new NpgsqlConnection(_connectionString);
        return await conn.QueryAsync<IntentoUsuario>(sql, new { UsuarioId = usuarioId });
    }
}
