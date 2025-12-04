namespace CampusEatsFrontend.Models.Order;

public class PlaceOrderRequest
{
    public Guid ClientId { get; set; }
    public List<Guid> MenuIDs { get; set; } = new();
    public List<Guid> ItemIDs { get; set; } = new();
}
