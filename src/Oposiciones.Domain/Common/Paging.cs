namespace Oposiciones.Domain.Common;

/// <summary>
/// Pagina y tamano de pagina ya normalizados. Se construye siempre por <see cref="Of"/>
/// para que ninguna capa superior pueda pedir una pagina negativa o un tamano ilimitado.
/// </summary>
public readonly record struct Paging
{
    public const int DefaultPageSize = 25;
    public const int MaxPageSize = 200;

    private Paging(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }

    public int Page { get; }

    public int PageSize { get; }

    /// <summary>Numero de filas a saltar en la consulta subyacente.</summary>
    public int Offset => (Page - 1) * PageSize;

    public static Paging Default => new(1, DefaultPageSize);

    public static Paging Of(int? page, int? pageSize)
    {
        int normalizedPage = page is null or < 1 ? 1 : page.Value;
        int normalizedSize = pageSize switch
        {
            null or < 1 => DefaultPageSize,
            > MaxPageSize => MaxPageSize,
            _ => pageSize.Value,
        };

        return new Paging(normalizedPage, normalizedSize);
    }
}
