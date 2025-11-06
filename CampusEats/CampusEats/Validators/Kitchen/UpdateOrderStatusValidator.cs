using CampusEats.Features.Kitchen.Requests;
using CampusEats.Features.Order;
using FluentValidation;

namespace CampusEats.Validators.Kitchen;

public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("OrderId is required");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid order status");

        RuleFor(x => x.NewStatus)
            .Must(status => status != OrderStatus.Cancelled)
            .WithMessage("Cannot change status to Cancelled. Use cancel endpoint instead");
    }
}
