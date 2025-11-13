using CampusEats.Features.Order.Requests;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using CampusEats.Features.Order;

namespace CampusEats.Features.Order.Handlers;

public class GetOrderHistoryHandler(CampusEatsContext context, ILogger<GetOrderHistoryHandler> logger)
{
    public async Task<IResult> Handle(GetOrderHistoryRequest request)
    {
        logger.LogInformation("Fetching order history for Client {ClientId}", request.ClientId);
        var orders = await context.Order
            .Where(o => o.ClientId == request.ClientId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
        return Results.Ok(orders);
    }
}