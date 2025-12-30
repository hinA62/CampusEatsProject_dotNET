using CampusEats.Features.Loyalty.Requests;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Features.Loyalty.Handlers;

public class GetLoyaltyBalanceHandler(CampusEatsContext db)
{
    public async Task<IResult> Handle(GetLoyaltyBalanceRequest request, CancellationToken ct = default)
    {
        var account = await db.LoyaltyAccounts.FirstOrDefaultAsync(a => a.UserId == request.UserId, ct);
        
        if (account is null)
        {
            return Results.Ok(new 
            { 
                userId = request.UserId, 
                points = 0,
                totalPointsEarned = 0,
                currentTier = LoyaltyTier.Bronze,
                cashbackRate = LoyaltyTierHelper.GetCashbackRate(LoyaltyTier.Bronze),
                nextTier = LoyaltyTier.Silver,
                pointsToNextTier = LoyaltyTierHelper.GetTierThreshold(LoyaltyTier.Silver)
            });
        }
        
        var nextTier = LoyaltyTierHelper.GetNextTier(account.CurrentTier);
        var pointsToNext = nextTier.HasValue 
            ? LoyaltyTierHelper.GetPointsToNextTier(account.TotalPointsEarned, account.CurrentTier)
            : 0;
        
        return Results.Ok(new 
        { 
            userId = request.UserId, 
            points = account.Points,
            totalPointsEarned = account.TotalPointsEarned,
            currentTier = account.CurrentTier,
            cashbackRate = LoyaltyTierHelper.GetCashbackRate(account.CurrentTier),
            nextTier,
            pointsToNextTier = pointsToNext
        });
    }
}