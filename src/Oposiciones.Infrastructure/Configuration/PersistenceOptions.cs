using System.ComponentModel.DataAnnotations;

namespace Oposiciones.Infrastructure.Configuration;

/// <summary>Proveedor de persistencia activo.</summary>
public enum PersistenceProvider
{
    /// <summary>
    /// Catalogo y banco cargados desde los ficheros JSON de <c>content/</c>, con los tests e
    /// intentos en memoria del proceso. Permite arrancar la Api sin ninguna infraestructura.
    /// </summary>
    InMemory = 0,

    /// <summary>SQL Server con Dapper y procedimientos almacenados.</summary>
    SqlServer = 1,
}

/// <summary>
/// Configuracion de persistencia. Cambiar de proveedor es cambiar una cadena en appsettings:
/// ni el dominio ni la capa de aplicacion saben cual esta activo.
/// </summary>
public sealed class PersistenceOptions
{
    public const string SectionName = "Persistence";

    public PersistenceProvider Provider { get; set; } = PersistenceProvider.InMemory;

    /// <summary>Nombre de la cadena de conexion dentro de <c>ConnectionStrings</c>.</summary>
    public string ConnectionStringName { get; set; } = "DefaultConnection";

    /// <summary>Cadena de conexion explicita. Tiene prioridad sobre <see cref="ConnectionStringName"/>.</summary>
    public string? ConnectionString { get; set; }

    [Range(1, 600)]
    public int CommandTimeoutSeconds { get; set; } = 30;

    /// <summary>Reintentos ante errores transitorios de SQL (fallos de red, throttling de Azure SQL).</summary>
    [Range(0, 10)]
    public int MaxRetryAttempts { get; set; } = 3;

    [Range(10, 10_000)]
    public int RetryBaseDelayMilliseconds { get; set; } = 200;

    /// <summary>Cachea el temario en memoria: cambia poco y se consulta en cada generacion de test.</summary>
    public bool EnableCatalogCache { get; set; } = true;

    [Range(1, 86_400)]
    public int CatalogCacheSeconds { get; set; } = 300;
}
