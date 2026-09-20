namespace ECommerce.Application.DTOs
{
    public class PagedRequest<TSortColumn> where TSortColumn : struct, Enum
    {
        private const int MinPageSize = 1;
        private const int MaxPageSize = 50;

        private int _pageSize = 10;
        private int _pageNumber = 1;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = (value < 1) ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = Math.Clamp(value, MinPageSize, MaxPageSize);
        }

        public TSortColumn? SortBy { get; set; }
        public bool IsDescending { get; set; }
    }
}
