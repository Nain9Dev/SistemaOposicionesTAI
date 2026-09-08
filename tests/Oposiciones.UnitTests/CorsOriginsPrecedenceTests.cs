using Microsoft.Extensions.Configuration;
using Oposiciones.Api.Security;
using Xunit;

namespace Oposiciones.UnitTests;

/// <summary>
/// Precedence between the two accepted forms.
///
/// This was found by running the container with `Cors__AllowedOrigins` set and watching the
/// start-up log report the origins from `appsettings.json` instead. In .NET configuration a
/// JSON array produces children under the key while an environment variable produces a
/// value at the key itself, so reading the array first meant the deployment's own setting
/// was silently ignored.
/// </summary>
public class CorsOriginsPrecedenceTests
{
    /// <summary>Mimics a JSON file's array plus an environment variable's scalar.</summary>
    private static IConfiguration WithBothForms(string scalar) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new("Cors:AllowedOrigins:0", "https://del-fichero.example"),
                new("Cors:AllowedOrigins:1", "https://tambien-del-fichero.example"),
            ])
            .AddInMemoryCollection([new("Cors:AllowedOrigins", scalar)])
            .Build();

    [Fact]
    public void El_valor_de_entorno_sobrescribe_el_array_del_fichero()
    {
        var origins = CorsOrigins.Resolve(WithBothForms("https://del-entorno.example"), isDevelopment: false);

        Assert.Equal(["https://del-entorno.example"], origins);
    }

    [Fact]
    public void Un_valor_de_entorno_vacio_no_anula_la_configuracion_del_fichero()
    {
        // Una variable declarada pero vacia es casi siempre un descuido, no la intencion
        // de quedarse sin ningun origen permitido.
        var origins = CorsOrigins.Resolve(WithBothForms("   "), isDevelopment: false);

        Assert.Equal(2, origins.Length);
    }

    [Fact]
    public void Sin_valor_escalar_se_usa_el_array()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection([new("Cors:AllowedOrigins:0", "https://del-fichero.example")])
            .Build();

        Assert.Equal(["https://del-fichero.example"], CorsOrigins.Resolve(configuration, isDevelopment: false));
    }
}
