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
        logger.LogInformation("Placing order...");

        var validator = new CampusEats.Validators.Order.PlaceOrderValidator();
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            foreach (var e in validation.Errors) logger.LogError(e.ErrorMessage);
            return Results.BadRequest(validation.Errors);
        }

        Debug.Assert(request.MenuIDs != null, "request.MenuIDs != null");
        
        // Nu folosim Distinct() pentru a păstra cantitățile (duplicatele = cantitate)
        var menuIds = request.MenuIDs.ToList();
        var itemIds = request.ItemIDs.ToList();
        
        // Obținem ID-urile unice pentru verificare
        var uniqueMenuIds = menuIds.Distinct().ToList();
        var uniqueItemIds = itemIds.Distinct().ToList();

        var menus = await context.Menu
            .Where(m => uniqueMenuIds.Contains(m.Id))
            .Select(m => new { m.Id, Price = (decimal?)m.Price })
            .ToListAsync();
        var items = await context.MenuItem
            .Where(i => uniqueItemIds.Contains(i.Id))
            .Select(i => new { i.Id, Price = (decimal)i.Price! })
            .ToListAsync();

        var missingMenus = uniqueMenuIds.Except(menus.Select(m => m.Id)).ToList();
        var missingItems = uniqueItemIds.Except(items.Select(i => i.Id)).ToList();
        if (missingMenus.Any() || missingItems.Any())
        {
            logger.LogWarning("Missing refs. Menus: {Menus} Items: {Items}",
                string.Join(',', missingMenus), string.Join(',', missingItems));
            return Results.BadRequest(new { Message = "Some MenuIDs/ItemIDs do not exist", 
                MissingMenuIDs = missingMenus, MissingItemIDs = missingItems });
        }

        // Calculăm prețul bazat pe fiecare ID din listă (inclusiv duplicate pentru cantități)
        decimal total = 0;
        
        // Pentru fiecare menu ID (inclusiv duplicate), adăugăm prețul
        foreach (var menuId in menuIds)
        {
            var menu = menus.FirstOrDefault(m => m.Id == menuId);
            if (menu != null && menu.Price.HasValue)
            {
                total += menu.Price.Value;
            }
        }
        
        // Pentru fiecare item ID (inclusiv duplicate), adăugăm prețul
        foreach (var itemId in itemIds)
        {
            var item = items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                total += item.Price;
            }
        }

        var order = new Order(
            Id: Guid.NewGuid(),
            ClientId: request.ClientId,
            Price: total,
            MenuIDs: menuIds, // Salvăm CU duplicate pentru a păstra cantitățile!
            ItemIDs: itemIds, // Salvăm CU duplicate pentru a păstra cantitățile!
            CreatedAt: DateTime.UtcNow,
            Status: OrderStatus.Pending
        );

        context.Order.Add(order);
        await context.SaveChangesAsync();

        logger.LogInformation("Order {OrderId} created for Client {ClientId} with total {Total}", order.Id, order.ClientId, total);
        return Results.Created($"/orders/{order.Id}", order);
    }
}