using CampusEats.Features.Auth.Requests;
using CampusEats.Features.User;
using FluentValidation;

namespace CampusEats.Validators.Auth;

public class UpdateUserRoleValidator : AbstractValidator<UpdateUserRoleRequest>
{
    public UpdateUserRoleValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.NewRole)
            .NotEmpty().WithMessage("Role is required.")
            .Must(role => role.Equals("Client", StringComparison.OrdinalIgnoreCase) || 
                         role.Equals("Kitchen", StringComparison.OrdinalIgnoreCase) || 
                         role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Invalid role. Valid roles: Client, Kitchen, Admin.");
    }
}
