namespace CampusEatsFrontend.Models.Menu;

public class UpdateMenuRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public List<Guid> ItemIds { get; set; } = new();
    public MenuCategory Category { get; set; }
    public DietaryRestrictions Restrictions { get; set; }
    public string? ImageUrl { get; set; }
}
