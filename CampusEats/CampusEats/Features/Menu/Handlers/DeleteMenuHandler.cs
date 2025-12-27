using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;

namespace CampusEats.Features.Menu.Handlers;

public class DeleteMenuHandler (CampusEatsContext context, ILogger<DeleteMenuHandler> logger)
{
    public async Task<IResult> Handle(DeleteMenuRequest request)
    {
        logger.LogInformation("Deleting menu with ID: {RequestId}", request.Id);
        
        var menu = await context.Menu.FindAsync(request.Id);
        if (menu == null)
        {
            logger.LogWarning("Menu not found");
            return Results.NotFound("Menu not found");
        }
        
        context.Menu.Remove(menu);
        await context.SaveChangesAsync();
        logger.LogInformation("Menu deleted successfully");
        
        return Results.Ok("Menu deleted successfully");
    }
}