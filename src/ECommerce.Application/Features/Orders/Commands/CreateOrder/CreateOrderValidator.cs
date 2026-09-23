using ECommerce.Application.Validators;
using FluentValidation;

namespace ECommerce.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(c => c.UserId).NotEmpty().NotEqual(Guid.Empty);

            RuleFor(c => c.DeliveryMethod).IsInEnum();

            RuleFor(c => c.ShippingAddress)
                .NotNull()
                .SetValidator(new AddressDtoValidator());

            RuleFor(c => c.Items).NotEmpty();
            RuleForEach(c => c.Items).SetValidator(new OrderItemRequestDtoValidator());
        }
    }
}
