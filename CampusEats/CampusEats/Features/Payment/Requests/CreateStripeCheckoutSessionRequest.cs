namespace CampusEats.Features.Payment.Requests;

public record CreateStripeCheckoutSessionRequest(
    Guid UserId,
    Guid OrderId,
    int? PointsToUse = null
    );