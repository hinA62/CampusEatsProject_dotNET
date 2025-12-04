using CampusEats.Features.Menu.Requests;
using FluentValidation;

namespace CampusEats.Validators.Menu;

public class UpdateItemValidator : AbstractValidator<UpdateItemRequest>
{
    public UpdateItemValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must not be empty and should be at least 3 characters long.");
        
        RuleFor(x => x.Price).NotEmpty().WithMessage("Price is required.")
            .GreaterThan(0).WithMessage("Price must be greater than 0.");
        
        RuleFor(x => x.ImageUrl)
            .Cascade(CascadeMode.Stop)
            .Must(uri => Uri.IsWellFormedUriString(uri!, UriKind.Absolute))
                .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
            .WithMessage("Image URL must be a valid URL when provided.");
    }
}