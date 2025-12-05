
using CampusEats.Features.Payment;

namespace CampusEats.Features.Payment.Requests;

public record CreatePaymentRequest(
    Guid UserId,
    Guid OrderId,
    decimal Amount,
    PaymentMethod Method);
