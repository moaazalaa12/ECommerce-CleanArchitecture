using Application.Exceptions;
using ECommerce.Application.Interfaces.Payment;
using ECommerce.Domain.Entities.OrderEntities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using MediatR;

namespace ECommerce.Application.Features.Payments.Commands.CreatePaymentIntent
{
    public class CreatePaymentIntentCommandHandler(IOrderRepository _orderRepository, IPaymentService _paymentService)
       : IRequestHandler<CreatePaymentIntentCommand, PaymentResponseDto>
    {
        public async Task<PaymentResponseDto> Handle(
            CreatePaymentIntentCommand request,
            CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderWithDetailsByIdAsync(request.OrderId);

            if (order == null)
            {
                throw new NotFoundException($"Order with ID {request.OrderId} was not found.");
            }

            if (order.UserId != request.UserId)
            {
                throw new ForbiddenException("You are not authorized to access this order.");
            }

            if (order.PaymentStatus == PaymentStatus.Success)
            {
                throw new BadRequestException("This order has already been paid.");
            }

            if (order.OrderStatus == OrderStatus.Cancelled)
            {
                throw new BadRequestException("Cannot create payment for a cancelled order.");
            }

            if (order.PaymentStatus == PaymentStatus.Refunded)
            {
                throw new BadRequestException("This order has already been refunded.");
            }

            var existingPayment = order.Payments
                .FirstOrDefault(p => p.PaymentStatus == PaymentStatus.Pending);

            var paymentRequest = new PaymentRequestDto(
                order.Id,
                order.TotalAmount,
                request.CustomerEmail,
                request.CustomerName,
                existingPayment?.TransactionId
            );

            var paymentResult = await _paymentService
                .CreateOrUpdatePaymentIntentAsync(paymentRequest, cancellationToken);

            if (!paymentResult.IsSuccess)
            {
                throw new BadRequestException(
                    $"Failed to initialize payment: {paymentResult.ErrorMessage}");
            }

            if (existingPayment == null)
            {
                var payment = new Payment
                {
                    OrderId = order.Id,
                    TransactionId = paymentResult.PaymentIntentId,
                    Amount = order.TotalAmount,
                    PaymentMethod = "Stripe",
                    PaymentStatus = PaymentStatus.Pending
                };
                order.Payments.Add(payment);
            }
            else
            {
                existingPayment.TransactionId = paymentResult.PaymentIntentId;
                existingPayment.Amount = order.TotalAmount;
            }

            return paymentResult;
        }
    }
}
