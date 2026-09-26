using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Interfaces.Payment
{
    public record RefundRequestDto(
        Guid OrderId,
        string ProviderTransactionId,
        decimal Amount,
        string Reason
    );
}
