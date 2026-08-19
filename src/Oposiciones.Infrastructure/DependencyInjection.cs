using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Oposiciones.Domain.Abstractions;
using Oposiciones.Infrastructure.Caching;
using Oposiciones.Infrastructure.Configuration;
using Oposiciones.Infrastructure.Content;
using Oposiciones.Infrastructure.InMemory;
using Oposiciones.Infrastructure.SqlServer;

namespace Oposiciones.Infrastructure;

/// <summary>
/// Registro de la infraestructura. Es el unico punto del sistema donde se decide si se habla con
/// SQL Server o con el contenido en memoria; a partir de aqui todo el codigo ve las mismas
/// interfaces del dominio.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string contentBasePath)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<PersistenceOptions>()
            .Bind(configuration.GetSection(PersistenceOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<ContentOptions>()
            .Bind(configuration.GetSection(ContentOptions.SectionName))
            .ValidateOnStart();

        services.AddMemoryCache();
        services.TryAddSingleton(TimeProvider.System);

        PersistenceOptions persistence = configuration
            .GetSection(PersistenceOptions.SectionName)
            .Get<PersistenceOptions>() ?? new PersistenceOptions();

        if (persistence.Provider == PersistenceProvider.SqlServer)
        {
            AddSqlServer(services, configuration, persistence);
        }
        else
        {
            AddInMemory(services, contentBasePath);
        }

        if (persistence.EnableCatalogCache)
        {
            services.Decorate<IExamCatalogRepository>((inner, provider) =>
                new CachedExamCatalogRepository(
                    inner,
                    provider.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>(),
                    provider.GetRequiredService<IOptions<PersistenceOptions>>()));
        }

        return services;
    }

    private static void AddSqlServer(
        IServiceCollection services,
        IConfiguration configuration,
        PersistenceOptions persistence)
    {
        string? connectionString = persistence.ConnectionString
            ?? configuration.GetConnectionString(persistence.ConnectionStringName);

        services.AddSingleton<ISqlConnectionFactory>(provider =>
            new SqlConnectionFactory(
                provider.GetRequiredService<IOptions<PersistenceOptions>>(),
                connectionString ?? string.Empty));

        services.AddSingleton<SqlResilience>();

        services.AddScoped<IExamCatalogRepository, SqlServerExamCatalogRepository>();
        services.AddScoped<IQuestionRepository, SqlServerQuestionRepository>();
        services.AddScoped<ITestRepository, SqlServerTestRepository>();
        services.AddScoped<IAttemptRepository, SqlServerAttemptRepository>();
    }

    private static void AddInMemory(IServiceCollection services, string contentBasePath)
    {
        services.AddSingleton<ContentLoader>();

        services.AddSingleton(provider =>
        {
            ContentLoader loader = provider.GetRequiredService<ContentLoader>();
            ContentOptions options = provider.GetRequiredService<IOptions<ContentOptions>>().Value;
            return loader.Load(options, contentBasePath);
        });

        services.AddSingleton<InMemoryStore>();

        services.AddScoped<IExamCatalogRepository, InMemoryExamCatalogRepository>();
        services.AddScoped<IQuestionRepository, InMemoryQuestionRepository>();
        services.AddScoped<ITestRepository, InMemoryTestRepository>();
        services.AddScoped<IAttemptRepository, InMemoryAttemptRepository>();

        // Carga el contenido durante el arranque en lugar de en la primera peticion: si un fichero
        // del banco esta mal, es mejor que el despliegue falle a que falle el opositor.
        services.AddHostedService<ContentWarmupService>();
    }

    /// <summary>
    /// Sustituye un servicio ya registrado por una version decorada. Es el minimo imprescindible
    /// para poder anadir cache sin arrastrar un contenedor de inyeccion adicional.
    /// </summary>
    private static void Decorate<TService>(
        this IServiceCollection services,
        Func<TService, IServiceProvider, TService> decorator)
        where TService : class
    {
        ServiceDescriptor? existing = services.LastOrDefault(descriptor =>
            descriptor.ServiceType == typeof(TService));

        if (existing is null)
        {
            return;
        }

        services.Remove(existing);

        services.Add(new ServiceDescriptor(
            typeof(TService),
            provider => decorator(CreateInstance<TService>(provider, existing), provider),
            existing.Lifetime));
    }

    private static TService CreateInstance<TService>(IServiceProvider provider, ServiceDescriptor descriptor)
        where TService : class
    {
        if (descriptor.ImplementationInstance is TService instance)
        {
            return instance;
        }

        if (descriptor.ImplementationFactory is not null)
        {
            return (TService)descriptor.ImplementationFactory(provider);
        }

        return (TService)ActivatorUtilities.CreateInstance(
            provider,
            descriptor.ImplementationType
                ?? throw new InvalidOperationException(
                    $"No se puede decorar {typeof(TService).Name}: el registro original no tiene tipo de implementacion."));
    }
}

/// <summary>Fuerza la carga y validacion del contenido al arrancar el proceso.</summary>
internal sealed class ContentWarmupService : Microsoft.Extensions.Hosting.IHostedService
{
    private readonly InMemoryStore _store;
    private readonly ILogger<ContentWarmupService> _logger;

    public ContentWarmupService(InMemoryStore store, ILogger<ContentWarmupService> logger)
    {
        _store = store;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Proveedor en memoria listo con {Exams} convocatorias y {Questions} preguntas.",
            _store.Catalog.Exams.Count,
            _store.Catalog.Questions.Count);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
