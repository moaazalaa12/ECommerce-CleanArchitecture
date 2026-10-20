using ECommerce.Application.DTOs.Reviews;
using FluentValidation;

namespace ECommerce.Application.Validators
{
    public class CreateReviewValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Rating).InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
            RuleFor(x => x.Comment).MaximumLength(1000);
        }
    }
}
