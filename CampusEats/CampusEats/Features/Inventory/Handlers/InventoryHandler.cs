using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Features.Inventory.Handlers;

public class InventoryHandler(CampusEatsContext ctx, ILogger<InventoryHandler> log)
{
    private readonly ILogger<InventoryHandler> _log = log;

    public async Task<InventoryDay> RebuildAsync(DateOnly date)
    {
        var start = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var end   = date.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);


        var orders = await ctx.Order
            .Where(o => o.CreatedAt >= start 
                        && o.CreatedAt <= end 
                        && o.Status != Features.Order.OrderStatus.Cancelled)
            .AsNoTracking()
            .ToListAsync();


        var allItemIds = orders.SelectMany(o => o.ItemIDs).ToList();


        var menuIds = orders.SelectMany(o => o.MenuIDs).Distinct().ToList();
        var menus = await ctx.Menu
            .Where(m => menuIds.Contains(m.Id))
            .Select(m => new { m.ItemId })
            .ToListAsync();

        foreach (var m in menus)
            allItemIds.AddRange(m.ItemId);


        var counts = allItemIds.GroupBy(x => x)
            .ToDictionary(g => g.Key, g => g.Count());


        var itemIds = counts.Keys.ToList();
        var meta = await ctx.MenuItem
            .Where(i => itemIds.Contains(i.Id))
            .Select(i => new { i.Id, i.Name, i.Price })
            .ToListAsync();


        var day = await ctx.InventoryDay
            .Include(d => d.Items)
            .FirstOrDefaultAsync(d => d.Date == date);
        if (day is null)
        {
            day = new InventoryDay { Date = date, GeneratedAtUtc = DateTime.UtcNow };
            ctx.InventoryDay.Add(day);
        }
        else
        {
            ctx.InventoryDayItems.RemoveRange(day.Items);
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
                Count = counts.GetValueOrDefault(m.Id, 0)
            });
        }

        await ctx.SaveChangesAsync();
        return day;
    }

    public async Task<InventoryDay?> GetAsync(DateOnly date)
        => await ctx.InventoryDay
            .Include(d => d.Items).AsNoTracking()
            .FirstOrDefaultAsync(d => d.Date == date);
}
