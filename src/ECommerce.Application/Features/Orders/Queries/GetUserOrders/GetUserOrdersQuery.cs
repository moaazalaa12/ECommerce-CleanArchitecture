using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Orders;
using ECommerce.Domain.Enums;
using MediatR;

namespace ECommerce.Application.Features.Orders.Queries.GetUserOrders
{
    public class GetUserOrdersQuery : PagedRequest<OrderSortColumn>, IRequest<PagedResponse<OrderDto>>
    {
        public Guid UserId { get; set; }
        public GetUserOrdersQuery(Guid userId) => UserId = userId;
    }
}
