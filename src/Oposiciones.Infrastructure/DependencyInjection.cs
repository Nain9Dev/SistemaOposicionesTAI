using Microsoft.Extensions.DependencyInjection;
using Oposiciones.Application.Interfaces.Security;
using Oposiciones.Domain.Interfaces;
using Oposiciones.Infrastructure.Data;
using Oposiciones.Infrastructure.Repositories;
using Oposiciones.Infrastructure.Security;

namespace Oposiciones.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
    {
        // Singleton: NpgsqlDataSource mantiene el pool de conexiones y la cache de sentencias.
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));

        services.AddScoped<ISyllabusRepository, SyllabusRepository>();
        services.AddScoped<ITestRepository, TestRepository>();
        services.AddScoped<IAttemptRepository, AttemptRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IProgresoRepository, ProgresoRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IStudyQuestionRepository, StudyQuestionRepository>();

        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

        return services;
    }
}
