using BackendBlog.DTO.Auth;
using FluentValidation;

namespace BackendBlog.Validators.Auth
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequestDto>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(request => request.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address format");
        }
    }
}
