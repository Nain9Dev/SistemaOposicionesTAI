using Npgsql;
using Oposiciones.Infrastructure.Data;
using Xunit;

namespace Oposiciones.UnitTests;

/// <summary>
/// Los proveedores gestionados (Neon, Render, Railway) entregan la credencial como URL
/// "postgres://", formato que Npgsql no acepta. El traductor estaba duplicado en Program.cs
/// y en el controlador de salud, con reglas de TLS distintas en cada copia.
/// </summary>
public class PostgresConnectionStringTests
{
    [Fact]
    public void Traduce_la_url_a_pares_clave_valor()
    {
        var result = PostgresConnectionString.Normalize(
            "postgres://usuario:secreto@db.example.com:6543/mibase?sslmode=require");

        var builder = new NpgsqlConnectionStringBuilder(result);

        Assert.Equal("db.example.com", builder.Host);
        Assert.Equal(6543, builder.Port);
        Assert.Equal("mibase", builder.Database);
        Assert.Equal("usuario", builder.Username);
        Assert.Equal("secreto", builder.Password);
        Assert.Equal(SslMode.Require, builder.SslMode);
    }

    [Fact]
    public void Aplica_el_puerto_por_defecto_de_postgres()
    {
        var builder = new NpgsqlConnectionStringBuilder(
            PostgresConnectionString.Normalize("postgresql://u:p@localhost/basedatos"));

        Assert.Equal(5432, builder.Port);
    }

    [Fact]
    public void Exige_tls_cuando_la_url_no_indica_sslmode()
    {
        // Por defecto se cifra: una cadena sin sslmode no debe acabar viajando en claro.
        var builder = new NpgsqlConnectionStringBuilder(
            PostgresConnectionString.Normalize("postgres://u:p@host/db"));

        Assert.Equal(SslMode.Require, builder.SslMode);
    }

    [Fact]
    public void Respeta_sslmode_disable_para_desarrollo_local()
    {
        var builder = new NpgsqlConnectionStringBuilder(
            PostgresConnectionString.Normalize("postgres://u:p@localhost:5432/db?sslmode=disable"));

        Assert.Equal(SslMode.Disable, builder.SslMode);
    }

    [Fact]
    public void Descodifica_las_credenciales_con_caracteres_escapados()
    {
        var builder = new NpgsqlConnectionStringBuilder(
            PostgresConnectionString.Normalize("postgres://us%40er:p%40ss%3Aword@host/db"));

        Assert.Equal("us@er", builder.Username);
        Assert.Equal("p@ss:word", builder.Password);
    }

    [Fact]
    public void Acepta_el_formato_clave_valor_sin_tocarlo()
    {
        var builder = new NpgsqlConnectionStringBuilder(
            PostgresConnectionString.Normalize("Host=localhost;Database=db;Username=u;Password=p"));

        Assert.Equal("localhost", builder.Host);
        Assert.Equal("db", builder.Database);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Una_cadena_vacia_falla_al_arrancar_y_no_en_la_primera_peticion(string? value)
    {
        Assert.Throws<InvalidOperationException>(() => PostgresConnectionString.Normalize(value));
    }
}
