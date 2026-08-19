using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Oposiciones.Domain.Abstractions;
using Oposiciones.Domain.Catalog;
using Oposiciones.Infrastructure.Configuration;
using Oposiciones.Infrastructure.SqlServer;

namespace Oposiciones.Api.Infrastructure;

/// <summary>
/// Comprobacion de disponibilidad del almacen activo.
/// <para>
/// No se limita a abrir una conexion: verifica que el catalogo responde y que hay temario cargado.
/// Una base accesible pero vacia no es un sistema sano, y un despliegue no deberia recibir trafico
/// hasta que pueda generar tests de verdad.
/// </para>
/// </summary>
public sealed class PersistenceHealthCheck : IHealthCheck
{
    private readonly IExamCatalogRepository _catalog;
    private readonly PersistenceOptions _options;
    private readonly ISqlConnectionFactory? _connections;

    public PersistenceHealthCheck(
        IExamCatalogRepository catalog,
        IOptions<PersistenceOptions> options,
        IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(services);

        _catalog = catalog;
        _options = options.Value;
        _connections = services.GetService(typeof(ISqlConnectionFactory)) as ISqlConnectionFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (_options.Provider == PersistenceProvider.SqlServer && _connections is not null)
            {
                await using SqlConnection connection = _connections.Create();
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            }

            IReadOnlyList<ExamProfile> exams =
                await _catalog.GetExamsAsync(cancellationToken).ConfigureAwait(false);

            var data = new Dictionary<string, object>
            {
                ["provider"] = _options.Provider.ToString(),
                ["exams"] = exams.Count,
            };

            return exams.Count == 0
                ? HealthCheckResult.Degraded("No hay convocatorias cargadas en el catalogo.", data: data)
                : HealthCheckResult.Healthy("Catalogo disponible.", data);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return HealthCheckResult.Unhealthy("No se ha podido consultar el almacen de datos.", ex);
        }
    }
}
