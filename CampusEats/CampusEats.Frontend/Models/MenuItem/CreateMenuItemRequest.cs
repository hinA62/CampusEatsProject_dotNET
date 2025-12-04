namespace CampusEatsFrontend.Models.MenuItem;

public class CreateMenuItemRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? Allergens { get; set; }
}
