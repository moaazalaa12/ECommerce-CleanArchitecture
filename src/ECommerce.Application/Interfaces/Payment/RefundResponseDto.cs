using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Interfaces.Payment
{
    public record RefundResponseDto(
        string? RefundTransactionId = null,
        string? ErrorMessage = null)
    {
        public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

        public static RefundResponseDto Failure(string error) => new(null, error);

        public static RefundResponseDto Success(string refundTransactionId) => new(refundTransactionId, null);
    }
}
