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
        logger.LogInformation("Creating menu...");

        // 1. Validare date
        var validationResult = await new CreateMenuValidator().ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            validationResult.Errors.ForEach(e => logger.LogInformation("A validation error occurred."));
            return Results.BadRequest(validationResult.Errors);
        }

        // 2. Obținere iteme și alergeni
        var menuItems = await context.MenuItem
            .Where(item => request.ItemIds != null && request.ItemIds.Contains(item.Id))
            .ToListAsync();

        var allAllergens = menuItems
            .SelectMany(item => item.Allergens ?? Enumerable.Empty<string>())
            .Select(a => a.ToLower())
            .Distinct()
            .ToList();

        // 3. Calcul restricții (Logic extrasă pentru a reduce complexitatea)
        var finalRestrictions = CalculateFinalRestrictions(allAllergens);

        logger.LogInformation("Menu restrictions calculated from allergens.");

        // 4. Creare și salvare
        Debug.Assert(request.ItemIds != null, "request.ItemIds != null");
        var menu = new Menu(Guid.NewGuid(), request.Name, request.Price, request.ItemIds, 
                            request.Category, finalRestrictions, request.ImageUrl);
        
        context.Menu.Add(menu);
        await context.SaveChangesAsync();

        return Results.Created($"/menu/{menu.Name}", menu);
    }

    private static DietaryRestrictions CalculateFinalRestrictions(List<string> allergens)
    {
        // Start with ALL restrictions
        var calculated = DietaryRestrictions.LactoseFree | 
                        DietaryRestrictions.GlutenFree | 
                        DietaryRestrictions.NutFree | 
                        DietaryRestrictions.DairyFree | 
                        DietaryRestrictions.NoSeafood;

        // Remove restriction if allergen found
        if (allergens.Any(a => a.Contains("lapte", StringComparison.OrdinalIgnoreCase) || 
                              a.Contains("lactoză", StringComparison.OrdinalIgnoreCase)))
            calculated &= ~DietaryRestrictions.LactoseFree;

        if (allergens.Any(a => a.Contains("gluten", StringComparison.OrdinalIgnoreCase)))
            calculated &= ~DietaryRestrictions.GlutenFree;

        if (allergens.Any(a => a.Contains("nuci", StringComparison.OrdinalIgnoreCase) || 
                              a.Contains("alune", StringComparison.OrdinalIgnoreCase)))
            calculated &= ~DietaryRestrictions.NutFree;

        if (allergens.Any(a => a.Contains("lactate", StringComparison.OrdinalIgnoreCase) ||
                              a.Contains("brânză", StringComparison.OrdinalIgnoreCase)))
            calculated &= ~DietaryRestrictions.DairyFree;

        if (allergens.Any(a => a.Contains("pește", StringComparison.OrdinalIgnoreCase) || 
                              a.Contains("fructe de mare", StringComparison.OrdinalIgnoreCase)))
            calculated &= ~DietaryRestrictions.NoSeafood;

        return calculated;
    }
}