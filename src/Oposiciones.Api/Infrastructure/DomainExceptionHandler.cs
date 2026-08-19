using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Oposiciones.Application.Common;
using Oposiciones.Domain.Common;

namespace Oposiciones.Api.Infrastructure;

/// <summary>
/// Traduce las excepciones del dominio a respuestas <c>ProblemDetails</c> segun RFC 9457.
/// <para>
/// Antes cualquier error de negocio (un examen inexistente, un filtro invalido) salia como un 500
/// generico. Aqui cada tipo recibe el codigo HTTP que le corresponde, y solo lo inesperado se
/// convierte en 500, registrandose con su traza completa.
/// </para>
/// </summary>
public sealed class DomainExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetails;
    private readonly ILogger<DomainExceptionHandler> _logger;

    public DomainExceptionHandler(
        IProblemDetailsService problemDetails,
        ILogger<DomainExceptionHandler> logger)
    {
        _problemDetails = problemDetails;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        (int status, string title) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Peticion invalida"),
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
            InsufficientQuestionsException => (StatusCodes.Status409Conflict, "Banco de preguntas insuficiente"),
            DomainException => (StatusCodes.Status400BadRequest, "Operacion no permitida"),
            OperationCanceledException => (StatusCodesExtra.ClientClosedRequest, "Peticion cancelada"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno"),
        };

        if (status >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Error no controlado procesando {Method} {Path}.",
                httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            _logger.LogInformation("Peticion rechazada ({Status}): {Message}", status, exception.Message);
        }

        httpContext.Response.StatusCode = status;

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            // Un 500 no revela detalles internos al cliente; el diagnostico queda en el log.
            Detail = status >= StatusCodes.Status500InternalServerError
                ? "Se ha producido un error inesperado. Consulte los registros del servidor."
                : exception.Message,
            Instance = httpContext.Request.Path,
        };

        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        if (exception is ValidationException validation)
        {
            problem.Extensions["errors"] = validation.Errors;
        }

        if (exception is InsufficientQuestionsException insufficient)
        {
            problem.Extensions["requested"] = insufficient.Requested;
            problem.Extensions["available"] = insufficient.Available;
        }

        return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception,
        }).ConfigureAwait(false);
    }
}

/// <summary>Codigos de estado no incluidos en <see cref="StatusCodes"/>.</summary>
internal static class StatusCodesExtra
{
    /// <summary>499: el cliente cerro la conexion antes de recibir la respuesta.</summary>
    public const int ClientClosedRequest = 499;
}
