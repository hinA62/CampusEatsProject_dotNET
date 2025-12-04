namespace CampusEatsFrontend.Models.Order;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public decimal Price { get; set; }
    public List<Guid> MenuIDs { get; set; } = new();
    public List<Guid> ItemIDs { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
}
