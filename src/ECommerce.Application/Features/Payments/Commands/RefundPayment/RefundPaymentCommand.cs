using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Payment;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Features.Payments.Commands.RefundPayment
{
    public record RefundPaymentCommand(
        Guid OrderId,
        string Reason
    ) : ITransactionalCommand<RefundResponseDto>;
}
