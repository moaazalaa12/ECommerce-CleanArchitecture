using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Interfaces.Payment
{
    public record PaymentResponseDto(
        string PaymentIntentId,
        string ClientSecret,
        string? ErrorMessage = null)
    {
        public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

        public static PaymentResponseDto Failure(string error) =>
            new(string.Empty, string.Empty, error);

        public static PaymentResponseDto Success(string intentId, string clientSecret) =>
            new(intentId, clientSecret, null);
    }
}
