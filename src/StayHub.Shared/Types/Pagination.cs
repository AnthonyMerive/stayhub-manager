using System.Diagnostics.CodeAnalysis;

namespace StayHub.Shared.Types;

/// <summary>
/// Parámetros de paginación con filtro genérico
/// </summary>
[ExcludeFromCodeCoverage]
public class PaginationParams<TFilter> where TFilter : class
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public TFilter? Filter { get; set; }
}

/// <summary>
/// Resultado de paginación genérico
/// </summary>
[ExcludeFromCodeCoverage]
public class Pagination<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public List<T> Items { get; set; } = [];

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public static Pagination<T> Create(List<T> items, int totalRecords, int pageNumber, int pageSize)
    {
        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        return new Pagination<T>
        {
            Items = items,
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = Math.Max(totalPages, 1)
        };
    }
}
