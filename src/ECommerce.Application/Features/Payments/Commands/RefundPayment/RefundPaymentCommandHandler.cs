using Application.Exceptions;
using ECommerce.Application.Interfaces.Payment;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Interfaces.OrderInterfaces;
using MediatR;

namespace ECommerce.Application.Features.Payments.Commands.RefundPayment
{
    public class RefundPaymentCommandHandler(IOrderRepository _orderRepository, IPaymentService _paymentService)
       : IRequestHandler<RefundPaymentCommand, RefundResponseDto>
    {
        public async Task<RefundResponseDto> Handle(
            RefundPaymentCommand request,
            CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderWithDetailsByIdAsync(request.OrderId);

            if (order == null)
            {
                throw new NotFoundException($"Order with ID {request.OrderId} was not found.");
            }

            if (order.PaymentStatus == PaymentStatus.Refunded)
            {
                throw new BadRequestException("This order has already been refunded.");
            }

            var successfulPayment = order.Payments
                .FirstOrDefault(p => p.PaymentStatus == PaymentStatus.Success);

            if (successfulPayment == null)
            {
                throw new BadRequestException(
                    "This order does not have a successful payment to refund.");
            }

            var refundRequest = new RefundRequestDto(
                order.Id,
                successfulPayment.TransactionId,
                successfulPayment.Amount,
                request.Reason
            );

            var refundResult = await _paymentService
                .RefundAsync(refundRequest, cancellationToken);

            if (!refundResult.IsSuccess)
            {
                throw new BadRequestException($"Refund failed: {refundResult.ErrorMessage}");
            }

            successfulPayment.PaymentStatus = PaymentStatus.Refunded;
            order.PaymentStatus = PaymentStatus.Refunded;

            return refundResult;
        }
    }
}
