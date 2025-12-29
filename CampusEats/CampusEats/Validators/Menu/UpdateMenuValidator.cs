using CampusEats.Features.Menu.Requests;
using FluentValidation;

namespace CampusEats.Validators.Menu;

public class UpdateMenuValidator : AbstractValidator<UpdateMenuRequest>
{
    public UpdateMenuValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Menu name is required.")
            .MaximumLength(50).WithMessage("Menu name must not exceed 50 characters.");

        RuleFor(x => x.Price).NotEmpty()
            .Must(p => p > 0)
            .WithMessage("Price is required and must be greater than zero.");

        RuleFor(x => x.ItemIds)
            .NotNull().WithMessage("At least one menu item is required.");
    }
}