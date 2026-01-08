using CampusEats.Features.Inventory;
using CampusEats.Features.Inventory.Handlers;
using CampusEats.Features.Menu;
using CampusEats.Features.Order;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Test.InventoryTests;

public class InventoryHandlerTests
{
    private readonly CampusEatsContext _context;
    private readonly InventoryHandler _handler;

    public InventoryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new CampusEatsContext(options);
        _handler = new InventoryHandler(_context);
    }

    [Fact]
    public async Task GetAsync_ExistingDate_ReturnsInventory()
    {
        var date = new DateOnly(2026, 1, 8);
        var inventory = new InventoryDay
        {
            Date = date,
            GeneratedAtUtc = DateTime.UtcNow,
            Items = new List<InventoryDayItem>
            {
                new() { Date = date, ItemId = Guid.NewGuid(), Name = "Item1", UnitPrice = 10, Count = 5 }
            }
        };
        _context.InventoryDay.Add(inventory);
        await _context.SaveChangesAsync();

        var result = await _handler.GetAsync(date);

        result.Should().NotBeNull();
        result!.Date.Should().Be(date);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAsync_NonExistingDate_ReturnsNull()
    {
        var date = new DateOnly(2026, 1, 15);

        var result = await _handler.GetAsync(date);

        result.Should().BeNull();
    }

    [Fact]
    public async Task RebuildAsync_CreatesNewInventory()
    {
        var date = new DateOnly(2026, 1, 8);
        var itemId = Guid.NewGuid();
        var item = new MenuItem(itemId, "Burger", 12m, null, null);
        _context.MenuItem.Add(item);

        var order = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            12m,
            new List<Guid>(),
            new List<Guid> { itemId },
            date.ToDateTime(new TimeOnly(12, 0), DateTimeKind.Utc),
            OrderStatus.Completed
        );
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        var result = await _handler.RebuildAsync(date);

        result.Should().NotBeNull();
        result.Date.Should().Be(date);
        result.Items.Should().HaveCount(1);
        result.Items.First().Count.Should().Be(1);
    }

    [Fact]
    public async Task RebuildAsync_UpdatesExistingInventory()
    {
        var date = new DateOnly(2026, 1, 8);
        var itemId = Guid.NewGuid();
        var item = new MenuItem(itemId, "Pizza", 15m, null, null);
        _context.MenuItem.Add(item);
        await _context.SaveChangesAsync();

        var order = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            15m,
            new List<Guid>(),
            new List<Guid> { itemId },
            date.ToDateTime(new TimeOnly(14, 0), DateTimeKind.Utc),
            OrderStatus.Completed
        );
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        var result = await _handler.RebuildAsync(date);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Count.Should().Be(1);
    }

    [Fact]
    public async Task RebuildAsync_ExcludesCancelledOrders()
    {
        var date = new DateOnly(2026, 1, 8);
        var itemId = Guid.NewGuid();
        var item = new MenuItem(itemId, "Salad", 8m, null, null);
        _context.MenuItem.Add(item);

        var cancelledOrder = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            8m,
            new List<Guid>(),
            new List<Guid> { itemId },
            date.ToDateTime(new TimeOnly(12, 0), DateTimeKind.Utc),
            OrderStatus.Cancelled
        );
        _context.Order.Add(cancelledOrder);
        await _context.SaveChangesAsync();

        var result = await _handler.RebuildAsync(date);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task RebuildAsync_AggregatesMultipleOrders()
    {
        var date = new DateOnly(2026, 1, 8);
        var itemId = Guid.NewGuid();
        var item = new MenuItem(itemId, "Fries", 5m, null, null);
        _context.MenuItem.Add(item);

        var order1 = new Order(Guid.NewGuid(), Guid.NewGuid(), 5m, new List<Guid>(), new List<Guid> { itemId }, date.ToDateTime(new TimeOnly(11, 0), DateTimeKind.Utc), OrderStatus.Completed);
        var order2 = new Order(Guid.NewGuid(), Guid.NewGuid(), 5m, new List<Guid>(), new List<Guid> { itemId }, date.ToDateTime(new TimeOnly(13, 0), DateTimeKind.Utc), OrderStatus.Pending);
        var order3 = new Order(Guid.NewGuid(), Guid.NewGuid(), 5m, new List<Guid>(), new List<Guid> { itemId }, date.ToDateTime(new TimeOnly(15, 0), DateTimeKind.Utc), OrderStatus.Completed);
        _context.Order.AddRange(order1, order2, order3);
        await _context.SaveChangesAsync();

        var result = await _handler.RebuildAsync(date);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Count.Should().Be(3);
    }
}
