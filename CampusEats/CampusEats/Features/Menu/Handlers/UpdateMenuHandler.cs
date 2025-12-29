using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;
using CampusEats.Validators.Menu;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Features.Menu.Handlers;

public class UpdateMenuHandler(CampusEatsContext context, ILogger<UpdateMenuHandler> logger, IValidator<UpdateMenuRequest> validator)
{
    // Mapare între restricții și alergenii care le invalidează
    private static readonly Dictionary<DietaryRestrictions, string[]> RestrictionExclusions = new()
    {
        { DietaryRestrictions.LactoseFree, ["lapte", "lactoză"] },
        { DietaryRestrictions.GlutenFree,  ["gluten", "grâu"] },
        { DietaryRestrictions.NutFree,     ["nuci", "arahide", "migdale", "cashew"] },
        { DietaryRestrictions.NoSeafood,   ["pește", "moluște", "crustacee"] },
        { DietaryRestrictions.DairyFree,   ["brânzeturi", "lapte", "ouă"] }
    };

    public async Task<IResult> Handle(UpdateMenuRequest request)
    {
        logger.LogInformation("Updating menu...");

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

        var menu = await context.Menu.FindAsync(request.Id);
        if (menu == null) return Results.NotFound($"Menu with ID: {request.Id} not found");

        var itemIds = request.ItemIds ?? menu.ItemId;
        var finalRestrictions = await CalculateMenuRestrictions(itemIds, menu.Restrictions);

        var updatedMenu = menu with
        {
            Name = request.Name,
            Price = request.Price,
            ItemId = itemIds,
            Category = request.Category,
            Restrictions = finalRestrictions,
            ImageUrl = request.ImageUrl ?? menu.ImageUrl
        };

        context.Entry(menu).CurrentValues.SetValues(updatedMenu);
        await context.SaveChangesAsync();
        
        return Results.Ok(updatedMenu);
    }

    private async Task<DietaryRestrictions> CalculateMenuRestrictions(List<Guid> itemIds, DietaryRestrictions currentRestrictions)
    {
        var items = await context.MenuItem
            .Where(item => itemIds.Contains(item.Id) && item.Allergens != null)
            .ToListAsync();
        
        var allAllergens = items
            .SelectMany(item => item.Allergens!)
            .Select(a => a.ToLower())
            .Distinct()
            .ToList();

        if (allAllergens.Count == 0) return currentRestrictions;

        var calculated = DietaryRestrictions.None;

        foreach (var check in RestrictionExclusions)
        {
            // Dacă niciunul dintre alergenii interziși nu este prezent, adăugăm restricția
            if (!allAllergens.Any(allergen => check.Value.Any(forbidden => allergen.Contains(forbidden))))
            {
                calculated |= check.Key;
            }
        }

        return calculated == DietaryRestrictions.None ? currentRestrictions : calculated;
    }
}