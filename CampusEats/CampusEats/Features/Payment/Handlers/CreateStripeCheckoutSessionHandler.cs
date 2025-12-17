using CampusEats.Features.Payment.Requests;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace CampusEats.Features.Payment.Handlers;
public class CreateStripeCheckoutSessionHandler
{
    private readonly CampusEatsContext _db;
    private readonly IConfiguration _config;

    public CreateStripeCheckoutSessionHandler(CampusEatsContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<IResult> Handle(CreateStripeCheckoutSessionRequest request, CancellationToken ct = default)
    {
        var order = await _db.Order.FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);
        if (order is null)
            return Results.NotFound("Order not found");

        var userExists = await _db.Users.AnyAsync(u => u.Id == request.UserId, ct);
        if (!userExists)
            return Results.NotFound("User not found");
        
        decimal finalAmount = order.Price;
        int pointsToUse = 0;

        if (request.PointsToUse is > 0)
        {
            pointsToUse = request.PointsToUse.Value;
            var maxDiscount = order.Price;              
            var discount = pointsToUse / 100m;           
            if (discount > maxDiscount)
            {
                discount = maxDiscount;
                pointsToUse = (int)Math.Floor(order.Price * 100);
            }

            finalAmount = order.Price - discount;
        }

        if (finalAmount <= 0)
            return Results.BadRequest("Final amount must be > 0 to create a Stripe payment.");

        var clientBaseUrl = _config["Stripe:ClientBaseUrl"]?.TrimEnd('/');
        if (string.IsNullOrEmpty(clientBaseUrl))
            return Results.Problem("Stripe:ClientBaseUrl not configured.");
        
        var options = new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = $"{clientBaseUrl}/payment-success?orderId={order.Id}&session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{clientBaseUrl}/payment/{order.Id}",
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "ron",
                        UnitAmount = (long)(finalAmount * 100), 
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "CampusEats Order",
                            Description = $"Order {order.Id}"
                        }
                    }
                }
            },
            Metadata = new Dictionary<string, string>
            {
                ["userId"] = request.UserId.ToString(),
                ["orderId"] = request.OrderId.ToString(),
                ["pointsToUse"] = pointsToUse.ToString()
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options, cancellationToken: ct);

        return Results.Ok(new { checkoutUrl = session.Url });
    }
}