namespace ECommerce.Application.DTOs.Orders
{
    public class OrderItemRequestDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
