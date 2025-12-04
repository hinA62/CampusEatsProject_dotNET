using CampusEats.Features.Kitchen.Requests;
using CampusEats.Features.Order;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusEats.Features.Kitchen.Handlers;

public class GetPendingOrdersHandler(CampusEatsContext context, ILogger<GetPendingOrdersHandler> logger)
{
    public async Task<IResult> Handle(GetPendingOrdersRequest request)
    {
        logger.LogInformation("Fetching orders with status filter: {Status}", request.Status ?? "All");

        IQueryable<Order.Order> query = context.Order;

        // Filter by status if provided
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<OrderStatus>(request.Status, ignoreCase: true, out var statusEnum))
            {
                query = query.Where(o => o.Status == statusEnum);
            }
            else
            {
                logger.LogWarning("Invalid status filter provided: {Status}", request.Status);
                return Results.BadRequest(new { Message = $"Invalid status: {request.Status}" });
            }
        }
        else
        {
            // By default, show only active orders (not Completed or Cancelled)
            query = query.Where(o => o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled);
        }

        var orders = await query
            .OrderBy(o => o.CreatedAt)
            .ToListAsync();

        logger.LogInformation("Found {Count} orders", orders.Count);

        return Results.Ok(orders);
    }
}
