namespace ECommerce.Application.DTOs.Wishlist
{
    public class WishlistItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public bool InStock { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
