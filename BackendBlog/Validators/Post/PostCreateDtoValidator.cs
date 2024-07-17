using BackendBlog.DTO.Post;
using FluentValidation;

namespace BackendBlog.Validators.Post
{
    public class PostCreateDtoValidator : AbstractValidator<PostCreateDto>
    {
        public PostCreateDtoValidator()
        {
            RuleFor(post => post.Title)
               .NotEmpty().WithMessage("Title is required")
               .Length(3, 150).WithMessage("Title length must be between 3 and 150 characters");
            RuleFor(post => post.Content)
                .MaximumLength(5000).WithMessage("Content length can't be more than 5000 characters")
                .When(post => !string.IsNullOrEmpty(post.Content));
            RuleFor(post => post.AuthorId)
                .GreaterThan(0).WithMessage("AuthorId must be a positive integer");
            RuleFor(post => post.Image)
                .Must(ValidationUtils.IsValidURL).WithMessage("Image must be a valid URL")
                .When(post => !string.IsNullOrEmpty(post.Image));
            RuleFor(post => post.CategoryIds)
                .ForEach(categoryId => categoryId.GreaterThan(0).WithMessage("Category must be positive integers"))
                .When(post => post.CategoryIds != null && post.CategoryIds.Any());
            RuleFor(post => post.TagsIds)
                .ForEach(tagId => tagId.GreaterThan(0).WithMessage("Tag must be positive integers"))
                .When(post => post.TagsIds != null && post.TagsIds.Any());
        }

    }
}
