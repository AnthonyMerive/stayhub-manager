using System.Diagnostics.CodeAnalysis;

namespace StayHub.Infrastructure.In.Rest.Dtos;

/// <summary>
/// DTO para resultados paginados
/// </summary>
[ExcludeFromCodeCoverage]
public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
}