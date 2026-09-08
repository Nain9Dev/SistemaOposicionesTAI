using Dapper;
using Oposiciones.Domain.Entities;
using Oposiciones.Domain.Interfaces;
using Oposiciones.Infrastructure.Data;

namespace Oposiciones.Infrastructure.Repositories;

public class ProgresoRepository : IProgresoRepository
{
    private const string ColumnList =
        "Id, UsuarioId, Aciertos, Fallos, Blancos, Total, Nota::float8 AS Nota, Bloque, Fecha";

    private readonly IDbConnectionFactory _connectionFactory;

    public ProgresoRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> AddIntentoAsync(IntentoUsuario intento, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO IntentosUsuario (UsuarioId, Aciertos, Fallos, Blancos, Total, Nota, Bloque, Fecha)
            VALUES (@UsuarioId, @Aciertos, @Fallos, @Blancos, @Total, @Nota, @Bloque, @Fecha)
            RETURNING Id;
            """;

        await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
        return await conn.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, intento, cancellationToken: cancellationToken));
    }

    public async Task<(IReadOnlyList<IntentoUsuario> Items, int TotalCount)> GetHistorialAsync(
        int usuarioId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        // Recuento y pagina en una sola ida y vuelta: antes eran dos comandos independientes
        // sobre conexiones distintas, con riesgo de devolver un total incoherente con la pagina.
        const string sql = $"""
            SELECT COUNT(*)::INT FROM IntentosUsuario WHERE UsuarioId = @UsuarioId;

            SELECT {ColumnList}
              FROM IntentosUsuario
             WHERE UsuarioId = @UsuarioId
             ORDER BY Fecha DESC, Id DESC
             OFFSET @Offset LIMIT @Limit;
            """;

        var parameters = new
        {
            UsuarioId = usuarioId,
            Offset = (long)(page - 1) * pageSize,
            Limit = pageSize
        };

        await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
        await using var multi = await conn.QueryMultipleAsync(
            new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

        var totalCount = await multi.ReadSingleAsync<int>();
        var items = (await multi.ReadAsync<IntentoUsuario>()).AsList();

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<IntentoUsuario>> GetUltimosIntentosAsync(
        int usuarioId, int limit, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT * FROM (
                SELECT {ColumnList}
                  FROM IntentosUsuario
                 WHERE UsuarioId = @UsuarioId
                 ORDER BY Fecha DESC, Id DESC
                 LIMIT @Limit
            ) AS ultimos
            ORDER BY Fecha ASC, Id ASC;
            """;

        await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
        var items = await conn.QueryAsync<IntentoUsuario>(
            new CommandDefinition(sql, new { UsuarioId = usuarioId, Limit = limit }, cancellationToken: cancellationToken));

        return items.AsList();
    }

    public async Task<int> DeleteHistorialAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM IntentosUsuario WHERE UsuarioId = @UsuarioId;";

        await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
        return await conn.ExecuteAsync(
            new CommandDefinition(sql, new { UsuarioId = usuarioId }, cancellationToken: cancellationToken));
    }

    public async Task<EstadisticasResumen> GetEstadisticasResumidasAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        // La agregacion por bloque se hace sobre las sumas del bloque, no como media de medias:
        // un simulacro de 40 preguntas no puede pesar lo mismo que uno de 5.
        const string sql = """
            SELECT
                COALESCE(SUM(Total), 0)::INT      AS TotalPreguntas,
                COALESCE(SUM(Aciertos), 0)::INT   AS Aciertos,
                COALESCE(SUM(Fallos), 0)::INT     AS Fallos,
                COALESCE(SUM(Blancos), 0)::INT    AS Blancos,
                COALESCE(AVG(Nota), 0)::float8    AS NotaMedia,
                COUNT(*)::INT                     AS TotalIntentos,
                COALESCE(MAX(Nota), 0)::float8    AS MejorNota,
                MAX(Fecha)                        AS UltimaActividad
              FROM IntentosUsuario
             WHERE UsuarioId = @UsuarioId;

            SELECT
                Bloque,
                COUNT(*)::INT                   AS Intentos,
                COALESCE(SUM(Aciertos), 0)::INT AS Aciertos,
                COALESCE(SUM(Fallos), 0)::INT   AS Fallos,
                COALESCE(SUM(Total), 0)::INT    AS Total,
                CASE WHEN COALESCE(SUM(Total), 0) > 0
                     THEN SUM(Aciertos) * 100.0 / SUM(Total)
                     ELSE 0 END::float8         AS "Precision",
                COALESCE(AVG(Nota), 0)::float8  AS NotaMedia
              FROM IntentosUsuario
             WHERE UsuarioId = @UsuarioId
             GROUP BY Bloque
             ORDER BY "Precision" ASC;

            SELECT COALESCE(Nota, 0)::float8
              FROM IntentosUsuario
             WHERE UsuarioId = @UsuarioId
             ORDER BY Fecha DESC, Id DESC
             LIMIT 1;
            """;

        await using var conn = await _connectionFactory.OpenAsync(cancellationToken);
        await using var multi = await conn.QueryMultipleAsync(
            new CommandDefinition(sql, new { UsuarioId = usuarioId }, cancellationToken: cancellationToken));

        var resumen = await multi.ReadSingleOrDefaultAsync<EstadisticasResumen>() ?? new EstadisticasResumen();
        var bloques = (await multi.ReadAsync<BloqueRendimiento>()).AsList();
        resumen.UltimaNota = await multi.ReadSingleOrDefaultAsync<double?>() ?? 0d;

        resumen.RendimientoPorBloque = bloques;
        foreach (var bloque in bloques)
        {
            resumen.ProgresoPorBloque[bloque.Bloque] = Math.Round(bloque.Precision, 2);
        }

        return resumen;
    }
}
