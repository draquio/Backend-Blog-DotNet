using BackendBlog.DTO.Comment;
using FluentValidation;

namespace BackendBlog.Validators.Comment
{
    public class CommentCreateDtoValidator : AbstractValidator<CommentCreateDto>
    {
        public CommentCreateDtoValidator()
        {
            RuleFor(comment => comment.AuthorComment)
                 .NotEmpty().WithMessage("Name is required")
                 .Length(1, 70).WithMessage("Name length can't be more than 70 characters");

            RuleFor(comment => comment.PostId)
                .GreaterThan(0).WithMessage("Post Id is required");

            RuleFor(comment => comment.Content)
                .NotEmpty().WithMessage("Content is required")
                .Length(1, 500).WithMessage("Content length can't be more than 500 characters");
        }
    }
}
