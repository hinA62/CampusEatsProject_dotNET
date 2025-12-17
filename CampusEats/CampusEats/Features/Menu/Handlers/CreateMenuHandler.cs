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
        
        // obtine alergeni din toate item-urile meniului
        var allAllergens = menuItems
            .Where(item => item.Allergens != null)
            .SelectMany(item => item.Allergens!)
            .Select(a => a.ToLower())
            .Distinct()
            .ToList();
        
        // START cu toate restricțiile (presupunem că totul e safe)
        var calculatedRestrictions = DietaryRestrictions.LactoseFree | 
                                     DietaryRestrictions.GlutenFree | 
                                     DietaryRestrictions.NutFree | 
                                     DietaryRestrictions.DairyFree | 
                                     DietaryRestrictions.NoSeafood;
        
        // ELIMINĂ restricțiile dacă găsim alergenii corespunzători
        if (allAllergens.Any(a => a.Contains("lapte") || a.Contains("lactoză")))
        {
            calculatedRestrictions &= ~DietaryRestrictions.LactoseFree;
        }
        
        if (allAllergens.Any(a => a.Contains("gluten") || a.Contains("grâu")))
        {
            calculatedRestrictions &= ~DietaryRestrictions.GlutenFree;
        }
        
        if (allAllergens.Any(a => a.Contains("nuci") || a.Contains("arahide") || 
                                  a.Contains("migdale") || a.Contains("cashew")))
        {
            calculatedRestrictions &= ~DietaryRestrictions.NutFree;
        }
        
        if (allAllergens.Any(a => a.Contains("pește") || a.Contains("moluște") || 
                                  a.Contains("crustacee")))
        {
            calculatedRestrictions &= ~DietaryRestrictions.NoSeafood;
        }

        if (allAllergens.Any(a => a.Contains("brânzeturi") || a.Contains("lapte") || 
                                  a.Contains("ouă")))
        {
            calculatedRestrictions &= ~DietaryRestrictions.DairyFree;
        }
        
        // Dacă nu avem item-uri, nu putem calcula restricții
        var finalRestrictions = menuItems.Any() 
            ? calculatedRestrictions 
            : DietaryRestrictions.None;
        
        logger.LogInformation("Menu restrictions: {Restrictions} (calculated from allergens: [{Allergens}])", 
            finalRestrictions, allAllergens.Count > 0 ? string.Join(", ", allAllergens) : "none");
        
        // create a menu cu restricții calculate automat
        Debug.Assert(request.ItemIds != null, "request.ItemIds != null");
        var menu = new Menu(Guid.NewGuid(), request.Name, 
            request.Price, request.ItemIds, request.Category, finalRestrictions, request.ImageUrl);
        context.Menu.Add(menu);
        await context.SaveChangesAsync();
        logger.LogInformation("Menu created with Name: {MenuName}", menu.Name);
        
        return Results.Created($"/menu/{menu.Name}", menu);
    }
    
}