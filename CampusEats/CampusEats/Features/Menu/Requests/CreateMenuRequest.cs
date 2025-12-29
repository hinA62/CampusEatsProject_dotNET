namespace CampusEats.Features.Menu.Requests;

public record CreateMenuRequest(
    Guid Id,
    string Name, 
    decimal? Price, 
    List<Guid> ItemIds, 
    MenuCategory Category, 
    string? ImageUrl
);