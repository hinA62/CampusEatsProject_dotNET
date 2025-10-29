using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;
using CampusEats.Validators.Menu;

namespace CampusEats.Features.Menu.Handlers;

public class UpdateMenuHandler (CampusEatsContext context, ILogger<UpdateMenuHandler> logger)
{
    public async Task<IResult> Handle(UpdateMenuRequest request)
    {
        logger.LogInformation($"Updating menu with ID: {request.Id}");
        
        //data validation
        var validator = new UpdateMenuValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                logger.LogError(error.ErrorMessage);
            }
            
            return Results.BadRequest(validationResult.Errors);
        }
        
        //update menu
        var menu = await context.Menu.FindAsync(request.Id);
        if (menu == null)
        {
            logger.LogWarning("Menu with ID: {MenuId} not found", request.Id);
            return Results.NotFound($"Menu with ID: {request.Id} not found");
        }
        var updatedMenu = menu with
        {
            Name = request.Name,
            Price = request.Price,
            ItemId = request.ItemId,
            Category = request.Category,
            Restrictions = request.Restrictions
        };
        context.Entry(menu).CurrentValues.SetValues(updatedMenu);
        await context.SaveChangesAsync();
        logger.LogInformation("Menu with ID: {MenuId} updated successfully", request.Id);
        
        return Results.Ok(updatedMenu);
    }
}