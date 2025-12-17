namespace CampusEatsFrontend.Models.Cart;

public class CartItem
{
    public Guid? MenuId { get; set; }
    public Guid? MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Subtotal => (Price ?? 0) * Quantity;
    public bool IsMenuItem => MenuItemId.HasValue;
}
