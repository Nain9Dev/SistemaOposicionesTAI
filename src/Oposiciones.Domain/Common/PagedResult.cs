namespace Oposiciones.Domain.Common;

/// <summary>
/// Resultado paginado generico. Todas las consultas de listado del sistema lo devuelven,
/// de forma que anadir un catalogo nuevo no obliga a inventar otro contrato de paginacion.
/// </summary>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, long TotalItems)
{
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    public static PagedResult<T> Empty(Paging paging) =>
        new(Array.Empty<T>(), paging.Page, paging.PageSize, 0);

    public static PagedResult<T> From(IReadOnlyList<T> items, Paging paging, long totalItems) =>
        new(items, paging.Page, paging.PageSize, totalItems);

    public PagedResult<TOut> Map<TOut>(Func<T, TOut> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        var mapped = new List<TOut>(Items.Count);
        foreach (T item in Items)
        {
            mapped.Add(selector(item));
        }

        return new PagedResult<TOut>(mapped, Page, PageSize, TotalItems);
    }
}
