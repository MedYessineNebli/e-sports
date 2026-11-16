namespace API.DTOs;

public sealed record PagedRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public int ValidatedPage => Math.Max(Page, 1);
    public int ValidatedPageSize => Math.Clamp(PageSize, 1, 100);
}

public sealed record PagedResult<T>(
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    IReadOnlyList<T> Items);
