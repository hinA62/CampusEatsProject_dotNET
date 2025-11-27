namespace CampusEats.Features.Order;

public record Order(
    Guid Id,
    Guid ClientId, 
    decimal? Price, 
    List<Guid> MenuIDs,
    List<Guid> ItemIDs,
    DateTime CreatedAt,
    OrderStatus Status
);