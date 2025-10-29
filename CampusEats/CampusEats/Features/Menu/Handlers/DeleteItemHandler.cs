using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;

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

        context.MenuItem.Remove(menuItem);
        await context.SaveChangesAsync();
        logger.LogInformation("Deleted MenuItem with ID: {MenuItemId}", menuItem.Id);
        
        return Results.NoContent();
    }
    
}