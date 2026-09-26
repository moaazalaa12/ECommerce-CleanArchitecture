using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DTOs.Payments
{
    public record PaymentIntentResponseDto(string ClientSecret);
}
