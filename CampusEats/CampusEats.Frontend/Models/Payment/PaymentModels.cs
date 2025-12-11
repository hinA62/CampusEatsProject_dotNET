namespace CampusEatsFrontend.Models.Payment;

public enum PaymentStatus
{
    Pending,
    Succeeded,
    Failed
}

public enum PaymentMethod
{
    MockCard,
    StripeTest
}

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public PaymentMethod Method { get; set; }
    public string? ExternalReference { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class CreatePaymentRequest
{
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public int? PointsToUse { get; set; }
}
