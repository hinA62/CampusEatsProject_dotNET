using CampusEats.Features.Order.Requests;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using CampusEats.Features.Order;

namespace CampusEats.Features.Order.Handlers;

public class GetOrderByIdHandler(CampusEatsContext context, ILogger<GetOrderByIdHandler> logger)
{
    public async Task<IResult> Handle(GetOrderByIdRequest request)
    {
        logger.LogInformation("Get order {OrderId}", request.OrderId);
        var order = await context.Order.FindAsync(request.OrderId);
        return order is null ? Results.NotFound($"Order with ID: {request.OrderId} not found") : Results.Ok(order);
    }
}