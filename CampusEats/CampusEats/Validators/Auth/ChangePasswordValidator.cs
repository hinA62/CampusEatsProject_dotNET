using CampusEats.Features.Auth.Requests;
using FluentValidation;

namespace CampusEats.Validators.Auth;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(10).WithMessage("Password must be at least 10 characters long")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit")
            .Matches(@"[@$!%*?&#]").WithMessage("Password must contain at least one special character (@$!%*?&#)");

        RuleFor(x => x)
            .Must(x => x.NewPassword != x.CurrentPassword)
            .WithMessage("New password must be different from current password")
            .When(x => !string.IsNullOrEmpty(x.CurrentPassword) && !string.IsNullOrEmpty(x.NewPassword));
    }
}

