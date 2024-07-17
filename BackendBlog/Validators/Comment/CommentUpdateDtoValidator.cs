using BackendBlog.DTO.Comment;
using FluentValidation;

namespace BackendBlog.Validators.Comment
{
    public class CommentUpdateDtoValidator : AbstractValidator<CommentUpdateDto>
    {
        public CommentUpdateDtoValidator()
        {

            RuleFor(comment => comment.Id)
                .GreaterThan(0).WithMessage("Id must be a positive integer");

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
