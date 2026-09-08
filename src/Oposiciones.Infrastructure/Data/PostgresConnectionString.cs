using Npgsql;

namespace Oposiciones.Infrastructure.Data;

/// <summary>
/// Normaliza la cadena de conexion a PostgreSQL. Los proveedores gestionados (Neon, Render,
/// Railway, Heroku) publican la credencial como URL "postgres://", formato que Npgsql no acepta
/// directamente. Este traductor vivia duplicado en Program.cs y en HealthController.
/// </summary>
public static class PostgresConnectionString
{
    public static string Normalize(string? rawConnectionString)
    {
        if (string.IsNullOrWhiteSpace(rawConnectionString))
        {
            throw new InvalidOperationException(
                "No hay cadena de conexion configurada (ConnectionStrings:DefaultConnection).");
        }

        var raw = rawConnectionString.Trim();

        if (!IsUrlFormat(raw))
        {
            // Ya viene en formato clave=valor: se valida delegando en el propio parser de Npgsql.
            return new NpgsqlConnectionStringBuilder(raw).ConnectionString;
        }

        var uri = new Uri(raw);
        var userInfo = uri.UserInfo.Split(':', 2);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty
        };

        // sslmode de la query string manda; si no viene, se exige TLS por defecto.
        builder.SslMode = ReadQueryValue(uri.Query, "sslmode")?.ToLowerInvariant() switch
        {
            "disable" => SslMode.Disable,
            "allow" => SslMode.Allow,
            "prefer" => SslMode.Prefer,
            "verify-ca" => SslMode.VerifyCA,
            "verify-full" => SslMode.VerifyFull,
            _ => SslMode.Require
        };

        return builder.ConnectionString;
    }

    private static bool IsUrlFormat(string value) =>
        value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
        value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase);

    private static string? ReadQueryValue(string query, string key)
    {
        foreach (var pair in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = pair.IndexOf('=');
            if (separator <= 0)
            {
                continue;
            }

            if (pair.AsSpan(0, separator).Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                return Uri.UnescapeDataString(pair[(separator + 1)..]);
            }
        }

        return null;
    }
}
