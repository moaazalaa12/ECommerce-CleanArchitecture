using FluentValidation;

namespace ECommerce.Application.Features.Payments.Commands.CreatePaymentIntent
{
    public class CreatePaymentIntentCommandValidator
       : AbstractValidator<CreatePaymentIntentCommand>
    {
        public CreatePaymentIntentCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("Order ID is required.");

            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");

            RuleFor(x => x.CustomerEmail)
                .NotEmpty()
                .WithMessage("Customer email is required.")
                .EmailAddress()
                .WithMessage("Customer email must be a valid email address.");

            RuleFor(x => x.CustomerName)
                .NotEmpty()
                .WithMessage("Customer name is required.")
                .MaximumLength(200)
                .WithMessage("Customer name must not exceed 200 characters.");
        }
    }
}
