using Microsoft.Extensions.DependencyInjection;
using Oposiciones.Domain.Interfaces;
using Oposiciones.Infrastructure.Repositories;
using Oposiciones.Application.Interfaces.Security;
using Oposiciones.Infrastructure.Security;

namespace Oposiciones.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<ISyllabusRepository>(_ => new SyllabusRepository(connectionString));
        services.AddScoped<ITestRepository>(_ => new TestRepository(connectionString));
        services.AddScoped<IAttemptRepository>(_ => new AttemptRepository(connectionString));
        services.AddScoped<IUsuarioRepository>(_ => new UsuarioRepository(connectionString));
        services.AddScoped<IProgresoRepository>(_ => new ProgresoRepository(connectionString));
        services.AddScoped<IRefreshTokenRepository>(_ => new RefreshTokenRepository(connectionString));
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        
        return services;
    }
}
