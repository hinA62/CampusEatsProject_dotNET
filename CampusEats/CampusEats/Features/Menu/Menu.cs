namespace CampusEats.Features.Menu;

public record Menu(
    Guid Id,
    string Name, 
    decimal Price, 
    List<Guid> ItemId, 
    MenuCategory Category, 
    DietaryRestrictions Restrictions
    );