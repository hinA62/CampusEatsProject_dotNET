namespace CampusEats.Features.Inventory;

public class InventoryDayItem
{
    public Guid Id { get; set; } = Guid.NewGuid();      // PK
    public DateOnly Date { get; set; }                  // FK -> InventoryDay.Date
    public Guid ItemId { get; set; }                    // referință la MenuItems.Id (nu FK hard dacă nu vrei)
    public string Name { get; set; } = string.Empty;    // snapshot (nume la momentul raportului)
    public decimal? UnitPrice { get; set; }              // snapshot (preț la momentul raportului)
    public int Count { get; set; }                      // câte porții s-au consumat în ziua respectivă
}
