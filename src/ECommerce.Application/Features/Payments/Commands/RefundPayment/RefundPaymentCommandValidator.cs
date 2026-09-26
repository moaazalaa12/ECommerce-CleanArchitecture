using FluentValidation;

namespace ECommerce.Application.Features.Payments.Commands.RefundPayment
{
    public class RefundPaymentCommandValidator
        : AbstractValidator<RefundPaymentCommand>
    {
        public RefundPaymentCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("Order ID is required.");

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("Refund reason is required.")
                .MaximumLength(500)
                .WithMessage("Refund reason must not exceed 500 characters.");
        }
    }
}
