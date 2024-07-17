using BackendBlog.DTO.Role;
using FluentValidation;

namespace BackendBlog.Validators.Role
{
    public class RoleCreateDtoValidator : AbstractValidator<RoleCreateDto>
    {
        public RoleCreateDtoValidator()
        {
            RuleFor(role => role.Name)
                .NotEmpty().WithMessage("Role name is required")
                .Length(1, 70).WithMessage("Role name length can't be more than 70 characters");
        }
    }
}
