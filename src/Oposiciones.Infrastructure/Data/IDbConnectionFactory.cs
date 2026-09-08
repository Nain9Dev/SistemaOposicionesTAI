using Npgsql;

namespace Oposiciones.Infrastructure.Data;

/// <summary>
/// Punto unico de apertura de conexiones. Evita que cada repositorio construya la cadena
/// de conexion por su cuenta y permite compartir el pool de Npgsql en toda la aplicacion.
/// </summary>
public interface IDbConnectionFactory
{
    ValueTask<NpgsqlConnection> OpenAsync(CancellationToken cancellationToken = default);
}
