namespace CampusEats.Features.Kitchen.Requests;

public record GetPendingOrdersRequest(
    string? Status = null // Optional: filter by specific status (Pending, Preparing, etc.)
);
