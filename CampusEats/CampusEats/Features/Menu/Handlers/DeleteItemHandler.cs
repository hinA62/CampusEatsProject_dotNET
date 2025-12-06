using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Features.Menu.Handlers;

public class DeleteItemHandler(CampusEatsContext context, ILogger<DeleteItemHandler> logger)
{
    public async Task<IResult> Handle(DeleteItemRequest request)
    {
        logger.LogInformation("Delete MenuItem with ID: {MenuItemId}", request.Id);
        var menuItem = await context.MenuItem.FindAsync(request.Id);
        if (menuItem == null)
        {
            logger.LogInformation("MenuItem with ID: {MenuItemId} not found", request.Id);
            return Results.NotFound($"MenuItem with ID: {request.Id} not found");
        }

        // Remove this item from all menus that contain it
        // Load all menus and filter in memory to avoid PostgreSQL array query issues
        var allMenus = await context.Menu.ToListAsync();
        var menusContainingItem = allMenus.Where(m => m.ItemId.Contains(request.Id)).ToList();

        foreach (var menu in menusContainingItem)
        {
            var updatedItemIds = menu.ItemId.Where(id => id != request.Id).ToList();
            var updatedMenu = menu with { ItemId = updatedItemIds };
            context.Entry(menu).CurrentValues.SetValues(updatedMenu);
            logger.LogInformation("Removed MenuItem {MenuItemId} from Menu {MenuId}", request.Id, menu.Id);
        }

        context.MenuItem.Remove(menuItem);
        await context.SaveChangesAsync();
        logger.LogInformation("Deleted MenuItem with ID: {MenuItemId}", menuItem.Id);
        
        return Results.NoContent();
    }
    
}