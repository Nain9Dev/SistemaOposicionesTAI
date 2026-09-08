using System.Diagnostics;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Oposiciones.Infrastructure.Data;

namespace Oposiciones.Api.Controllers;

/// <summary>
/// Sondas de disponibilidad. Antes reconstruia la cadena de conexion por su cuenta,
/// duplicando el traductor de URL de PostgreSQL y devolviendo el error crudo al cliente.
/// </summary>
[ApiController]
[Route("api/health")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<HealthController> _logger;

    public HealthController(IDbConnectionFactory connectionFactory, ILogger<HealthController> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    /// <summary>Sonda de vida: responde sin tocar dependencias externas.</summary>
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        status = "ok",
        service = "Oposiciones.Api",
        utc = DateTime.UtcNow
    });

    /// <summary>Sonda de disponibilidad: comprueba que la base de datos responde.</summary>
    [HttpGet("db")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Db(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await using var connection = await _connectionFactory.OpenAsync(cancellationToken);
            await connection.ExecuteScalarAsync<int>(
                new CommandDefinition("SELECT 1;", cancellationToken: cancellationToken));

            stopwatch.Stop();

            return Ok(new
            {
                status = "ok",
                database = "postgresql",
                latencyMs = stopwatch.ElapsedMilliseconds
            });
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            stopwatch.Stop();
            _logger.LogError(exception, "La sonda de base de datos ha fallado.");

            // El detalle del fallo se queda en el log: expone host, usuario y esquema.
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                status = "unavailable",
                database = "postgresql",
                latencyMs = stopwatch.ElapsedMilliseconds
            });
        }
    }
}
