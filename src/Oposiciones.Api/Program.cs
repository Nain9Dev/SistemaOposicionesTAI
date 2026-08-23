using Oposiciones.Domain.Interfaces;
using Oposiciones.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Oposiciones.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "OposicionesTAI",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "OposicionesTAIUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "ClaveSuperSecretaDeDesarrolloTAI2026!+*"))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Sistema Oposiciones TAI API", Version = "v1", Description = "API Backend en .NET 10 para la preparación de Oposiciones TAI (INAP) con arquitectura Dapper y T-SQL." });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://nain9dev.github.io", "https://tai-study-system.vercel.app", "https://tai-frontend.vercel.app", "https://tai.naindev.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var cs = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(local);Database=OposicionesTAI;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddScoped<ISyllabusRepository>(_ => new SyllabusRepository(cs));
builder.Services.AddScoped<ITestRepository>(_ => new TestRepository(cs));
builder.Services.AddScoped<IAttemptRepository>(_ => new AttemptRepository(cs));
builder.Services.AddScoped<IUsuarioRepository>(_ => new UsuarioRepository(cs));
builder.Services.AddScoped<IProgresoRepository>(_ => new ProgresoRepository(cs));
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Oposiciones TAI v1"));
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();