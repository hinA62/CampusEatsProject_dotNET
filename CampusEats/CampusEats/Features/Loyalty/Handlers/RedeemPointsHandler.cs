using CampusEats.Features.Loyalty.Requests;
using CampusEats.Persistence;
using CampusEats.Validators.Loyalty;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Features.Loyalty.Handlers;

public class RedeemPointsHandler(CampusEatsContext db, ILogger<RedeemPointsHandler> logger)
{
    public async Task<IResult> Handle(RedeemPointsRequest request, CancellationToken ct)
    {
        var validator = new RedeemPointsValidator();
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                logger.LogError(error.ErrorMessage);
            }

            return Results.BadRequest(validationResult.Errors);
        }
        
        
        var account = await db.LoyaltyAccounts
            .FirstOrDefaultAsync(a => a.UserId == request.UserId, ct);

        if (account is null)
        {
            return Results.BadRequest("Loyalty account not found.");
        }

        if (account.Points < request.PointsToRedeem)
        {
            return Results.BadRequest("Not enough points.");
        }

        account.Points -= request.PointsToRedeem;
        account.UpdatedAtUtc = DateTime.UtcNow;

        var tx = new LoyaltyTransaction
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Type = LoyaltyTransactionType.Redeem,
            Points = request.PointsToRedeem,
            Description = "Redeemed points",
            CreatedAtUtc = DateTime.UtcNow
        };

        await db.LoyaltyTransactions.AddAsync(tx, ct);
        await db.SaveChangesAsync(ct);
        logger.LogInformation($"Points redeemed for user {request.UserId}: {request.PointsToRedeem}");

        return Results.Ok(new {
            account.UserId,
            account.Points
        });
    }
}
