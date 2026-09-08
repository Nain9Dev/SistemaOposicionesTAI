using Microsoft.Extensions.DependencyInjection;
using Oposiciones.Application.Interfaces;
using Oposiciones.Application.Services;

namespace Oposiciones.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registra los servicios de aplicacion en un unico punto, en lugar de ir anadiendolos
    /// sueltos en Program.cs conforme aparecian.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Sin estado y sin dependencias: una sola instancia basta.
        services.AddSingleton<IScoringService, ScoringService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProgresoService, ProgresoService>();
        services.AddScoped<IStudyQuestionService, StudyQuestionService>();
        services.AddScoped<IAttemptService, AttemptService>();

        return services;
    }
}
