namespace CampusEatsFrontend.Models.Menu;

public class CreateMenuRequest
{
    public string Name { get; set; } = null!;
    public decimal? Price { get; set; }
    public List<Guid>? ItemIds { get; set; } = [];
    public MenuCategory Category { get; set; }
    public DietaryRestrictions Restrictions { get; set; }
}