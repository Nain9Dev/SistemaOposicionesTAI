using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Oposiciones.Api.Security;

public static class RateLimitPolicies
{
    /// <summary>Login, registro y rotacion de sesion: superficie de fuerza bruta.</summary>
    public const string Auth = "AuthLimiter";

    /// <summary>Endpoints de lectura abiertos, como el banco de preguntas.</summary>
    public const string Public = "PublicLimiter";

    public sealed class Settings
    {
        public const string SectionName = "RateLimiting";

        public int AuthPermitLimit { get; set; } = 10;
        public int AuthWindowSeconds { get; set; } = 60;
        public int PublicPermitLimit { get; set; } = 120;
        public int PublicWindowSeconds { get; set; } = 60;
    }

    /// <summary>
    /// Registra los limitadores particionados por cliente.
    ///
    /// La version anterior usaba AddFixedWindowLimiter con un unico limitador compartido por
    /// todas las peticiones de la politica: cinco intentos de login al minuto en total, para
    /// todo el mundo. Bastaba con que un atacante gastase la cuota para bloquear el acceso a
    /// los demas usuarios. Ahora la cuota es por direccion de origen.
    /// </summary>
    public static void Configure(RateLimiterOptions options, Settings settings)
    {
        options.AddPolicy(Auth, context => Partition(
            context,
            settings.AuthPermitLimit,
            TimeSpan.FromSeconds(settings.AuthWindowSeconds)));

        options.AddPolicy(Public, context => Partition(
            context,
            settings.PublicPermitLimit,
            TimeSpan.FromSeconds(settings.PublicWindowSeconds)));

        options.OnRejected = async (context, token) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
                context.HttpContext.Response.Headers.RetryAfter =
                    ((int)retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture);
            }

            await context.HttpContext.Response.WriteAsJsonAsync(
                new { message = "Demasiadas peticiones. Espere un momento e intentelo de nuevo." },
                cancellationToken: token);
        };
    }

    private static RateLimitPartition<string> Partition(HttpContext context, int permitLimit, TimeSpan window) =>
        RateLimitPartition.GetFixedWindowLimiter(
            ResolveClientKey(context),
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = window,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });

    /// <summary>
    /// Clave de particion del cliente. Se prefiere el usuario autenticado y se cae a la IP
    /// remota, que detras de un proxy exige tener configurado ForwardedHeaders para no
    /// agrupar a todo el trafico bajo la direccion del balanceador.
    /// </summary>
    private static string ResolveClientKey(HttpContext context) =>
        context.User.Identity?.IsAuthenticated == true
            ? $"user:{context.User.Identity.Name}"
            : $"ip:{context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";
}
