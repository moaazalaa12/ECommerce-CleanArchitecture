using ECommerce.Application.DTOs.Orders;
using MediatR;

namespace ECommerce.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<OrderDto>
    {
        public Guid OrderId { get; set; }
        public GetOrderByIdQuery(Guid orderId) => OrderId = orderId;
    }
}
