using CampusEats.Features.Loyalty.Requests;
using FluentValidation;

namespace CampusEats.Validators.Loyalty;

public class RedeemPointsValidator : AbstractValidator<RedeemPointsRequest>
{
    public RedeemPointsValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(x => x.PointsToRedeem)
            .GreaterThan(0).WithMessage("Points to redeem must be greater than 0.");
    }
}