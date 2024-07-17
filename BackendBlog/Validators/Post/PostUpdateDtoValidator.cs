using BackendBlog.DTO.Post;
using FluentValidation;

namespace BackendBlog.Validators.Post
{
    public class PostUpdateDtoValidator : AbstractValidator<PostUpdateDto>
    {
        public PostUpdateDtoValidator()
        {
            RuleFor(post => post.Id)
                .GreaterThan(0).WithMessage("Id must be a positive integer");
            RuleFor(post => post.Title)
               .NotEmpty().WithMessage("Title is required")
               .Length(3, 150).WithMessage("Title length must be between 3 and 150 characters");
            RuleFor(post => post.Content)
                .MaximumLength(5000).WithMessage("Content length can't be more than 5000 characters")
                .When(post => !string.IsNullOrEmpty(post.Content));
            RuleFor(post => post.UserId)
                .GreaterThan(0).WithMessage("UserId must be a positive integer");
            RuleFor(post => post.ImageId)
                .GreaterThan(0).WithMessage("Id must be a positive integer")
                .When(post => post.ImageId != null && post.ImageId.HasValue);
            RuleFor(post => post.CategoryIds)
                .ForEach(categoryId => categoryId.GreaterThan(0).WithMessage("Category must be positive integers"))
                .When(post => post.CategoryIds != null && post.CategoryIds.Any());
        }
        
    }
}
