namespace ECommerce.Application.Interfaces.Payment
{
    public record PaymentRequestDto(
        Guid OrderId,
        decimal TotalAmount,
        string CustomerEmail,
        string CustomerName,
        string? PaymentIntentId = null
    );
}
