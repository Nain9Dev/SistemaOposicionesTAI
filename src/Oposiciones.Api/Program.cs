using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Oposiciones.Domain.Interfaces;
using Oposiciones.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Oposiciones.Application.Services;
using Oposiciones.Api.Middleware;
using Oposiciones.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("AuthLimiter", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
    
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(new { message = "Demasiados intentos. Por favor, espere 1 minuto." }, cancellationToken: token);
    };
});

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
var redisUrl = builder.Configuration["REDIS_URL"];
if (!string.IsNullOrEmpty(redisUrl))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisUrl;
        options.InstanceName = "OposicionesTAI_";
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey == "REPLACE_WITH_YOUR_SECRET_KEY" || jwtKey.Length < 32)
{
    throw new InvalidOperationException("La clave secreta JWT no está configurada o es demasiado corta. Debe tener al menos 32 caracteres para ser segura.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("access_token"))
                {
                    context.Token = context.Request.Cookies["access_token"];
                }
                return Task.CompletedTask;
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Sistema Oposiciones TAI API", Version = "v1", Description = "API Backend en .NET 10 para la preparación de Oposiciones TAI (INAP) con PostgreSQL." });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCors", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
    
    options.AddPolicy("ProductionCors", policy =>
    {
        policy.WithOrigins("https://tai.naindev.com", "https://tai-study-system.vercel.app", "https://tai-frontend.vercel.app", "https://nain9dev.github.io")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var cs = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
if (cs.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) || cs.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
{
    var uri = new Uri(cs);
    var userInfo = uri.UserInfo.Split(':');
    cs = $"Host={uri.Host};Port={(uri.Port > 0 ? uri.Port : 5432)};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true;";
}
else
{
    throw new InvalidOperationException("Solo se soporta conexión a PostgreSQL mediante URL (postgres://...).");
}

builder.Services.AddInfrastructureServices(cs);
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<Oposiciones.Application.Interfaces.IProgresoService, Oposiciones.Application.Services.ProgresoService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentCors");
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Oposiciones TAI v1"));
}
else
{
    app.UseCors("ProductionCors");
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CsrfValidationMiddleware>();

app.MapControllers();

app.Run();