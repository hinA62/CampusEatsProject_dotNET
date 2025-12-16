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
        
        var calculatedRestrictions = DietaryRestrictions.FoodAllergyFriendly;
        
        if (!allAllergens.Any(a => a.Contains("Lapte") || 
                                   a.Contains("Lactoză")))
        {
            calculatedRestrictions |= DietaryRestrictions.LactoseFree;
        }
        
        if (!allAllergens.Any(a => a.Contains("Gluten") || 
                                   a.Contains("Grâu")))
        {
            calculatedRestrictions |= DietaryRestrictions.GlutenFree;
        }
        
        if (!allAllergens.Any(a => a.Contains("Nuci") ||
                                   a.Contains("Arahide") || 
                                   a.Contains("Migdale") || 
                                   a.Contains("Cashew")))
        {
            calculatedRestrictions |= DietaryRestrictions.NutFree;
        }
        
        if (!allAllergens.Any(a => a.Contains("Pește") ||
                                   a.Contains("Moluște") || 
                                   a.Contains("Crustacee")))
        {
            calculatedRestrictions |= DietaryRestrictions.NoSeafood;
        }

        if (!allAllergens.Any(a => a.Contains("Brânzeturi") ||
                                   a.Contains("Lapte") ||
                                   a.Contains("Ouă")))
        {
            calculatedRestrictions |= DietaryRestrictions.DairyFree;
        }
        
        var finalRestrictions = calculatedRestrictions != DietaryRestrictions.FoodAllergyFriendly
            ? calculatedRestrictions 
            : menu.Restrictions;
        
        logger.LogInformation("Menu restrictions: {Restrictions} (calculated from allergens: [{Allergens}])", 
            finalRestrictions, allAllergens.Count > 0 ? string.Join(", ", allAllergens) : "none");
        
        var updatedMenu = menu with
        {
            Name = request.Name,
            Price = request.Price ?? menu.Price,
            ItemId = itemIds,
            Category = request.Category,
            Restrictions = finalRestrictions,
            ImageUrl = request.ImageUrl ?? menu.ImageUrl
        };
        context.Entry(menu).CurrentValues.SetValues(updatedMenu);
        await context.SaveChangesAsync();
        logger.LogInformation("Menu with ID: {MenuId} updated successfully", request.Id);
        
        return Results.Ok(updatedMenu);
    }
}