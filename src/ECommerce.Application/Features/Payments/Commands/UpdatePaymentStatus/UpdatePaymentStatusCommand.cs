using ECommerce.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Features.Payments.Commands.UpdatePaymentStatus
{
    public record UpdatePaymentStatusCommand(
        string PaymentIntentId,
        bool IsSuccess
    ) : ITransactionalCommand;
}
