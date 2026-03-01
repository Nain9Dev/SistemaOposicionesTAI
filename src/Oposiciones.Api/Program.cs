using Oposiciones.Domain.Interfaces;
using Oposiciones.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var cs = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddScoped<ISyllabusRepository>(_ => new SyllabusRepository(cs));
builder.Services.AddScoped<ITestRepository>(_ => new TestRepository(cs));
builder.Services.AddScoped<IAttemptRepository>(_ => new AttemptRepository(cs));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.MapControllers();

app.Run();