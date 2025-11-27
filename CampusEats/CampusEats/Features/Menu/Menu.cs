using System.Text.Json.Serialization;

namespace CampusEats.Features.Menu;

public record Menu(
    Guid Id,
    string Name, 
    decimal? Price, 
    [property: JsonPropertyName("ItemId")]
    List<Guid> ItemId, 
    MenuCategory Category, 
    DietaryRestrictions Restrictions
    );