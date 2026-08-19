using System.Threading.RateLimiting;
using Asp.Versioning;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using Oposiciones.Api.Configuration;
using Oposiciones.Api.Infrastructure;
using Oposiciones.Application;
using Oposiciones.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

ApiOptions apiOptions = builder.Configuration.GetSection(ApiOptions.SectionName).Get<ApiOptions>()
    ?? new ApiOptions();

builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection(ApiOptions.SectionName));

// --------------------------------------------------------------------------------------------
// Capas de la aplicacion. El host solo compone: no conoce SQL, ficheros ni reglas de correccion.
// --------------------------------------------------------------------------------------------
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, AppContext.BaseDirectory);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// --------------------------------------------------------------------------------------------
// Errores como ProblemDetails (RFC 9457) en lugar de paginas de error o 500 genericos.
// --------------------------------------------------------------------------------------------
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
});
builder.Services.AddExceptionHandler<DomainExceptionHandler>();

// --------------------------------------------------------------------------------------------
// Versionado de la Api. La version viaja en la ruta (api/v1/...) y los clientes que no la indican
// siguen funcionando contra la version por defecto.
// --------------------------------------------------------------------------------------------
builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("X-Api-Version"));
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sistema Oposiciones TAI API",
        Version = "v1",
        Description =
            "Motor de examinacion para oposiciones. Catalogo de temarios oficiales, banco de "
            + "preguntas con referencia normativa, generacion reproducible de tests y correccion "
            + "con el baremo de la convocatoria.",
    });

    options.CustomSchemaIds(type => type.FullName?.Replace('+', '.'));
});

// --------------------------------------------------------------------------------------------
// CORS. Los origenes se configuran; permitir cualquiera queda como opcion explicita de la demo.
// --------------------------------------------------------------------------------------------
const string CorsPolicy = "OposicionesCors";
builder.Services.AddCors(options => options.AddPolicy(CorsPolicy, policy =>
{
    if (apiOptions.Cors.AllowedOrigins.Length == 0)
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        return;
    }

    policy.WithOrigins(apiOptions.Cors.AllowedOrigins).AllowAnyMethod().AllowAnyHeader();

    if (apiOptions.Cors.AllowCredentials)
    {
        policy.AllowCredentials();
    }
}));

// --------------------------------------------------------------------------------------------
// Limitacion de peticiones por cliente. La generacion de tests es la operacion mas cara y no
// deberia poder saturarse desde un solo navegador en bucle.
// --------------------------------------------------------------------------------------------
if (apiOptions.RateLimit.Enabled)
{
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonimo",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = apiOptions.RateLimit.PermitLimit,
                    Window = TimeSpan.FromSeconds(apiOptions.RateLimit.WindowSeconds),
                    QueueLimit = apiOptions.RateLimit.QueueLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                }));
    });
}

// --------------------------------------------------------------------------------------------
// Cache de salida para el catalogo, que es identico para todos los clientes y cambia con cada
// convocatoria. Evita repetir la misma consulta en cada carga de la aplicacion web.
// --------------------------------------------------------------------------------------------
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("catalogo", policy => policy
        .Expire(TimeSpan.FromMinutes(10))
        .SetVaryByQuery("examCode", "blockCode"));
});

builder.Services.AddResponseCompression();

builder.Services.AddHealthChecks()
    .AddCheck<PersistenceHealthCheck>(
        "persistencia",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "ready" });

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseResponseCompression();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseCors(CorsPolicy);

if (apiOptions.RateLimit.Enabled)
{
    app.UseRateLimiter();
}

app.UseOutputCache();

if (app.Environment.IsDevelopment() || apiOptions.EnableSwaggerInProduction)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Oposiciones TAI v1");
        options.DocumentTitle = "Sistema Oposiciones TAI - API";
    });
}

// Sondas separadas: 'live' responde mientras el proceso este en pie, 'ready' solo cuando el
// almacen de datos responde. Es lo que necesita un orquestador para no enrutar trafico antes de
// tiempo ni reiniciar un contenedor que simplemente esta esperando a la base de datos.
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = WriteHealthResponseAsync,
});

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.MapControllers();

app.Run();

static Task WriteHealthResponseAsync(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json; charset=utf-8";

    var payload = new
    {
        status = report.Status.ToString(),
        totalDurationMs = report.TotalDuration.TotalMilliseconds,
        checks = report.Entries.Select(entry => new
        {
            name = entry.Key,
            status = entry.Value.Status.ToString(),
            description = entry.Value.Description,
            data = entry.Value.Data,
        }),
    };

    return context.Response.WriteAsJsonAsync(payload);
}

/// <summary>
/// Punto de entrada expuesto para que las pruebas de integracion puedan levantar la Api completa
/// con <c>WebApplicationFactory</c>.
/// </summary>
public partial class Program
{
}
