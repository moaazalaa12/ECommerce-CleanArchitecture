using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Interfaces.Payment
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreateOrUpdatePaymentIntentAsync(
            PaymentRequestDto request,
            CancellationToken cancellationToken = default);

        Task<RefundResponseDto> RefundAsync(
            RefundRequestDto request,
            CancellationToken cancellationToken = default);
    }
}
