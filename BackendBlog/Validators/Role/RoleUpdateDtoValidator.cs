using BackendBlog.DTO.Role;
using FluentValidation;

namespace BackendBlog.Validators.Role
{
    public class RoleUpdateDtoValidator : AbstractValidator<RoleUpdateDto>
    {
        public RoleUpdateDtoValidator()
        {
            RuleFor(role => role.Id)
            .GreaterThan(0).WithMessage("Id must be a positive integer");

            RuleFor(role => role.Name)
                .NotEmpty().WithMessage("Role name is required")
                .Length(1, 70).WithMessage("Role name length can't be more than 70 characters");
        }
    }
}
