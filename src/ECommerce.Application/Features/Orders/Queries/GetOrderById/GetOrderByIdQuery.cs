using ECommerce.Application.DTOs.Orders;
using MediatR;

namespace ECommerce.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<OrderDto>
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public GetOrderByIdQuery(Guid orderId, Guid userId)
        {
            OrderId = orderId;
            UserId = userId;
        }
    }
}
