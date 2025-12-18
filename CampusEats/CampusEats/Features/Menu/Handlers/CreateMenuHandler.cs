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
            validationResult.Errors.ForEach(e => logger.LogError(e.ErrorMessage));
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
        var finalRestrictions = CalculateFinalRestrictions(allAllergens, request.Restrictions);

        logger.LogInformation("Menu restrictions calculated from allergens.");

        // 4. Creare și salvare
        Debug.Assert(request.ItemIds != null, "request.ItemIds != null");
        var menu = new Menu(Guid.NewGuid(), request.Name, request.Price, request.ItemIds, 
                            request.Category, finalRestrictions, request.ImageUrl);
        
        context.Menu.Add(menu);
        await context.SaveChangesAsync();

        return Results.Created($"/menu/{menu.Name}", menu);
    }

    private DietaryRestrictions CalculateFinalRestrictions(List<string> allergens, DietaryRestrictions requestedRestrictions)
    {
        var calculated = DietaryRestrictions.None;

        // Mapare cuvinte cheie -> Restricție
        var rules = new Dictionary<DietaryRestrictions, string[]>
        {
            { DietaryRestrictions.LactoseFree, ["lapte", "lactoză"] },
            { DietaryRestrictions.GlutenFree,  ["gluten", "grâu"] },
            { DietaryRestrictions.NutFree,     ["nuci", "arahide", "migdale", "cashew"] },
            { DietaryRestrictions.NoSeafood,   ["pește", "moluște", "crustacee"] },
            { DietaryRestrictions.DairyFree,   ["brânzeturi", "lapte", "ouă"] }
        };

        foreach (var rule in rules)
        {
            if (!allergens.Any(a => rule.Value.Any(keyword => a.Contains(keyword))))
            {
                calculated |= rule.Key;
            }
        }

        return calculated != DietaryRestrictions.None ? calculated : requestedRestrictions;
    }
}