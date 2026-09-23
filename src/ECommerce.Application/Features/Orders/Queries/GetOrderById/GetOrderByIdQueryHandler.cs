using Application.Exceptions;
using AutoMapper;
using ECommerce.Application.DTOs.Orders;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using MediatR;

namespace ECommerce.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler(IOrderRepository _orderRepository, IMapper _mapper) : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderWithDetailsByIdAsync(request.OrderId, cancellationToken);

            if (order == null)
                throw new NotFoundException($"Order with Id {request.OrderId} was not found.");

            return _mapper.Map<OrderDto>(order);
        }
    }
}
