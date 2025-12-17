using CampusEats.Features.Payment.Requests;
using CampusEats.Features.Loyalty;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;
using OrderEntity = CampusEats.Features.Order.Order;
using CampusEats.Features.Order;

namespace CampusEats.Features.Payment.Handlers;

public class CreatePaymentHandler(CampusEatsContext db)
{
    public async Task<IResult> Handle
        (CreatePaymentRequest request, CancellationToken ct = default)
    {
        // de verificat și User dacă vrei extra safe:
        var userExists = await db.Users.AnyAsync
            (u => u.Id == request.UserId, ct);
        if (!userExists)
        {
            return Results.NotFound("User not found");
        }

        // NEW FLOW: If OrderId is null, create order from MenuIDs/ItemIDs
        OrderEntity? order = null;
        Guid orderId;
        
        if (request.OrderId.HasValue)
        {
            // OLD FLOW: OrderId provided, fetch existing order
            order = await db.Order.FirstOrDefaultAsync
                (o => o.Id == request.OrderId.Value, ct);
            if (order is null)
            {
                return Results.NotFound("Order not found");
            }
            orderId = order.Id;
        }
        else
        {
            // NEW FLOW: Create order from cart data
            if ((request.MenuIDs == null || !request.MenuIDs.Any()) && 
                (request.ItemIDs == null || !request.ItemIDs.Any()))
            {
                return Results.BadRequest("Either OrderId or MenuIDs/ItemIDs must be provided");
            }

            var menuIds = request.MenuIDs ?? new List<Guid>();
            var itemIds = request.ItemIDs ?? new List<Guid>();
            
            // Get unique IDs for validation
            var uniqueMenuIds = menuIds.Distinct().ToList();
            var uniqueItemIds = itemIds.Distinct().ToList();

            var menus = await db.Menu
                .Where(m => uniqueMenuIds.Contains(m.Id))
                .Select(m => new { m.Id, Price = (decimal?)m.Price })
                .ToListAsync(ct);
            var items = await db.MenuItem
                .Where(i => uniqueItemIds.Contains(i.Id))
                .Select(i => new { i.Id, i.Price })
                .ToListAsync(ct);

            var missingMenus = uniqueMenuIds.Except(menus.Select(m => m.Id)).ToList();
            var missingItems = uniqueItemIds.Except(items.Select(i => i.Id)).ToList();
            if (missingMenus.Any() || missingItems.Any())
            {
                return Results.BadRequest(new { Message = "Some MenuIDs/ItemIDs do not exist", 
                    MissingMenuIDs = missingMenus, MissingItemIDs = missingItems });
            }

            // Calculate total price (with duplicates for quantities)
            decimal total = 0;
            foreach (var menuId in menuIds)
            {
                var menu = menus.FirstOrDefault(m => m.Id == menuId);
                if (menu != null && menu.Price.HasValue)
                {
                    total += menu.Price.Value;
                }
            }
            foreach (var itemId in itemIds)
            {
                var item = items.FirstOrDefault(i => i.Id == itemId);
                total += item?.Price ?? 0;
            }

            // Create new order
            order = new OrderEntity(
                Id: Guid.NewGuid(),
                ClientId: request.UserId,
                Price: total,
                MenuIDs: menuIds,
                ItemIDs: itemIds,
                CreatedAt: DateTime.UtcNow,
                Status: OrderStatus.Pending
            );

            db.Order.Add(order);
            await db.SaveChangesAsync(ct); // Save order before payment
            orderId = order.Id;
        }

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
            {
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
            
            // Create redeem transaction
            var redeemTx = new LoyaltyTransaction
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Type = LoyaltyTransactionType.Redeem,
                Points = -pointsUsed, // Negative for deduction
                Description = $"Redeemed {pointsUsed} points for ${discount:F2} discount on order {orderId}",
                CreatedAtUtc = DateTime.UtcNow
            };
            await db.LoyaltyTransactions.AddAsync(redeemTx, ct);
        }

        // deocamdată simulăm un payment de succes (mock)
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            OrderId = orderId, // Use the orderId variable (from existing or newly created order)
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

            // Mark account as modified for EF Core
            db.LoyaltyAccounts.Update(account);

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
