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

        // deocamdată simulăm un payment de succes (mock)
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            OrderId = request.OrderId,
            Amount = request.Amount,
            Method = request.Method,
            Status = PaymentStatus.Succeeded,
            CreatedAtUtc = DateTime.UtcNow,
            ExternalReference = $"MOCK-{Guid.NewGuid()}"
        };

        await db.Payments.AddAsync(payment, ct);

        // Integrare simplă cu Loyalty: 1 leu = 1 punct
        var points = (int)Math.Round(payment.Amount);

        var account = await db.LoyaltyAccounts.FirstOrDefaultAsync
            (a => a.UserId == request.UserId, ct);
        if (account is null)
        {
            account = new LoyaltyAccount
            {
                UserId = request.UserId,
                Points = 0,
                UpdatedAtUtc = DateTime.UtcNow
            };
            await db.LoyaltyAccounts.AddAsync(account, ct);
        }

        account.Points += points;
        account.UpdatedAtUtc = DateTime.UtcNow;

        var tx = new LoyaltyTransaction
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Type = LoyaltyTransactionType.Earn,
            Points = points,
            Description = $"Points earned from payment {payment.Id}",
            CreatedAtUtc = DateTime.UtcNow
        };

        await db.LoyaltyTransactions.AddAsync(tx, ct);

        await db.SaveChangesAsync(ct);

        return Results.Created($"/api/payments/{payment.Id}", payment);
    }
}
