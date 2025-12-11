using System.Text.RegularExpressions;
using CampusEats.Features.Auth.Requests;
using FluentValidation;

namespace CampusEats.Validators.Auth;

public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("Username cannot exceed 50 characters long.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .Must(email => Regex.IsMatch
            (email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            .WithMessage("Invalid email format.")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(10).WithMessage("Password must be at least 10 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[!@#$%^&*(),.?""':{}|<>]")
            .WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm Password is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}