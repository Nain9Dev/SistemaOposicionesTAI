using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Oposiciones.Domain.Abstractions;
using Oposiciones.Domain.Catalog;
using Oposiciones.Infrastructure.Configuration;

namespace Oposiciones.Infrastructure.Caching;

/// <summary>
/// Decorador que cachea el temario en memoria.
/// <para>
/// El temario cambia con cada convocatoria, es decir, casi nunca, pero se lee en cada generacion
/// de test y en cada pantalla de seleccion. Cachearlo elimina la mayoria de los viajes a la base
/// de datos sin tocar ni el dominio ni los repositorios: es un decorador, no una modificacion.
/// </para>
/// </summary>
public sealed class CachedExamCatalogRepository : IExamCatalogRepository
{
    private const string CacheKeyPrefix = "catalog:";

    private readonly IExamCatalogRepository _inner;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _lifetime;

    public CachedExamCatalogRepository(
        IExamCatalogRepository inner,
        IMemoryCache cache,
        IOptions<PersistenceOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _inner = inner;
        _cache = cache;
        _lifetime = TimeSpan.FromSeconds(options.Value.CatalogCacheSeconds);
    }

    public Task<IReadOnlyList<ExamProfile>> GetExamsAsync(CancellationToken cancellationToken = default) =>
        GetOrCreateAsync($"{CacheKeyPrefix}exams", () => _inner.GetExamsAsync(cancellationToken));

    public Task<ExamProfile?> GetExamAsync(string examCode, CancellationToken cancellationToken = default) =>
        GetOrCreateAsync(
            $"{CacheKeyPrefix}exam:{examCode.ToUpperInvariant()}",
            () => _inner.GetExamAsync(examCode, cancellationToken));

    public Task<IReadOnlyList<SyllabusBlock>> GetBlocksAsync(
        string examCode,
        CancellationToken cancellationToken = default) =>
        GetOrCreateAsync(
            $"{CacheKeyPrefix}blocks:{examCode.ToUpperInvariant()}",
            () => _inner.GetBlocksAsync(examCode, cancellationToken));

    public Task<IReadOnlyList<SyllabusTopic>> GetTopicsAsync(
        string examCode,
        string? blockCode = null,
        CancellationToken cancellationToken = default) =>
        GetOrCreateAsync(
            $"{CacheKeyPrefix}topics:{examCode.ToUpperInvariant()}:{blockCode?.ToUpperInvariant() ?? "*"}",
            () => _inner.GetTopicsAsync(examCode, blockCode, cancellationToken));

    private async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory)
    {
        if (_cache.TryGetValue(key, out T? cached) && cached is not null)
        {
            return cached;
        }

        T value = await factory().ConfigureAwait(false);

        // Un catalogo inexistente tambien se cachea, aunque brevemente: evita que una peticion
        // repetida con un codigo erroneo golpee la base de datos una y otra vez.
        _cache.Set(key, value, value is null ? TimeSpan.FromSeconds(15) : _lifetime);
        return value;
    }
}
