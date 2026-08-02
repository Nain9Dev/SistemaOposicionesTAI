using Oposiciones.Domain.Interfaces;
using Oposiciones.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Sistema Oposiciones TAI API", Version = "v1", Description = "API Backend en .NET 10 para la preparación de Oposiciones TAI (INAP) con arquitectura Dapper y T-SQL." });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var cs = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(local);Database=OposicionesTAI;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddScoped<ISyllabusRepository>(_ => new SyllabusRepository(cs));
builder.Services.AddScoped<ITestRepository>(_ => new TestRepository(cs));
builder.Services.AddScoped<IAttemptRepository>(_ => new AttemptRepository(cs));

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Oposiciones TAI v1"));
}

app.MapControllers();

app.Run();