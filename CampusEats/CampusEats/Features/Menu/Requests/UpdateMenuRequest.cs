namespace CampusEats.Features.Menu.Requests;
public record UpdateMenuRequest(
    Guid Id,
    string Name,
    decimal Price,
    List<Guid> ItemIds,
    MenuCategory Category,
    DietaryRestrictions Restrictions,
    string? ImageUrl
);
