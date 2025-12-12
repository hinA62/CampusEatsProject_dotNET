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
        Console.WriteLine($"\n💳 ===== CreatePaymentHandler START =====");
        Console.WriteLine($"💳 UserId: {request.UserId}");
        Console.WriteLine($"💳 OrderId: {request.OrderId}");
        Console.WriteLine($"💳 Amount: ${request.Amount}");
        Console.WriteLine($"💳 PointsToUse: {request.PointsToUse}");

        // verificăm că există comanda
        var order = await db.Order.FirstOrDefaultAsync
            (o => o.Id == request.OrderId, ct);
        if (order is null)
        {
            Console.WriteLine($"❌ Order not found: {request.OrderId}");
            return Results.NotFound("Order not found");
        }

        Console.WriteLine($"✅ Order found: {order.Id}, Price: ${order.Price}");

        // de verificat și User dacă vrei extra safe:
        var userExists = await db.Users.AnyAsync
            (u => u.Id == request.UserId, ct);
        if (!userExists)
        {
            Console.WriteLine($"❌ User not found: {request.UserId}");
            return Results.NotFound("User not found");
        }

        Console.WriteLine($"✅ User exists: {request.UserId}");

        // Get or create loyalty account
        var account = await db.LoyaltyAccounts.FirstOrDefaultAsync
            (a => a.UserId == request.UserId, ct);
        
        if (account is null)
        {
            Console.WriteLine($"📝 Creating new loyalty account for user {request.UserId}");
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
            Console.WriteLine($"✅ New loyalty account created. Points: {account.Points}, Tier: {account.CurrentTier}");
        }
        else
        {
            Console.WriteLine($"✅ Existing loyalty account found. Current Points: {account.Points}, Tier: {account.CurrentTier}");
        }

        // Handle points usage for discount
        decimal finalAmount = request.Amount;
        int pointsUsed = 0;
        
        if (request.PointsToUse.HasValue && request.PointsToUse.Value > 0)
        {
            if (request.PointsToUse.Value > account.Points)
            {
                Console.WriteLine($"❌ Insufficient points. Have: {account.Points}, Need: {request.PointsToUse.Value}");
                return Results.BadRequest($"Insufficient points. You have {account.Points} points.");
            }
            
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
            
            Console.WriteLine($"🎯 Points redeemed: {pointsUsed}, Discount: ${discount:F2}, FinalAmount: ${finalAmount:F2}");
            
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

            Console.WriteLine($"🎁 ===== LOYALTY EARNING =====");
            Console.WriteLine($"🎁 Current Tier: {account.CurrentTier}");
            Console.WriteLine($"🎁 Cashback Rate: {cashbackRate}x");
            Console.WriteLine($"🎁 Final Amount: ${finalAmount:F2}");
            Console.WriteLine($"🎁 Points to Add: {pointsEarned}");
            Console.WriteLine($"🎁 Current Points Before: {account.Points}");

            // Add points
            account.Points += pointsEarned;
            account.TotalPointsEarned += pointsEarned;
            
            // Update tier based on total points earned
            var newTier = LoyaltyTierHelper.CalculateTier(account.TotalPointsEarned);
            account.CurrentTier = newTier;
            account.UpdatedAtUtc = DateTime.UtcNow;

            Console.WriteLine($"🎁 Current Points After: {account.Points}");
            Console.WriteLine($"🎁 Total Points Earned (All Time): {account.TotalPointsEarned}");
            Console.WriteLine($"🎁 New Tier: {newTier}");

            // Mark account as modified for EF Core
            db.LoyaltyAccounts.Update(account);
            Console.WriteLine($"🎁 ✅ Loyalty account marked for update in EF Core");

            // Create single cashback transaction
            var cashbackTx = new LoyaltyTransaction
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Type = LoyaltyTransactionType.CashbackBonus,
                Points = pointsEarned,
                Description = $"{newTier} tier cashback ({cashbackRate}x points per $) from payment {Guid.NewGuid()}",
                CreatedAtUtc = DateTime.UtcNow
            };
            await db.LoyaltyTransactions.AddAsync(cashbackTx, ct);
            Console.WriteLine($"🎁 ✅ Loyalty transaction created (ID: {cashbackTx.Id})");
        }
        else
        {
            Console.WriteLine($"⚠️ FinalAmount is 0 or negative (${finalAmount:F2}), no loyalty points earned");
        }

        Console.WriteLine($"\n💾 Saving all changes to database...");
        var saveResult = await db.SaveChangesAsync(ct);
        Console.WriteLine($"💾 ✅ SaveChangesAsync returned: {saveResult} changes");
        Console.WriteLine($"💳 ===== CreatePaymentHandler COMPLETE =====\n");

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
