using System.Text;
using CampusEats.Features.Payment.Requests;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace CampusEats.Features.Payment.Handlers;

public class StripeWebhookHandler
{
    private readonly IConfiguration _config;
    private readonly CreatePaymentHandler _paymentHandler;

    public StripeWebhookHandler(IConfiguration config, CreatePaymentHandler paymentHandler)
    {
        _config = config;
        _paymentHandler = paymentHandler;
    }

    public async Task<IResult> Handle(HttpRequest request, CancellationToken ct = default)
    {
        var json = await new StreamReader(request.Body).ReadToEndAsync();
        var signature = request.Headers["Stripe-Signature"].ToString();
        var webhookSecret = _config["Stripe:WebhookSecret"];

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, signature, webhookSecret, throwOnApiVersionMismatch: false);
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"Webhook signature validation failed: {ex.Message}");
        }

        if (stripeEvent.Type != Events.CheckoutSessionCompleted)
        {
            return Results.Ok(); // ignorăm alte event-uri
        }

        var session = stripeEvent.Data.Object as Session;
        if (session is null)
            return Results.BadRequest();

        var metadata = session.Metadata ?? new Dictionary<string, string>();

        if (!metadata.TryGetValue("userId", out var userIdStr) ||
            !metadata.TryGetValue("orderId", out var orderIdStr) ||
            !Guid.TryParse(userIdStr, out var userId) ||
            !Guid.TryParse(orderIdStr, out var orderId))
        {
            return Results.BadRequest("Missing metadata.");
        }

        int? pointsToUse = null;
        if (metadata.TryGetValue("pointsToUse", out var pointsStr) &&
            int.TryParse(pointsStr, out var pts) &&
            pts > 0)
        {
            pointsToUse = pts;
        }

        var amountTotal = (decimal)(session.AmountTotal ?? 0) / 100m;

        var req = new CreatePaymentRequest(
            userId,
            orderId,
            amountTotal,
            PaymentMethod.StripeTest,
            pointsToUse
        );

        // folosim CreatePaymentHandler-ul tău existent (loyalty, tiers, etc.)
        return await _paymentHandler.Handle(req, ct);
    }
}