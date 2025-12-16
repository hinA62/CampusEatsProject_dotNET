using System.Diagnostics;
using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;
using CampusEats.Validators.Menu;
using Microsoft.EntityFrameworkCore;

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
        
        // calculeaza automat restrictii 
        var menuItems = await context.MenuItem
            .Where(item => request.ItemIds != null && request.ItemIds.Contains(item.Id))
            .ToListAsync();
        
        // obtine alergeni
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
            : request.Restrictions;
        
        logger.LogInformation("Menu restrictions: {Restrictions} (calculated from allergens: [{Allergens}])", 
            finalRestrictions, allAllergens.Count > 0 ? string.Join(", ", allAllergens) : "none");
        
        //create a menu
        Debug.Assert(request.ItemIds != null, "request.ItemIds != null");
        var menu = new Menu(Guid.NewGuid(), request.Name, 
            request.Price, request.ItemIds, request.Category, finalRestrictions, request.ImageUrl);
        context.Menu.Add(menu);
        await context.SaveChangesAsync();
        logger.LogInformation("Menu created with Name: {MenuName}", menu.Name);
        
        return Results.Created($"/menu/{menu.Name}", menu);
    }
    
}