using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Payment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Features.Payments.Commands.CreatePaymentIntent
{
    public record CreatePaymentIntentCommand(
        Guid OrderId,
        Guid UserId,
        string CustomerEmail,
        string CustomerName
    ) : ITransactionalCommand<PaymentResponseDto>;
}
