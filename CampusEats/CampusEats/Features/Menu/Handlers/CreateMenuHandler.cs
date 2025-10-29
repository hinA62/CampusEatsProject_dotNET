using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;
using CampusEats.Validators.Menu;

namespace CampusEats.Features.Menu.Handlers;

public class CreateMenuHandler(CampusEatsContext context, ILogger<CreateMenuHandler> logger)
{
    public async Task<IResult> Handle(CreateMenuRequest request)
    {
        logger.LogInformation($"Creating menu {request.Name}");
        
        //data validation
        var validator = new CreateMenuValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                logger.LogError(error.ErrorMessage);
            }

            return Results.BadRequest(validationResult.Errors);
        }
        
        //create menu
        var menu = new Menu(Guid.NewGuid(), request.Name, request.Price, request.ItemIds, request.Category, request.Restrictions);
        context.Menu.Add(menu);
        await context.SaveChangesAsync();
        logger.LogInformation("Menu created with Name: {MenuName}", menu.Name);
        
        return Results.Created($"/menu/{menu.Name}", menu);
    }
    
}