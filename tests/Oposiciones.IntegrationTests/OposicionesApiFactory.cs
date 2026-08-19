using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Oposiciones.IntegrationTests;

/// <summary>
/// Levanta la Api completa en memoria, con el proveedor de contenido real del repositorio.
/// <para>
/// Se desactiva la limitacion de peticiones porque una bateria de pruebas dispara muchas mas
/// llamadas por minuto que un opositor y, de no hacerlo, los ultimos casos fallarian con 429 sin
/// que hubiera ningun defecto real.
/// </para>
/// </summary>
public sealed class OposicionesApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration(configuration =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Persistence:Provider"] = "InMemory",
                ["Content:RootPath"] = "content",
                ["Content:FailOnInvalidContent"] = "true",
                ["Api:RateLimit:Enabled"] = "false",
            });
        });
    }
}

/// <summary>Comparte una unica instancia de la Api entre todas las pruebas de la coleccion.</summary>
[CollectionDefinition("api")]
public sealed class ApiCollection : ICollectionFixture<OposicionesApiFactory>
{
}
