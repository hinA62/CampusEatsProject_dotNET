namespace CampusEats.Features.Menu.Requests;

public record UpdateItemRequest(
    Guid Id, 
    string Name, 
    decimal Price, 
    string? ImageUrl, 
    List<string>? Allergens
);