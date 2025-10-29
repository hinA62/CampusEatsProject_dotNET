using CampusEats.Features.Menu.Requests;
using FluentValidation;

namespace CampusEats.Validators.Menu;

public class CreateMenuValidator : AbstractValidator<CreateMenuRequest>
{
    public CreateMenuValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Menu name is required.")
            .MaximumLength(50).WithMessage("Menu name cannot exceed 50 characters.");
        
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
        
        RuleFor(x => x.ItemIds)
            .NotEmpty().WithMessage("At least one menu item must be selected.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("At least one category must be selected.");
    }
}