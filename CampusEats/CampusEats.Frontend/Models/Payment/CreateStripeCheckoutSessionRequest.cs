namespace CampusEatsFrontend.Models.Payment;

public class CreateStripeCheckoutSessionRequest
{
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }
    public int? PointsToUse { get; set; }
}

public class StripeCheckoutSessionResponse
{
    public string CheckoutUrl { get; set; } = string.Empty;
}