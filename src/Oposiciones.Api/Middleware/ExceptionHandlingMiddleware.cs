using System.Diagnostics;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Oposiciones.Domain.Exceptions;

namespace Oposiciones.Api.Middleware;

/// <summary>
/// Traduce cualquier excepcion no controlada a una respuesta ProblemDetails (RFC 9457).
///
/// Mejoras sobre la version anterior: los errores de negocio dejan de convertirse en 500,
/// se propaga un identificador de correlacion para poder cruzar la respuesta con el log,
/// las cancelaciones del cliente no se registran como fallo y nunca se intenta escribir el
/// cuerpo si la respuesta ya se habia empezado a enviar.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private const string CorrelationHeader = "X-Correlation-Id";

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ResolveCorrelationId(context);
        context.Response.Headers[CorrelationHeader] = correlationId;

        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // El cliente cerro la conexion: no es un fallo del servidor y no debe alertar.
            _logger.LogDebug("Peticion {Method} {Path} cancelada por el cliente.",
                context.Request.Method, context.Request.Path);
        }
        catch (Exception exception)
        {
            await HandleAsync(context, exception, correlationId);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception, string correlationId)
    {
        var problem = Translate(exception, correlationId);

        if (problem.Status >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception,
                "Error no controlado en {Method} {Path}. Correlacion: {CorrelationId}.",
                context.Request.Method, context.Request.Path, correlationId);
        }
        else
        {
            _logger.LogWarning(
                "Peticion rechazada en {Method} {Path} con estado {Status}. Correlacion: {CorrelationId}. {Detail}",
                context.Request.Method, context.Request.Path, problem.Status, correlationId, problem.Detail);
        }

        if (context.Response.HasStarted)
        {
            // Ya se envio la cabecera: sobrescribir el cuerpo provocaria una segunda excepcion.
            _logger.LogWarning("No se pudo emitir ProblemDetails: la respuesta ya habia comenzado.");
            return;
        }

        // No se llama a Response.Clear(): eliminaria las cabeceras CORS que ya inserto su
        // middleware y el navegador no podria leer el cuerpo del error.
        context.Response.ContentLength = null;
        context.Response.Headers[CorrelationHeader] = correlationId;
        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problem);
    }

    private ProblemDetails Translate(Exception exception, string correlationId)
    {
        var problem = exception switch
        {
            ValidationException validation => Build(
                StatusCodes.Status400BadRequest,
                "Datos de entrada no validos.",
                string.Join(" ", validation.Errors.Select(error => error.ErrorMessage))),

            BusinessRuleException business => Build(
                StatusCodes.Status400BadRequest,
                "Operacion no permitida.",
                business.Message),

            NotFoundException notFound => Build(
                StatusCodes.Status404NotFound,
                "Recurso no encontrado.",
                notFound.Message),

            ConflictException conflict => Build(
                StatusCodes.Status409Conflict,
                "Conflicto con el estado actual del recurso.",
                conflict.Message),

            ForbiddenException forbidden => Build(
                StatusCodes.Status403Forbidden,
                "Acceso denegado.",
                forbidden.Message),

            PostgresException postgres => TranslatePostgres(postgres),

            _ => Build(
                StatusCodes.Status500InternalServerError,
                "Ha ocurrido un error interno.",
                // Fuera de desarrollo no se filtra el detalle: puede contener SQL o rutas internas.
                _environment.IsDevelopment() ? exception.Message : "Consulte con el administrador citando el identificador de correlacion.")
        };

        problem.Extensions["correlationId"] = correlationId;
        return problem;
    }

    private ProblemDetails TranslatePostgres(PostgresException exception) => exception.SqlState switch
    {
        // unique_violation
        PostgresErrorCodes.UniqueViolation => Build(
            StatusCodes.Status409Conflict,
            "El recurso ya existe.",
            "Ya existe un registro con esos datos."),

        // foreign_key_violation
        PostgresErrorCodes.ForeignKeyViolation => Build(
            StatusCodes.Status400BadRequest,
            "Referencia no valida.",
            "Alguno de los identificadores indicados no existe."),

        // check_violation
        PostgresErrorCodes.CheckViolation => Build(
            StatusCodes.Status400BadRequest,
            "Datos incoherentes.",
            "Los valores enviados incumplen una restriccion de integridad."),

        // undefined_table / undefined_function: falta ejecutar las migraciones de db/.
        PostgresErrorCodes.UndefinedTable or PostgresErrorCodes.UndefinedFunction => Build(
            StatusCodes.Status503ServiceUnavailable,
            "Base de datos no inicializada.",
            "Faltan objetos de base de datos. Ejecute los scripts de la carpeta db/ en orden."),

        _ => Build(
            StatusCodes.Status500InternalServerError,
            "Error de base de datos.",
            _environment.IsDevelopment() ? exception.MessageText : "Error al acceder al almacen de datos.")
    };

    private static ProblemDetails Build(int status, string title, string detail) => new()
    {
        Status = status,
        Title = title,
        Detail = detail
    };

    private static string ResolveCorrelationId(HttpContext context)
    {
        var incoming = context.Request.Headers[CorrelationHeader].FirstOrDefault();
        return string.IsNullOrWhiteSpace(incoming)
            ? Activity.Current?.Id ?? context.TraceIdentifier
            : incoming;
    }
}
