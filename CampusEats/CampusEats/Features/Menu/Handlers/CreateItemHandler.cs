using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;
using CampusEats.Validators.Menu;

namespace CampusEats.Features.Menu.Handlers;

public class CreateItemHandler (CampusEatsContext context, ILogger<CreateItemHandler> logger)
{
    public async Task<IResult> Handle(CreateItemRequest request)
    {
        logger.LogInformation($"Creating menu item {request.Name}");
        
        //data validation
        var validator = new CreateItemValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                logger.LogError(error.ErrorMessage);
            }

            return Results.BadRequest(validationResult.Errors);
        }
        
        //create menu item
        var menuItem = new MenuItem(Guid.NewGuid(), request.Name, request.Price, request.ImageUrl, request.Allergens);
        context.MenuItem.Add(menuItem);
        await context.SaveChangesAsync();
        logger.LogInformation("Menu item created with ID: {MenuItemId}", menuItem.Id);
        
        return Results.Created($"/menu/{menuItem.Id}", menuItem);
    }
}