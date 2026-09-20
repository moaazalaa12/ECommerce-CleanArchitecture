namespace ECommerce.Application.DTOs
{
    public record PagedResponse<T>(
        IReadOnlyList<T> Data,
        int PageNumber,
        int PageSize,
        int TotalRecords
    )
    {
        public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;
    }
}
