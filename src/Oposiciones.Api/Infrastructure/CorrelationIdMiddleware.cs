namespace Oposiciones.Api.Infrastructure;

/// <summary>
/// Propaga un identificador de correlacion por peticion.
/// <para>
/// Lo toma de la cabecera <c>X-Correlation-Id</c> si el cliente la envia y lo devuelve siempre en
/// la respuesta. Con varias instancias detras de un balanceador es la unica forma practica de
/// seguir el rastro de una peticion concreta entre los registros de todas ellas.
/// </para>
/// </summary>
public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        string correlationId = context.Request.Headers.TryGetValue(HeaderName, out var provided)
            && !string.IsNullOrWhiteSpace(provided)
                ? provided.ToString()
                : context.TraceIdentifier;

        context.TraceIdentifier = correlationId;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            await _next(context).ConfigureAwait(false);
        }
    }
}
