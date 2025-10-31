using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;
using CampusEats.Validators.Menu;
using Microsoft.EntityFrameworkCore;

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
        
        var itemIds = request.ItemIds ?? menu.ItemId;
        
        var menuItems = await context.MenuItem
            .Where(item => itemIds.Contains(item.Id))
            .ToListAsync();
        
        var allAllergens = menuItems
            .Where(item => item.Allergens != null)
            .SelectMany(item => item.Allergens!)
            .Select(a => a.ToLower())
            .Distinct()
            .ToList();
        
        var calculatedRestrictions = DietaryRestrictions.None;
        
        if (!allAllergens.Any(a => a.Contains("dairy") || a.Contains("milk") || a.Contains("lactose") || a.Contains("cheese")))
        {
            calculatedRestrictions |= DietaryRestrictions.LactoseFree;
        }
        
        if (!allAllergens.Any(a => a.Contains("gluten") || a.Contains("wheat")))
        {
            calculatedRestrictions |= DietaryRestrictions.GlutenFree;
        }
        
        if (!allAllergens.Any(a => a.Contains("nut") || a.Contains("peanut") || a.Contains("almond") || a.Contains("cashew")))
        {
            calculatedRestrictions |= DietaryRestrictions.NutFree;
        }
        
        var finalRestrictions = calculatedRestrictions != DietaryRestrictions.None 
            ? calculatedRestrictions 
            : menu.Restrictions;
        
        logger.LogInformation("Menu restrictions: {Restrictions} (calculated from allergens: [{Allergens}])", 
            finalRestrictions, allAllergens.Count > 0 ? string.Join(", ", allAllergens) : "none");
        
        var updatedMenu = menu with
        {
            Name = request.Name,
            Price = request.Price,
            ItemId = itemIds,
            Category = request.Category,
            Restrictions = finalRestrictions
        };
        context.Entry(menu).CurrentValues.SetValues(updatedMenu);
        await context.SaveChangesAsync();
        logger.LogInformation("Menu with ID: {MenuId} updated successfully", request.Id);
        
        return Results.Ok(updatedMenu);
    }
}