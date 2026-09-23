using ECommerce.Application.DTOs.Orders;
using FluentValidation;

namespace ECommerce.Application.Features.Orders.Commands.CreateOrder
{
    public class OrderItemRequestDtoValidator : AbstractValidator<OrderItemRequestDto>
    {
        public OrderItemRequestDtoValidator()
        {
            RuleFor(i => i.ProductId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(i => i.Quantity).GreaterThan(0);
        }
    }
}
