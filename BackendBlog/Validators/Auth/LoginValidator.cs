using BackendBlog.DTO.Auth;
using FluentValidation;

namespace BackendBlog.Validators.Auth
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(login => login.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address format");
            RuleFor(login => login.Password)
                 .NotEmpty().WithMessage("Password is required");
        }
    }
}
