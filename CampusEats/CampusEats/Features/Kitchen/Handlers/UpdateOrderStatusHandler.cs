using CampusEats.Features.Kitchen.Requests;
using CampusEats.Features.Order;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusEats.Features.Kitchen.Handlers;

public class UpdateOrderStatusHandler(CampusEatsContext context, ILogger<UpdateOrderStatusHandler> logger)
{
    public async Task<IResult> Handle(UpdateOrderStatusRequest request)
    {
        logger.LogInformation("Updating order {OrderId} to status {Status}", request.OrderId, request.NewStatus);

        var validator = new CampusEats.Validators.Kitchen.UpdateOrderStatusValidator();
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
            return Results.NotFound(new { Message = "Order not found" });
        }


        var isValidTransition = IsValidStatusTransition(order.Status, request.NewStatus);
        if (!isValidTransition)
        {
            logger.LogWarning("Invalid status transition from {OldStatus} to {NewStatus}", order.Status, request.NewStatus);
            return Results.BadRequest(new 
            { 
                Message = $"Invalid status transition from {order.Status} to {request.NewStatus}",
                CurrentStatus = order.Status,
                RequestedStatus = request.NewStatus
            });
        }


        var updatedOrder = order with { Status = request.NewStatus };
        
        context.Order.Remove(order);
        context.Order.Add(updatedOrder);
        await context.SaveChangesAsync();

        logger.LogInformation("Order {OrderId} status updated from {OldStatus} to {NewStatus}", 
            request.OrderId, order.Status, request.NewStatus);

        return Results.Ok(new
        {
            Message = "Order status updated successfully",
            Order = updatedOrder
        });
    }

    private static bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {

        return (currentStatus, newStatus) switch
        {

            (OrderStatus.Pending, OrderStatus.Confirmed) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,


            (OrderStatus.Confirmed, OrderStatus.Preparing) => true,
            (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,


            (OrderStatus.Preparing, OrderStatus.Completed) => true,


            _ when currentStatus == newStatus => true,


            _ => false
        };
    }
}
