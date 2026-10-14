using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Payments.Commands.UpdatePaymentStatus
{
    public class UpdatePaymentStatusCommandHandler(
    IOrderRepository _orderRepository,
    IUnitOfWork _unitOfWork,
    INotificationService _notificationService,
    ILogger<UpdatePaymentStatusCommandHandler> _logger)
    : IRequestHandler<UpdatePaymentStatusCommand>
    {
        public async Task Handle(
            UpdatePaymentStatusCommand request,
            CancellationToken cancellationToken)
        {
            var order = await _orderRepository
                .GetOrderByPaymentIntentIdAsync(request.PaymentIntentId);

            if (order == null)
            {
                _logger.LogWarning(
                    "Webhook received for unknown PaymentIntent: {PaymentIntentId}",
                    request.PaymentIntentId);
                return;
            }

            var payment = order.Payments
                .FirstOrDefault(p => p.TransactionId == request.PaymentIntentId);

            if (payment == null)
            {
                _logger.LogWarning(
                    "Payment record not found for PaymentIntent: {PaymentIntentId}",
                    request.PaymentIntentId);
                return;
            }

            var targetStatus = request.IsSuccess
                ? PaymentStatus.Success
                : PaymentStatus.Failed;

            if (payment.PaymentStatus == targetStatus)
            {
                _logger.LogInformation(
                    "Webhook event already processed for PaymentIntent: {PaymentIntentId}",
                    request.PaymentIntentId);
                return;
            }

            if (request.IsSuccess)
            {
                payment.PaymentStatus = PaymentStatus.Success;
                order.PaymentStatus = PaymentStatus.Success;
                order.OrderStatus = OrderStatus.Processing;
            }
            else
            {
                payment.PaymentStatus = PaymentStatus.Failed;
                order.PaymentStatus = PaymentStatus.Failed;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Payment status updated for Order {OrderId}: {Status}",
                order.Id, targetStatus);

            if (request.IsSuccess)
            {
                await _notificationService.SendNotificationAsync(
                    userId: order.UserId,
                    title: "Payment Successful",
                    message: $"Payment for order #{order.Id} was successful.",
                    type: NotificationType.PaymentSuccess,
                    relatedEntityId: order.Id,
                    cancellationToken: cancellationToken);
            }
            else
            {
                await _notificationService.SendNotificationAsync(
                    userId: order.UserId,
                    title: "Payment Failed",
                    message: $"Payment for order #{order.Id} failed. Please try again.",
                    type: NotificationType.PaymentFailed,
                    relatedEntityId: order.Id,
                    cancellationToken: cancellationToken);
            }
        }
    }
}
