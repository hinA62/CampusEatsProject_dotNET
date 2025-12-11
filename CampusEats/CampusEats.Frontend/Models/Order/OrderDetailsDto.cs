namespace CampusEatsFrontend.Models.Order;

public class OrderDetailsDto
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string ClientUsername { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public List<Guid> MenuIDs { get; set; } = new();
    public List<Guid> ItemIDs { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    
    // Details
    public List<OrderMenuItemDto> Menus { get; set; } = new();
    public List<OrderMenuItemDto> Items { get; set; } = new();
}

public class OrderMenuItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
