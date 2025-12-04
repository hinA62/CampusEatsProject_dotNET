using System.Diagnostics;
using CampusEats.Features.Order.Requests;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using CampusEats.Features.Order;

namespace CampusEats.Features.Order.Handlers;

public class PlaceOrderHandler(CampusEatsContext context, ILogger<PlaceOrderHandler> logger)
{
    public async Task<IResult> Handle(PlaceOrderRequest request)
    {
        logger.LogInformation("Placing order for Client {ClientId}", request.ClientId);

        var validator = new CampusEats.Validators.Order.PlaceOrderValidator();
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            foreach (var e in validation.Errors) logger.LogError(e.ErrorMessage);
            return Results.BadRequest(validation.Errors);
        }

        Debug.Assert(request.MenuIDs != null, "request.MenuIDs != null");
        var menuIds = request.MenuIDs.Distinct().ToList();
        var itemIds = request.ItemIDs.Distinct().ToList();

        var menus = await context.Menu
            .Where(m => menuIds.Contains(m.Id))
            .Select(m => new { m.Id, m.Price })
            .ToListAsync();
        var items = await context.MenuItem
            .Where(i => itemIds.Contains(i.Id))
            .Select(i => new { i.Id, i.Price })
            .ToListAsync();

        var missingMenus = menuIds.Except(menus.Select(m => m.Id)).ToList();
        var missingItems = itemIds.Except(items.Select(i => i.Id)).ToList();
        if (missingMenus.Any() || missingItems.Any())
        {
            logger.LogWarning("Missing refs. Menus: {Menus} Items: {Items}",
                string.Join(',', missingMenus), string.Join(',', missingItems));
            return Results.BadRequest(new { Message = "Some MenuIDs/ItemIDs do not exist", 
                MissingMenuIDs = missingMenus, MissingItemIDs = missingItems });
        }

        decimal? total = menus.Sum(m => m.Price) + items.Sum(i => i.Price);

        var order = new Order(
            Id: Guid.NewGuid(),
            ClientId: request.ClientId,
            Price: (decimal)total,
            MenuIDs: menuIds,
            ItemIDs: itemIds,
            CreatedAt: DateTime.UtcNow,
            Status: OrderStatus.Pending
        );

        context.Order.Add(order);
        await context.SaveChangesAsync();

        logger.LogInformation("Order {OrderId} created for Client {ClientId}", order.Id, order.ClientId);
        return Results.Created($"/orders/{order.Id}", order);
    }
}