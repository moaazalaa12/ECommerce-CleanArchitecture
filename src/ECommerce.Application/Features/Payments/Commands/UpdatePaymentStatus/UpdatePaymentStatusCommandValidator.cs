using FluentValidation;

namespace ECommerce.Application.Features.Payments.Commands.UpdatePaymentStatus
{
    public class UpdatePaymentStatusCommandValidator
        : AbstractValidator<UpdatePaymentStatusCommand>
    {
        public UpdatePaymentStatusCommandValidator()
        {
            RuleFor(x => x.PaymentIntentId)
                .NotEmpty()
                .WithMessage("PaymentIntent ID is required.")
                .Must(id => id.StartsWith("pi_"))
                .WithMessage("PaymentIntent ID must be a valid Stripe ID (starts with 'pi_').");
        }
    }
}
