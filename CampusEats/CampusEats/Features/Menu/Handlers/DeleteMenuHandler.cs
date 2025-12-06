using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;

namespace CampusEats.Features.Menu.Handlers;

public class DeleteMenuHandler (CampusEatsContext context, ILogger<DeleteMenuHandler> logger)
{
    public async Task<IResult> Handle(DeleteMenuRequest request)
    {
        logger.LogInformation($"Deleting menu with ID: {request.Id}");
        
        var menu = await context.Menu.FindAsync(request.Id);
        if (menu == null)
        {
            logger.LogWarning("Menu with ID: {MenuId} not found", request.Id);
            return Results.NotFound($"Menu with ID: {request.Id} not found");
        }
        
        context.Menu.Remove(menu);
        await context.SaveChangesAsync();
        logger.LogInformation("Menu with ID: {MenuId} deleted successfully", request.Id);
        
        return Results.Ok($"Menu with ID: {request.Id} deleted successfully");
    }
}