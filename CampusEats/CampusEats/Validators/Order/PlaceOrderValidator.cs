using FluentValidation;
using CampusEats.Features.Order.Requests;

namespace CampusEats.Validators.Order;

public class PlaceOrderValidator : AbstractValidator<PlaceOrderRequest>
{
    public PlaceOrderValidator()
    {
        RuleFor(r => r.ClientId).NotEmpty();
        RuleFor(r => r.MenuIDs).NotNull();
        RuleFor(r => r.ItemIDs).NotNull();
        RuleFor(r => r).Must(r => (r.MenuIDs?.Count ?? 0) + (r.ItemIDs?.Count ?? 0) > 0)
            .WithMessage("At least one Menu or Item must be provided.");
    }
}