using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;
using CampusEats.Validators.Menu;

namespace CampusEats.Features.Menu.Handlers;

public class UpdateItemHandler (CampusEatsContext context, ILogger<UpdateItemHandler> logger)
{
    public async Task<IResult> Handle(UpdateItemRequest request)
    {
        logger.LogInformation($"Updating menu item with ID: {request.Id}");
        
        //data validation
        var validator = new UpdateItemValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                logger.LogError(error.ErrorMessage);
            }
            
            return Results.BadRequest(validationResult.Errors);
        }
        
        //update menu item
        var menuItem = await context.MenuItem.FindAsync(request.Id);
        if (menuItem == null)
        {
            logger.LogWarning("Menu item with ID: {MenuItemId} not found", request.Id);
            return Results.NotFound($"Menu item with ID: {request.Id} not found");
        }
        
        var updatedMenuItem = menuItem with
        {
            Name = request.Name,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            Allergens = request.Allergens
        };
        context.Entry(menuItem).CurrentValues.SetValues(updatedMenuItem);
        await context.SaveChangesAsync();
        logger.LogInformation("Menu item with ID: {MenuItemId} updated successfully", request.Id);
        
        return Results.Ok(updatedMenuItem);
    }
    
}