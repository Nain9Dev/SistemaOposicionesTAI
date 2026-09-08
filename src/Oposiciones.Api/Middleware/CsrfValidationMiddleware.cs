using Oposiciones.Api.Extensions;
using Oposiciones.Api.Security;

namespace Oposiciones.Api.Middleware;

/// <summary>
/// Valida el token CSRF en las peticiones que mutan estado. Es necesario porque el JWT viaja
/// en cookie: el navegador la adjunta sola en peticiones cross-site, asi que la cookie por si
/// misma no prueba intencion del usuario. La cabecera si, porque otro origen no puede fijarla.
/// </summary>
public class CsrfValidationMiddleware
{
    public const string HeaderName = "X-CSRF-Token";

    private static readonly string[] ExemptPaths =
    [
        "/api/auth/login",
        "/api/auth/register",
        "/api/auth/refresh"
    ];

    private readonly RequestDelegate _next;
    private readonly ILogger<CsrfValidationMiddleware> _logger;

    public CsrfValidationMiddleware(RequestDelegate next, ILogger<CsrfValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ICsrfTokenService csrfTokenService)
    {
        if (!RequiresValidation(context))
        {
            await _next(context);
            return;
        }

        var sessionId = context.User.GetSessionId();
        if (string.IsNullOrEmpty(sessionId))
        {
            // Token de acceso sin jti: procede de una version anterior del emisor y no puede
            // anclarse a una sesion. Se rechaza para no dejar un hueco sin proteccion CSRF.
            await WriteForbiddenAsync(context, "Sesion no valida para operaciones de escritura. Inicie sesion de nuevo.");
            return;
        }

        var headerToken = context.Request.Headers[HeaderName].FirstOrDefault();
        if (!await csrfTokenService.ValidateAsync(sessionId, headerToken, context.RequestAborted))
        {
            _logger.LogWarning(
                "Token CSRF invalido o ausente en {Method} {Path}.",
                context.Request.Method,
                context.Request.Path);

            await WriteForbiddenAsync(context, "Token CSRF ausente o invalido.");
            return;
        }

        await _next(context);
    }

    private static bool RequiresValidation(HttpContext context)
    {
        var method = context.Request.Method;

        // Los metodos seguros no mutan estado y quedan fuera del alcance de CSRF.
        if (HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsOptions(method) || HttpMethods.IsTrace(method))
        {
            return false;
        }

        // Sin sesion no hay privilegio que suplantar; la autorizacion se encarga del resto.
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        var path = context.Request.Path.Value;
        if (path is null)
        {
            return true;
        }

        foreach (var exempt in ExemptPaths)
        {
            if (path.StartsWith(exempt, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    private static Task WriteForbiddenAsync(HttpContext context, string message)
    {
        if (context.Response.HasStarted)
        {
            return Task.CompletedTask;
        }

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return context.Response.WriteAsJsonAsync(new
        {
            type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.4",
            title = "Peticion rechazada por proteccion CSRF.",
            status = StatusCodes.Status403Forbidden,
            detail = message
        });
    }
}
