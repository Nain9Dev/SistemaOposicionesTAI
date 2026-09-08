namespace Oposiciones.Application.DTOs;

/// <summary>Envoltorio de paginacion comun a todos los listados de la API.</summary>
public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }

    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

/// <summary>Limites de paginacion aplicados en servidor, no negociables por el cliente.</summary>
public static class PagingDefaults
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    /// <summary>
    /// Satura pagina y tamano a un rango seguro. Sin esto, page=0 generaba un OFFSET negativo
    /// (error de SQL) y pageSize arbitrario permitia volcar la tabla entera.
    /// </summary>
    public static (int Page, int PageSize) Sanitize(int page, int pageSize)
    {
        var safePage = page < DefaultPage ? DefaultPage : page;
        var safePageSize = pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);
        return (safePage, safePageSize);
    }
}
