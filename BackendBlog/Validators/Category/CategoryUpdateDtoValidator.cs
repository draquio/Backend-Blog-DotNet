using BackendBlog.DTO.Category;
using FluentValidation;

namespace BackendBlog.Validators.Category
{
    public class CategoryUpdateDtoValidator : AbstractValidator<CategoryUpdateDto>
    {
        public CategoryUpdateDtoValidator()
        {
            RuleFor(category => category.Id)
            .GreaterThan(0).WithMessage("Id must be a positive integer");

            RuleFor(category => category.Name)
            .NotEmpty().WithMessage("Category name is required")
            .Length(1, 50).WithMessage("Category name length can't be more than 50 characters");

            RuleFor(category => category.Description)
                .MaximumLength(200).WithMessage("Description length can't be more than 200 characters")
                .When(category => !string.IsNullOrEmpty(category.Description));
        }
    }
}
