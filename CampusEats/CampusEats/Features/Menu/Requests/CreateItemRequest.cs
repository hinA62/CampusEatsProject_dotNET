namespace CampusEats.Features.Menu.Requests;

public record CreateItemRequest(Guid Id, string Name, decimal Price, string ImageUrl, List<string> Allergens);