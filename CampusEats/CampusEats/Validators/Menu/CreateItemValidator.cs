using CampusEats.Features.Menu.Requests;
using FluentValidation;

namespace CampusEats.Validators.Menu;

public class CreateItemValidator : AbstractValidator<CreateItemRequest>
{
    public CreateItemValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty().MinimumLength(3)
            .WithMessage("Name must not be empty and should be at least 3 characters long.");
        
        RuleFor(x => x.Price).NotEmpty().GreaterThan(0).WithMessage("Price must be greater than 0.");
        
        RuleFor(x => x.ImageUrl).Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .WithMessage("A valid Image URL is required.");
    }
}