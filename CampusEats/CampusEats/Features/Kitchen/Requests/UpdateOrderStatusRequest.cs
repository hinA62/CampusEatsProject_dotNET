using CampusEats.Features.Order;

namespace CampusEats.Features.Kitchen.Requests;

public record UpdateOrderStatusRequest(
    Guid OrderId,
    OrderStatus NewStatus
);
