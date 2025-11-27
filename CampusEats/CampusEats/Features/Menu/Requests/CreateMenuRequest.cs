namespace CampusEats.Features.Menu.Requests;

public record CreateMenuRequest(
    string Name, 
    decimal? Price, 
    List<Guid>? ItemIds, 
    MenuCategory Category, 
    DietaryRestrictions Restrictions
    );