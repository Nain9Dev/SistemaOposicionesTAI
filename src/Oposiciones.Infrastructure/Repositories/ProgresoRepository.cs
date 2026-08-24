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

    public async Task<(IEnumerable<IntentoUsuario> Items, int TotalCount)> GetHistorialAsync(int usuarioId, int page, int pageSize)
    {
        var countSql = "SELECT COUNT(*) FROM IntentosUsuario WHERE UsuarioId = @UsuarioId";
        var sql = @"
            SELECT * FROM IntentosUsuario 
            WHERE UsuarioId = @UsuarioId 
            ORDER BY Fecha DESC 
            OFFSET @Offset LIMIT @Limit";
            
        using var conn = new NpgsqlConnection(_connectionString);
        var totalCount = await conn.ExecuteScalarAsync<int>(countSql, new { UsuarioId = usuarioId });
        var items = await conn.QueryAsync<IntentoUsuario>(sql, new { 
            UsuarioId = usuarioId, 
            Offset = (page - 1) * pageSize, 
            Limit = pageSize 
        });
        
        return (items, totalCount);
    }

    public async Task<EstadisticasResumen> GetEstadisticasResumidasAsync(int usuarioId)
    {
        var sql = @"
            -- 1. Estadísticas globales
            SELECT 
                COALESCE(SUM(Total), 0) AS TotalPreguntas, 
                COALESCE(SUM(Aciertos), 0) AS Aciertos, 
                COALESCE(SUM(Fallos), 0) AS Fallos, 
                COALESCE(AVG(Nota), 0) AS NotaMedia
            FROM IntentosUsuario 
            WHERE UsuarioId = @UsuarioId;

            -- 2. Estadísticas por bloque
            SELECT 
                Bloque, 
                AVG(CAST(Aciertos AS FLOAT) / NULLIF(Total, 0) * 100) AS PromedioAciertos
            FROM IntentosUsuario
            WHERE UsuarioId = @UsuarioId
            GROUP BY Bloque;
        ";

        using var conn = new NpgsqlConnection(_connectionString);
        using var multi = await conn.QueryMultipleAsync(sql, new { UsuarioId = usuarioId });

        var resumen = await multi.ReadSingleOrDefaultAsync<EstadisticasResumen>() ?? new EstadisticasResumen();

        var bloques = await multi.ReadAsync();
        foreach (var row in bloques)
        {
            if (row.Bloque != null && row.PromedioAciertos != null)
            {
                resumen.ProgresoPorBloque[row.Bloque] = (double)row.PromedioAciertos;
            }
        }

        return resumen;
    }
}
