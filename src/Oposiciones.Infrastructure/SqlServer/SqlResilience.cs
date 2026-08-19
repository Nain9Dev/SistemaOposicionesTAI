using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Oposiciones.Infrastructure.Configuration;

namespace Oposiciones.Infrastructure.SqlServer;

/// <summary>
/// Reintenta operaciones que han fallado por un error transitorio de SQL Server.
/// <para>
/// Solo se reintentan errores conocidos de red, de conexion o de limitacion de recursos (los
/// tipicos de Azure SQL). Un error de datos o de sintaxis no se reintenta nunca: repetirlo daria
/// exactamente el mismo fallo y solo serviria para retrasar el mensaje de error.
/// </para>
/// </summary>
public sealed class SqlResilience
{
    /// <summary>Codigos de error de SQL Server considerados transitorios.</summary>
    private static readonly HashSet<int> TransientErrorNumbers = new()
    {
        -2,     // Timeout expirado
        20,     // Fallo en el handshake de la instancia
        64,     // Error de conexion durante el inicio de sesion
        233,    // Conexion cerrada por el servidor
        4060,   // No se pudo abrir la base de datos solicitada
        4221,   // Login a la replica de lectura fallido, no hay quorum
        10053,  // Conexion abortada por el software del host
        10054,  // Conexion reiniciada por el interlocutor
        10060,  // Error de red al establecer la conexion
        10928,  // Limite de recursos alcanzado (Azure SQL)
        10929,  // El servidor esta ocupado (Azure SQL)
        40197,  // Error de servicio durante el procesamiento
        40501,  // Servicio limitado (throttling)
        40613,  // Base de datos no disponible temporalmente
        49918,  // No hay recursos suficientes para procesar la peticion
        49919,  // Demasiadas operaciones de creacion o actualizacion en curso
        49920,  // Demasiadas operaciones en curso
    };

    private readonly int _maxAttempts;
    private readonly int _baseDelayMilliseconds;
    private readonly ILogger<SqlResilience> _logger;

    public SqlResilience(IOptions<PersistenceOptions> options, ILogger<SqlResilience> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        _maxAttempts = Math.Max(1, options.Value.MaxRetryAttempts + 1);
        _baseDelayMilliseconds = options.Value.RetryBaseDelayMilliseconds;
        _logger = logger;
    }

    public async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(operation);

        for (int attempt = 1; ; attempt++)
        {
            try
            {
                return await operation(cancellationToken).ConfigureAwait(false);
            }
            catch (SqlException ex) when (attempt < _maxAttempts && IsTransient(ex))
            {
                // Espera exponencial: 200 ms, 400 ms, 800 ms... para no insistir sobre un servidor
                // que ya esta saturado.
                TimeSpan delay = TimeSpan.FromMilliseconds(_baseDelayMilliseconds * Math.Pow(2, attempt - 1));

                _logger.LogWarning(
                    ex,
                    "Error transitorio de SQL Server (numero {Number}) en el intento {Attempt} de {MaxAttempts}. Reintentando en {Delay} ms.",
                    ex.Number,
                    attempt,
                    _maxAttempts,
                    delay.TotalMilliseconds);

                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    public Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken) =>
        ExecuteAsync<object?>(async token =>
        {
            await operation(token).ConfigureAwait(false);
            return null;
        }, cancellationToken);

    private static bool IsTransient(SqlException exception)
    {
        foreach (SqlError error in exception.Errors)
        {
            if (TransientErrorNumbers.Contains(error.Number))
            {
                return true;
            }
        }

        return TransientErrorNumbers.Contains(exception.Number);
    }
}
