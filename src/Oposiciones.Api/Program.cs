using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Oposiciones.Api.Extensions;
using Oposiciones.Api.Middleware;
using Oposiciones.Api.Security;
using Oposiciones.Application;
using Oposiciones.Infrastructure;
using Oposiciones.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Configuracion obligatoria: se valida al arrancar, no en la primera peticion.
// ---------------------------------------------------------------------------
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey == "REPLACE_WITH_YOUR_SECRET_KEY" || jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "La clave secreta JWT no esta configurada o es demasiado corta (minimo 32 caracteres). " +
        "Definala en Jwt:Key o en la variable de entorno Jwt__Key.");
}

// El traductor de cadenas de conexion vive en Infrastructure y admite tanto el formato
// URL de los proveedores gestionados como el formato clave=valor.
var connectionString = PostgresConnectionString.Normalize(
    builder.Configuration.GetConnectionString("DefaultConnection"));

// ---------------------------------------------------------------------------
// Servicios
// ---------------------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddResponseCaching();
builder.Services.AddProblemDetails();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Sistema Oposiciones TAI API",
        Version = "v1",
        Description = "API .NET 10 para la preparacion de Oposiciones TAI (INAP) sobre PostgreSQL."
    });
});

// Cache distribuida: Redis si esta configurado, memoria en caso contrario.
var redisUrl = builder.Configuration["REDIS_URL"];
if (!string.IsNullOrWhiteSpace(redisUrl))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisUrl;
        options.InstanceName = "OposicionesTAI_";
    });
}
else
{
    // Aviso explicito: con varias instancias los tokens CSRF no se comparten entre ellas.
    builder.Services.AddDistributedMemoryCache();
}

var rateLimitSettings = builder.Configuration
    .GetSection(RateLimitPolicies.Settings.SectionName)
    .Get<RateLimitPolicies.Settings>() ?? new RateLimitPolicies.Settings();

builder.Services.AddRateLimiter(options => RateLimitPolicies.Configure(options, rateLimitSettings));

// Las cookies cross-site solo tienen sentido cuando cliente y API viven en dominios distintos.
// En desarrollo sobre http://localhost, Secure + SameSite=None hace que el navegador las descarte.
builder.Services.Configure<AuthCookieOptions>(builder.Configuration.GetSection(AuthCookieOptions.SectionName));
builder.Services.PostConfigure<AuthCookieOptions>(options =>
{
    if (!builder.Configuration.GetSection(AuthCookieOptions.SectionName).Exists())
    {
        options.CrossSite = !builder.Environment.IsDevelopment();
    }
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // El token no viaja en Authorization: se lee de la cookie HttpOnly.
                if (context.Request.Cookies.TryGetValue(AuthCookieOptions.AccessTokenCookie, out var token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            },

            OnTokenValidated = async context =>
            {
                // La firma y la caducidad son validas, pero la sesion puede haberse cerrado.
                var sessionId = context.Principal?.GetSessionId();
                if (string.IsNullOrEmpty(sessionId))
                {
                    return;
                }

                var revocations = context.HttpContext.RequestServices.GetRequiredService<ISessionRevocationStore>();
                if (await revocations.IsRevokedAsync(sessionId, context.HttpContext.RequestAborted))
                {
                    context.Fail("La sesion se ha cerrado.");
                }
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "OposicionesTAI",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "OposicionesTAIUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            // Sin margen de holgura: una caducidad de 60 minutos no debe durar 65.
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

const string CorsPolicy = "TaiClient";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                     ?? (builder.Environment.IsDevelopment()
                         ? ["http://localhost:5173", "http://localhost:4173"]
                         : []);

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        // AllowCredentials es imprescindible para las cookies y prohibe el comodin de origen:
        // la lista blanca se declara en configuracion, no en codigo.
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .WithHeaders("Content-Type", "Accept", CsrfValidationMiddleware.HeaderName, "X-Correlation-Id")
              .WithExposedHeaders("X-Correlation-Id")
              .AllowCredentials();
    });
});

builder.Services.AddInfrastructureServices(connectionString);
builder.Services.AddApplicationServices();

builder.Services.AddSingleton<ICsrfTokenService, CsrfTokenService>();
builder.Services.AddSingleton<ISessionRevocationStore, SessionRevocationStore>();
builder.Services.AddSingleton<AuthCookieWriter>();

var app = builder.Build();

if (allowedOrigins.Length == 0)
{
    app.Logger.LogWarning(
        "No hay origenes CORS configurados (Cors:AllowedOrigins). El cliente web no podra consumir la API.");
}

if (string.IsNullOrWhiteSpace(redisUrl) && !app.Environment.IsDevelopment())
{
    app.Logger.LogWarning(
        "REDIS_URL no configurado: la cache es local al proceso. Con mas de una instancia, " +
        "los tokens CSRF emitidos por una no seran validos en las demas.");
}

// ---------------------------------------------------------------------------
// Canalizacion. El orden importa:
//   errores -> CORS -> limitador -> autenticacion -> autorizacion -> CSRF -> endpoints
// El manejador de errores va primero para envolver todo lo demas; CORS antes del limitador
// para que un 429 tambien llegue al navegador con las cabeceras correctas.
// ---------------------------------------------------------------------------
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Oposiciones TAI v1"));
}
else
{
    app.UseHsts();
}

app.UseCors(CorsPolicy);
app.UseResponseCaching();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CsrfValidationMiddleware>();

app.MapControllers();

app.Run();

/// <summary>Expuesto para las pruebas de integracion con WebApplicationFactory.</summary>
public partial class Program;
