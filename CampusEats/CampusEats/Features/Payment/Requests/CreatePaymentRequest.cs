
using CampusEats.Features.Payment;

namespace CampusEats.Features.Payment.Requests;

public record CreatePaymentRequest(
    Guid UserId,
    Guid? OrderId, // Optional: if null, will create order from MenuIDs/ItemIDs
    decimal Amount,
    PaymentMethod Method,
    int? PointsToUse = null, // Optional: points to use for discount
    List<Guid>? MenuIDs = null, // Optional: for creating order
    List<Guid>? ItemIDs = null); // Optional: for creating order