using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;
using CampusEats.Validators.Menu;

namespace CampusEats.Features.Menu.Handlers;

public class CreateItemHandler (CampusEatsContext context, ILogger<CreateItemHandler> logger)
{
    public async Task<IResult> Handle(CreateItemRequest request)
    {
        logger.LogInformation("Creating menu item...");
        
        //data validation
        var validator = new CreateItemValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                logger.LogError("A validation error occurred.");
            }

            return Results.BadRequest(validationResult.Errors);
        }
        
        //create menu item
        var menuItem = new MenuItem(Guid.NewGuid(), request.Name, (decimal)request.Price!, request.ImageUrl, request.Allergens);
        context.MenuItem.Add(menuItem);
        await context.SaveChangesAsync();
        logger.LogInformation("Menu item created successfully.");
        
        return Results.Created($"/menu/{menuItem.Id}", menuItem);
    }
}