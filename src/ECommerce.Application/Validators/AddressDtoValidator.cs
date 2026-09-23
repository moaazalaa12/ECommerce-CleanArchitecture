using ECommerce.Application.DTOs.Orders;
using FluentValidation;

namespace ECommerce.Application.Validators
{
    public class AddressDtoValidator : AbstractValidator<AddressDto>
    {
        public AddressDtoValidator()
        {
            RuleFor(a => a.Street).NotEmpty().MaximumLength(200);
            RuleFor(a => a.City).NotEmpty().MaximumLength(100);
            RuleFor(a => a.State).NotEmpty().MaximumLength(100);
            RuleFor(a => a.Country).NotEmpty().MaximumLength(100);
            RuleFor(a => a.ZipCode).NotEmpty().MaximumLength(20);
        }
    }
}
