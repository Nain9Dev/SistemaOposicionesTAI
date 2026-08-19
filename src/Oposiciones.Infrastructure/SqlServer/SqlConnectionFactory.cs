using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Oposiciones.Infrastructure.Configuration;

namespace Oposiciones.Infrastructure.SqlServer;

/// <summary>
/// Crea conexiones a SQL Server. Existe como abstraccion para que los repositorios no construyan
/// conexiones a mano ni reciban una cadena de conexion suelta: la configuracion queda en un unico
/// sitio y las pruebas pueden sustituir la fabrica.
/// </summary>
public interface ISqlConnectionFactory
{
    SqlConnection Create();

    int CommandTimeoutSeconds { get; }
}

/// <inheritdoc />
public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IOptions<PersistenceOptions> options, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "El proveedor SqlServer requiere una cadena de conexion. Configure "
                + "'ConnectionStrings:DefaultConnection' o 'Persistence:ConnectionString'.");
        }

        _connectionString = connectionString;
        CommandTimeoutSeconds = options.Value.CommandTimeoutSeconds;
    }

    public int CommandTimeoutSeconds { get; }

    public SqlConnection Create() => new(_connectionString);
}
