using System.Text.Json.Serialization;

namespace CampusEats.Features.Menu;

public record Menu(
    Guid Id,
    string Name, 
    decimal? Price, 
    [property: JsonPropertyName("ItemIds")]
    List<Guid> ItemId, 
    MenuCategory Category, 
    DietaryRestrictions Restrictions
    );