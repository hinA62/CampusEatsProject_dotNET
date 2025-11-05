using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Features.Inventory;

public class InventoryService
{
    private readonly CampusEatsContext _ctx;
    private readonly ILogger<InventoryService> _log;

    public InventoryService(CampusEatsContext ctx, ILogger<InventoryService> log)
    {
        _ctx = ctx; _log = log;
    }

    public async Task<InventoryDay> RebuildAsync(DateOnly date)
    {
        var start = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var end   = date.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        // ia toate comenzile non-cancelate din zi
        var orders = await _ctx.Order
            .Where(o => o.CreatedAt >= start && o.CreatedAt <= end && o.Status != Features.Order.OrderStatus.Cancelled)
            .AsNoTracking()
            .ToListAsync();

        // colectează itemele directe din comenzi
        var allItemIds = orders.SelectMany(o => o.ItemIDs).ToList();

        // extinde meniurile la itemele lor
        var menuIds = orders.SelectMany(o => o.MenuIDs).Distinct().ToList();
        var menus = await _ctx.Menu
            .Where(m => menuIds.Contains(m.Id))
            .Select(m => new { m.ItemId }) // List<Guid>
            .ToListAsync();

        foreach (var m in menus)
            if (m.ItemId is not null) allItemIds.AddRange(m.ItemId);

        // agregă
        var counts = allItemIds.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

        // metadate iteme
        var itemIds = counts.Keys.ToList();
        var meta = await _ctx.MenuItem
            .Where(i => itemIds.Contains(i.Id))
            .Select(i => new { i.Id, i.Name, i.Price })
            .ToListAsync();

        // upsert InventoryDay
        var day = await _ctx.InventoryDay.Include(d => d.Items).FirstOrDefaultAsync(d => d.Date == date);
        if (day is null)
        {
            day = new InventoryDay { Date = date, GeneratedAtUtc = DateTime.UtcNow };
            _ctx.InventoryDay.Add(day);
        }
        else
        {
            _ctx.InventoryDayItems.RemoveRange(day.Items);
            day.Items.Clear();
            day.GeneratedAtUtc = DateTime.UtcNow;
        }

        foreach (var m in meta)
        {
            day.Items.Add(new InventoryDayItem
            {
                Date = date,
                ItemId = m.Id,
                Name = m.Name,
                UnitPrice = m.Price,
                Count = counts.TryGetValue(m.Id, out var c) ? c : 0
            });
        }

        await _ctx.SaveChangesAsync();
        return day;
    }

    public async Task<InventoryDay?> GetAsync(DateOnly date)
        => await _ctx.InventoryDay.Include(d => d.Items).AsNoTracking().FirstOrDefaultAsync(d => d.Date == date);
}
