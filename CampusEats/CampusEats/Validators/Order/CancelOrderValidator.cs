using FluentValidation;
using CampusEats.Features.Order.Requests;


namespace CampusEats.Validators.Order;

public class CancelOrderValidator : AbstractValidator<CancelOrderRequest>
{
    public CancelOrderValidator()
    {
        RuleFor(r => r.OrderId).NotEmpty().WithMessage("Order Id is required.");
    }
}
