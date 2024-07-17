using BackendBlog.DTO.User;
using FluentValidation;

namespace BackendBlog.Validators.User
{
    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            RuleFor(user => user.Id)
                .GreaterThan(0).WithMessage("Id must be a positive integer");

            RuleFor(user => user.Username)
                .NotEmpty().WithMessage("Username is required")
                .Length(1, 50).WithMessage("Username length can't be more than 50 characters")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Username can only contain alphanumeric characters.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address format");

            RuleFor(user => user.RoleId)
                .GreaterThan(0).WithMessage("Role is required and must be greater than 0");
        }
    }
}
