namespace CampusEats.Features.Menu.Requests;
// Restrictions nu mai sunt în request - se deduc automat din alergenii items-urilor
public record UpdateMenuRequest(
    Guid Id,
    string Name,
    decimal Price,
    List<Guid>? ItemIds,
    MenuCategory Category
);
