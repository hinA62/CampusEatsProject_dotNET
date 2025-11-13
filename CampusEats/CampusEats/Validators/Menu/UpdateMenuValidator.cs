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

        RuleFor(x => x.Price)
            .NotNull().GreaterThan(0).WithMessage("Menu price must be greater than zero.");

        RuleFor(x => x.ItemIds)
            .NotNull().NotEmpty().WithMessage("At least one menu item is required.");
    }
}