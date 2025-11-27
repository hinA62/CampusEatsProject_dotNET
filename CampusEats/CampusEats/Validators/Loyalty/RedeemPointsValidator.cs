using CampusEats.Features.Loyalty.Requests;
using FluentValidation;

namespace CampusEats.Validators.Loyalty;

public class RedeemPointsValidator : AbstractValidator<RedeemPointsRequest>
{
    public RedeemPointsValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PointsToRedeem)
            .GreaterThan(0).WithMessage("PointsToRedeem must be greater than 0.");
    }
}