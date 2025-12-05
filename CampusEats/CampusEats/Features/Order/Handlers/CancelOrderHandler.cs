using CampusEats.Features.Order.Requests;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using CampusEats.Features.Order;

namespace CampusEats.Features.Order.Handlers;

public class CancelOrderHandler(CampusEatsContext context, ILogger<CancelOrderHandler> logger)
{
    public async Task<IResult> Handle(CancelOrderRequest request)
    {
        logger.LogInformation("Cancel order {OrderId}", request.OrderId);

        var validator = new CampusEats.Validators.Order.CancelOrderValidator();
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            foreach (var e in validation.Errors) logger.LogError(e.ErrorMessage);
            return Results.BadRequest(validation.Errors);
        }

        var order = await context.Order.FindAsync(request.OrderId);
        if (order is null)
        {
            logger.LogWarning("Order {OrderId} not found", request.OrderId);
            return Results.NotFound($"Order with ID: {request.OrderId} not found");
        }
        if (order.Status != OrderStatus.Pending)
        {
            logger.LogWarning("Order {OrderId} cannot be cancelled from status {Status}", request.OrderId, order.Status);
            return Results.Conflict($"Only pending orders can be cancelled. Current status: {order.Status}");
        }

        var updated = order with { Status = OrderStatus.Cancelled };
        context.Entry(order).CurrentValues.SetValues(updated);
        await context.SaveChangesAsync();

        logger.LogInformation("Order {OrderId} cancelled", request.OrderId);
        return Results.Ok(updated);
    }
}
