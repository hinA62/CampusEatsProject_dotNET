namespace CampusEats.Features.Order.Requests;

public record PlaceOrderRequest
(
    Guid ClientId,
    List<Guid> MenuIDs,
    List<Guid> ItemIDs
);