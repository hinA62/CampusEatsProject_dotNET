using CampusEats.Features.Loyalty.Requests;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Features.Loyalty.Handlers;

public class RedeemPointsHandler
{
    private readonly CampusEatsContext _db;

    public RedeemPointsHandler(CampusEatsContext db)
    {
        _db = db;
    }

    public async Task<IResult> Handle(RedeemPointsRequest request, CancellationToken ct = default)
    {
        if (request.PointsToRedeem <= 0)
            return Results.BadRequest("PointsToRedeem must be greater than 0.");

        var account = await _db.LoyaltyAccounts
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

        await _db.LoyaltyTransactions.AddAsync(tx, ct);
        await _db.SaveChangesAsync(ct);

        return Results.Ok(new {
            account.UserId,
            account.Points
        });
    }
}