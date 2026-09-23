using Application.Exceptions;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using MediatR;

namespace ECommerce.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler(IOrderRepository _orderRepository) : IRequestHandler<UpdateOrderStatusCommand, bool>
    {
        public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);

            if (order == null)
                throw new NotFoundException($"Order with Id {request.OrderId} was not found.");

            order.OrderStatus = request.NewStatus;

            _orderRepository.Update(order);

            return true;
        }
    }
}
