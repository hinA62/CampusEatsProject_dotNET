using CampusEats.Features.Payment.Requests;
using CampusEats.Features.Loyalty;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Features.Payment.Handlers;

public class CreatePaymentHandler(CampusEatsContext db)
{
    public async Task<IResult> Handle
        (CreatePaymentRequest request, CancellationToken ct = default)
    {
        // verificăm că există comanda
        var order = await db.Order.FirstOrDefaultAsync
            (o => o.Id == request.OrderId, ct);
        if (order is null)
            return Results.NotFound("Order not found");

        // de verificat și User dacă vrei extra safe:
        var userExists = await db.Users.AnyAsync
            (u => u.Id == request.UserId, ct);
        if (!userExists)
            return Results.NotFound("User not found");

        // Get or create loyalty account
        var account = await db.LoyaltyAccounts.FirstOrDefaultAsync
            (a => a.UserId == request.UserId, ct);
        
        if (account is null)
        {
            account = new LoyaltyAccount
            {
                UserId = request.UserId,
                Points = 0,
                TotalPointsEarned = 0,
                CurrentTier = LoyaltyTier.Bronze,
                UpdatedAtUtc = DateTime.UtcNow
            };
            await db.LoyaltyAccounts.AddAsync(account, ct);
            await db.SaveChangesAsync(ct); // Save to get the account
        }

        // Handle points usage for discount
        decimal finalAmount = request.Amount;
        int pointsUsed = 0;
        
        if (request.PointsToUse.HasValue && request.PointsToUse.Value > 0)
        {
            if (request.PointsToUse.Value > account.Points)
                return Results.BadRequest($"Insufficient points. You have {account.Points} points.");
            
            // 100 points = $1 discount
            pointsUsed = request.PointsToUse.Value;
            decimal discount = pointsUsed / (decimal)LoyaltyTierHelper.POINTS_PER_DOLLAR;
            
            if (discount > request.Amount)
            {
                // Can't discount more than the order amount
                discount = request.Amount;
                pointsUsed = (int)Math.Floor(request.Amount * LoyaltyTierHelper.POINTS_PER_DOLLAR);
            }
            
            finalAmount = request.Amount - discount;
            
            // Deduct points
            account.Points -= pointsUsed;
            account.UpdatedAtUtc = DateTime.UtcNow;
            
            // Create redeem transaction
            var redeemTx = new LoyaltyTransaction
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Type = LoyaltyTransactionType.Redeem,
                Points = -pointsUsed, // Negative for deduction
                Description = $"Redeemed {pointsUsed} points for ${discount:F2} discount on order {order.Id}",
                CreatedAtUtc = DateTime.UtcNow
            };
            await db.LoyaltyTransactions.AddAsync(redeemTx, ct);
        }

        // deocamdată simulăm un payment de succes (mock)
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            OrderId = request.OrderId,
            Amount = finalAmount, // Use final amount after discount
            Method = request.Method,
            Status = PaymentStatus.Succeeded,
            CreatedAtUtc = DateTime.UtcNow,
            ExternalReference = $"MOCK-{Guid.NewGuid()}"
        };

        await db.Payments.AddAsync(payment, ct);

        // Loyalty integration with tier system
        // Only earn points if payment amount > 0 (not fully paid with points)
        if (finalAmount > 0)
        {
            // Earn points based on tier cashback rate
            // Bronze: 1 point/$, Silver: 3 points/$, Gold: 5 points/$, etc.
            var cashbackRate = LoyaltyTierHelper.GetCashbackRate(account.CurrentTier);
            var pointsEarned = (int)Math.Floor(finalAmount * cashbackRate);

            // Add points
            account.Points += pointsEarned;
            account.TotalPointsEarned += pointsEarned;
            
            // Update tier based on total points earned
            var newTier = LoyaltyTierHelper.CalculateTier(account.TotalPointsEarned);
            account.CurrentTier = newTier;
            account.UpdatedAtUtc = DateTime.UtcNow;

            // Create single cashback transaction
            var cashbackTx = new LoyaltyTransaction
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Type = LoyaltyTransactionType.CashbackBonus,
                Points = pointsEarned,
                Description = $"{account.CurrentTier} tier cashback ({cashbackRate}x points per $) from payment {payment.Id}",
                CreatedAtUtc = DateTime.UtcNow
            };
            await db.LoyaltyTransactions.AddAsync(cashbackTx, ct);
        }

        await db.SaveChangesAsync(ct);

        return Results.Created($"/api/payments/{payment.Id}", new 
        { 
            payment,
            pointsUsed,
            discount = request.Amount - finalAmount,
            finalAmount,
            pointsEarned = finalAmount > 0 ? (int)Math.Round(finalAmount) : 0,
            cashbackEarned = finalAmount > 0 ? (int)Math.Round(finalAmount * LoyaltyTierHelper.GetCashbackRate(account.CurrentTier)) : 0,
            currentTier = account.CurrentTier,
            totalPoints = account.Points
        });
    }
}
