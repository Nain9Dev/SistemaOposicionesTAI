using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Oposiciones.Application.Services;

namespace Oposiciones.Application;

/// <summary>Registro de la capa de aplicacion. Cualquier host (Api, CLI o tests) usa este unico punto.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // TimeProvider inyectable: permite congelar el reloj en las pruebas de correccion.
        services.TryAddSingleton(TimeProvider.System);

        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<IQuestionBankService, QuestionBankService>();
        services.AddScoped<ITestGenerationService, TestGenerationService>();
        services.AddScoped<IAttemptService, AttemptService>();

        return services;
    }
}
