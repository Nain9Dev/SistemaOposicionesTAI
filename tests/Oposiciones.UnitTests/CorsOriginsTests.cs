using Microsoft.Extensions.Configuration;
using Oposiciones.Api.Security;
using Xunit;

namespace Oposiciones.UnitTests;

/// <summary>
/// The deployment guide told operators to set <c>CORS__AllowedOrigins</c> to a
/// comma-separated string. Binding to <c>string[]</c> needs the indexed form, so that value
/// produced an empty allowlist — and the only symptom is an opaque CORS error in the
/// browser, with the API itself looking perfectly healthy.
/// </summary>
public class CorsOriginsTests
{
    private static IConfiguration Config(params (string Key, string Value)[] entries) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(entries.Select(e => new KeyValuePair<string, string?>(e.Key, e.Value)))
            .Build();

    [Fact]
    public void Acepta_la_forma_indexada_de_variables_de_entorno()
    {
        var origins = CorsOrigins.Resolve(
            Config(
                ("Cors:AllowedOrigins:0", "https://tai.naindev.com"),
                ("Cors:AllowedOrigins:1", "https://otra.example")),
            isDevelopment: false);

        Assert.Equal(["https://tai.naindev.com", "https://otra.example"], origins);
    }

    [Fact]
    public void Acepta_una_lista_separada_por_comas()
    {
        var origins = CorsOrigins.Resolve(
            Config(("Cors:AllowedOrigins", "https://tai.naindev.com, https://otra.example")),
            isDevelopment: false);

        Assert.Equal(["https://tai.naindev.com", "https://otra.example"], origins);
    }

    [Fact]
    public void Elimina_la_barra_final_que_rompe_la_comparacion_del_navegador()
    {
        // El navegador compara origen exacto: "https://x.com/" nunca coincide con "https://x.com".
        var origins = CorsOrigins.Resolve(
            Config(("Cors:AllowedOrigins", "https://tai.naindev.com/")),
            isDevelopment: false);

        Assert.Equal(["https://tai.naindev.com"], origins);
    }

    [Fact]
    public void Descarta_un_origen_con_ruta()
    {
        var origins = CorsOrigins.Resolve(
            Config(("Cors:AllowedOrigins", "https://tai.naindev.com/api")),
            isDevelopment: false);

        Assert.Empty(origins);
    }

    [Theory]
    [InlineData("tai.naindev.com")]
    [InlineData("ftp://tai.naindev.com")]
    [InlineData("no es una url")]
    public void Descarta_valores_que_no_son_un_origen_http(string value)
    {
        Assert.Empty(CorsOrigins.Resolve(Config(("Cors:AllowedOrigins", value)), isDevelopment: false));
    }

    [Fact]
    public void Informa_de_los_origenes_descartados_para_poder_diagnosticarlos()
    {
        var invalid = CorsOrigins.FindInvalid(
            Config(("Cors:AllowedOrigins", "https://valido.example, no-es-una-url")));

        Assert.Equal(["no-es-una-url"], invalid);
    }

    [Fact]
    public void No_duplica_origenes_repetidos()
    {
        var origins = CorsOrigins.Resolve(
            Config(("Cors:AllowedOrigins", "https://x.example, https://x.example/, https://X.example")),
            isDevelopment: false);

        Assert.Single(origins);
    }

    [Fact]
    public void En_desarrollo_asume_los_puertos_de_vite()
    {
        var origins = CorsOrigins.Resolve(Config(), isDevelopment: true);

        Assert.Contains("http://localhost:5173", origins);
        Assert.Contains("http://localhost:4173", origins);
    }

    [Fact]
    public void Fuera_de_desarrollo_no_asume_ningun_origen()
    {
        // Un valor por defecto en produccion seria una lista blanca que nadie ha aprobado.
        Assert.Empty(CorsOrigins.Resolve(Config(), isDevelopment: false));
    }
}
