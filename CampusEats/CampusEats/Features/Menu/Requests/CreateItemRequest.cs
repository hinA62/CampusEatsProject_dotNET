namespace CampusEats.Features.Menu.Requests;

public record CreateItemRequest(
    string Name, 
    decimal? Price, 
    string? ImageUrl, 
    List<string>? Allergens
);