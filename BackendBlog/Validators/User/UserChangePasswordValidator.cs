using BackendBlog.DTO.User;
using FluentValidation;

namespace BackendBlog.Validators.User
{
    public class UserChangePasswordValidator : AbstractValidator<UserChangePasswordDto>
    {
        public UserChangePasswordValidator()
        {
            RuleFor(user => user.Id)
                .GreaterThan(0).WithMessage("Id must be a positive integer");

            RuleFor(user => user.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required");

            RuleFor(user => user.NewPassword)
                .NotEmpty().WithMessage("New password is required");

            RuleFor(user => user.RepeatPassword)
                .NotEmpty().WithMessage("Repeat Password is required")
                .Equal(user => user.NewPassword).WithMessage("The password and confirmation password do not match.");
        }
    }
}
