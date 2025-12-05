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
        

        var menuItems = await context.MenuItem
            .Where(item => request.ItemIds != null && request.ItemIds.Contains(item.Id))
            .ToListAsync();
        

        var allAllergens = menuItems
            .Where(item => item.Allergens != null)
            .SelectMany(item => item.Allergens!)
            .Select(a => a.ToLower())
            .Distinct()
            .ToList();
        
        var calculatedRestrictions = DietaryRestrictions.None;
        
        if (!allAllergens.Any(a => a.Contains("dairy") || 
                                   a.Contains("milk") || 
                                   a.Contains("lactose") || 
                                   a.Contains("cheese")))
        {
            calculatedRestrictions |= DietaryRestrictions.LactoseFree;
        }
        
        if (!allAllergens.Any(a => a.Contains("gluten") || 
                                   a.Contains("wheat")))
        {
            calculatedRestrictions |= DietaryRestrictions.GlutenFree;
        }
        
        if (!allAllergens.Any(a => a.Contains("nut") ||
                                   a.Contains("peanut") || 
                                   a.Contains("almond") || 
                                   a.Contains("cashew")))
        {
            calculatedRestrictions |= DietaryRestrictions.NutFree;
        }
        
        var finalRestrictions = calculatedRestrictions != DietaryRestrictions.None 
            ? calculatedRestrictions 
            : request.Restrictions;
        
        logger.LogInformation("Menu restrictions: {Restrictions} (calculated from allergens: [{Allergens}])", 
            finalRestrictions, allAllergens.Count > 0 ? string.Join(", ", allAllergens) : "none");
        

        Debug.Assert(request.ItemIds != null, "request.ItemIds != null");
        var menu = new Menu(Guid.NewGuid(), request.Name, 
            request.Price, request.ItemIds, request.Category, finalRestrictions);
        context.Menu.Add(menu);
        await context.SaveChangesAsync();
        logger.LogInformation("Menu created with Name: {MenuName}", menu.Name);
        
        return Results.Created($"/menu/{menu.Name}", menu);
    }
    
}
