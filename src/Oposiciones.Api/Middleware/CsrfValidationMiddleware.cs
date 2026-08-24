using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace Oposiciones.Api.Middleware;

public class CsrfValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDistributedCache _cache;

    public CsrfValidationMiddleware(RequestDelegate next, IDistributedCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Solo validar peticiones que mutan estado
        var method = context.Request.Method;
        if (HttpMethods.IsPost(method) || HttpMethods.IsPut(method) || HttpMethods.IsDelete(method) || HttpMethods.IsPatch(method))
        {
            var path = context.Request.Path.Value?.ToLower();
            // Ignorar endpoints de autenticación porque aún no tienen sesión
            if (path != null && (path.Contains("/api/auth/login") || path.Contains("/api/auth/register") || path.Contains("/api/auth/refresh")))
            {
                await _next(context);
                return;
            }

            // Si el usuario está autenticado, validar token CSRF
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    var tokenFromHeader = context.Request.Headers["X-CSRF-Token"].FirstOrDefault();
                    
                    if (string.IsNullOrEmpty(tokenFromHeader))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(new { message = "CSRF Token missing." });
                        return;
                    }

                    var storedToken = await _cache.GetStringAsync($"csrf_{userId}");
                    if (string.IsNullOrEmpty(storedToken) || storedToken != tokenFromHeader)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(new { message = "Invalid CSRF Token." });
                        return;
                    }
                }
            }
        }

        await _next(context);
    }
}
