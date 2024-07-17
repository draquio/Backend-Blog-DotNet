using BackendBlog.DTO.User;
using FluentValidation;

namespace BackendBlog.Validators.User
{
    public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateDtoValidator()
        {
            RuleFor(user => user.Username)
                .NotEmpty().WithMessage("Username is required")
                .Length(1, 50).WithMessage("Username length can't be more than 50 characters")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Username can only contain alphanumeric characters.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address format");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password is required")
                .Length(3, 30).WithMessage("Password length must be between 3 and 30 characters");

            RuleFor(user => user.RepeatPassword)
                .NotEmpty().WithMessage("Repeat Password is required")
                .Equal(user => user.Password).WithMessage("The password and confirmation password do not match.");

            RuleFor(user => user.RoleId)
                .GreaterThan(0).WithMessage("Role is required and must be greater than 0");
        }
    }
}
