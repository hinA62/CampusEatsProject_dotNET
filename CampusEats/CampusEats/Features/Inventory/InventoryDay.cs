namespace CampusEats.Features.Inventory;

public class InventoryDay
{
    public DateOnly Date { get; set; }                  // PK (ex: 2025-11-05)
    public DateTime GeneratedAtUtc { get; set; }        // când ai calculat snapshot-ul
    public ICollection<InventoryDayItem> Items 
    { get; set; } = new List<InventoryDayItem>();
}