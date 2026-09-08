using Microsoft.Extensions.Configuration;

namespace Oposiciones.Api.Security;

/// <summary>
/// Resolves the CORS allowlist from configuration.
///
/// Binding a string array straight from environment variables requires the indexed form
/// (<c>Cors__AllowedOrigins__0</c>, <c>__1</c>, …), which is easy to get wrong and gives no
/// feedback when you do: the list simply comes back empty and every browser request fails
/// with an opaque CORS error. Hosting dashboards also make a single comma-separated value
/// far more natural to enter.
///
/// Both forms are therefore accepted, and the result is validated so a malformed origin is
/// reported at start-up rather than at the first request.
/// </summary>
public static class CorsOrigins
{
    public const string SectionName = "Cors:AllowedOrigins";

    public static string[] Resolve(IConfiguration configuration, bool isDevelopment)
    {
        var origins = ReadCandidates(configuration)
            .Select(Normalize)
            .Where(IsValidOrigin)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (origins.Length > 0)
        {
            return origins;
        }

        // Local development gets the Vite dev and preview ports, so a fresh clone runs
        // without any configuration at all. Nothing is assumed anywhere else.
        return isDevelopment
            ? ["http://localhost:5173", "http://localhost:4173"]
            : [];
    }

    /// <summary>Origins present in configuration that were rejected, for start-up logging.</summary>
    public static string[] FindInvalid(IConfiguration configuration) =>
        ReadCandidates(configuration)
            .Select(Normalize)
            .Where(origin => !IsValidOrigin(origin))
            .ToArray();

    /// <summary>
    /// Reads the configured origins from whichever form is present.
    ///
    /// The scalar is checked first, and deliberately wins over the indexed array. Only an
    /// environment variable or a command-line switch can produce a scalar at this key —
    /// a JSON file naturally yields children — and those providers are meant to override
    /// the file. Reading the array first meant an operator setting
    /// <c>Cors__AllowedOrigins</c> in a hosting dashboard had it silently ignored in favour
    /// of whatever `appsettings.json` happened to contain.
    /// </summary>
    private static IEnumerable<string> ReadCandidates(IConfiguration configuration)
    {
        var scalar = configuration[SectionName];
        if (!string.IsNullOrWhiteSpace(scalar))
        {
            return SplitEntry(scalar);
        }

        // The array form may itself hold comma-separated entries, so it is split too.
        return configuration.GetSection(SectionName).Get<string[]>()?.SelectMany(SplitEntry) ?? [];
    }

    private static IEnumerable<string> SplitEntry(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? []
            : value.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    /// <summary>
    /// An origin is scheme, host and port. A trailing slash makes the browser's comparison
    /// fail, and it is the single most common way to get this wrong.
    /// </summary>
    private static string Normalize(string value) => value.Trim().TrimEnd('/');

    private static bool IsValidOrigin(string value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
        && string.IsNullOrEmpty(uri.AbsolutePath.TrimStart('/'))
        && string.IsNullOrEmpty(uri.Query);
}
