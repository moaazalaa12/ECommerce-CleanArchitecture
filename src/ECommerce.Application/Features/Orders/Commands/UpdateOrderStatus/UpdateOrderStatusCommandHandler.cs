using Application.Exceptions;
using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using MediatR;

namespace ECommerce.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler(
    IOrderRepository _orderRepository,
    IUnitOfWork _unitOfWork,
    INotificationService _notificationService) : IRequestHandler<UpdateOrderStatusCommand, bool>
    {
        public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);

            if (order == null)
                throw new NotFoundException($"Order with Id {request.OrderId} was not found.");

            order.OrderStatus = request.NewStatus;
            _orderRepository.Update(order);


            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var (title, message, type) = request.NewStatus switch
            {
                OrderStatus.Shipped => ("Order Shipped", $"Your order #{order.Id} has been shipped.", NotificationType.OrderShipped),
                OrderStatus.Delivered => ("Order Delivered", $"Your order #{order.Id} has been delivered. Enjoy!", NotificationType.OrderDelivered),
                OrderStatus.Cancelled => ("Order Cancelled", $"Your order #{order.Id} has been cancelled.", NotificationType.OrderCancelled),
                OrderStatus.Processing => ("Order Processing", $"Your order #{order.Id} is being processed.", NotificationType.OrderProcessing),
                _ => (string.Empty, string.Empty, NotificationType.OrderPlaced)
            };

            if (!string.IsNullOrEmpty(title))
            {
                await _notificationService.SendNotificationAsync(
                    userId: order.UserId,
                    title: title,
                    message: message,
                    type: type,
                    relatedEntityId: order.Id,
                    cancellationToken: cancellationToken);
            }

            return true;
        }
    }
}
