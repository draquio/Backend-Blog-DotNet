using BackendBlog.DTO.Auth;
using FluentValidation;


namespace BackendBlog.Validators.Auth
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(register => register.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address format");

            RuleFor(register => register.Password)
                .NotEmpty().WithMessage("Password is required");

            RuleFor(register => register.RepeatPassword)
                .NotEmpty().WithMessage("Repeat Password is required")
                .Equal(register => register.Password).WithMessage("The password and confirmation password do not match.");

            RuleFor(register => register.Username)
                .NotEmpty().WithMessage("Username i required");
        }
    }
}
