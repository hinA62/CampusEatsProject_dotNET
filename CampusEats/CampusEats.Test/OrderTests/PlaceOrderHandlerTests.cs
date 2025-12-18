using CampusEats.Features.Order;
using CampusEats.Features.Order.Handlers;
using CampusEats.Features.Order.Requests;
using CampusEats.Features.User;
using CampusEats.Features.Menu;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;

namespace CampusEats.Test.OrderTests;

public class PlaceOrderHandlerTests : IDisposable
{
    private readonly CampusEatsContext _context;
    private readonly PlaceOrderHandler _handler;
    private readonly ILogger<PlaceOrderHandler> _logger;

    public PlaceOrderHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new CampusEatsContext(options);
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<PlaceOrderHandler>();
        _handler = new PlaceOrderHandler(_context, _logger);
    }

    [Fact]
    public async Task Given_ValidRequest_When_Handle_Then_ShouldCreateOrder()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var user = new User { Id = clientId, Email = "client@mail.com", Username = "Client", PasswordHash = "hashedpw", Role = UserRole.Client, CreatedAt = DateTime.UtcNow };
        await _context.Users.AddAsync(user);
        
        var menuId = Guid.NewGuid();
        var menu = new Menu(menuId, "Lunch Special", 15.99m, new List<Guid>(), MenuCategory.Meat, DietaryRestrictions.None, null);
        await _context.Menu.AddAsync(menu);
        
        var itemId = Guid.NewGuid();
        var item = new MenuItem(itemId, "Extra Cheese", 2.50m, null, null);
        await _context.MenuItem.AddAsync(item);
        
        await _context.SaveChangesAsync();
        
        var request = new PlaceOrderRequest(clientId, new List<Guid> { menuId }, new List<Guid> { itemId });
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        var order = await _context.Order.FirstOrDefaultAsync();
        order.Should().NotBeNull();
        order!.ClientId.Should().Be(clientId);
        order.MenuIDs.Should().ContainSingle().Which.Should().Be(menuId);
        order.ItemIDs.Should().ContainSingle().Which.Should().Be(itemId);
        order.Price.Should().Be(18.49m); // 15.99 + 2.50
        order.Status.Should().Be(OrderStatus.Pending);
    }

    [Fact]
    public async Task Given_MissingMenuIds_When_Handle_Then_ShouldReturnBadRequest()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var user = new User { Id = clientId, Email = "client@mail.com", Username = "Client", PasswordHash = "hashedpw", Role = UserRole.Client, CreatedAt = DateTime.UtcNow };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        var nonExistentMenuId = Guid.NewGuid();
        var request = new PlaceOrderRequest(clientId, new List<Guid> { nonExistentMenuId }, new List<Guid>());
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        var orders = await _context.Order.ToListAsync();
        orders.Should().BeEmpty();
    }

    [Fact]
    public async Task Given_MissingItemIds_When_Handle_Then_ShouldReturnBadRequest()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var user = new User { Id = clientId, Email = "client@mail.com", Username = "Client", PasswordHash = "hashedpw", Role = UserRole.Client, CreatedAt = DateTime.UtcNow };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        var nonExistentItemId = Guid.NewGuid();
        var request = new PlaceOrderRequest(clientId, new List<Guid>(), new List<Guid> { nonExistentItemId });
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        var orders = await _context.Order.ToListAsync();
        orders.Should().BeEmpty();
    }

    [Fact]
    public async Task Given_DuplicateIds_When_Handle_Then_ShouldDeduplicateAndCalculateCorrectly()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var user = new User { Id = clientId, Email = "client@mail.com", Username = "Client", PasswordHash = "hashedpw", Role = UserRole.Client, CreatedAt = DateTime.UtcNow };
        await _context.Users.AddAsync(user);
        
        var menuId = Guid.NewGuid();
        var menu = new Menu(menuId, "Burger", 10.00m, new List<Guid>(), MenuCategory.Meat, DietaryRestrictions.None, null);
        await _context.Menu.AddAsync(menu);
        
        await _context.SaveChangesAsync();
        
        // Request with duplicate menuIds
        var request = new PlaceOrderRequest(clientId, new List<Guid> { menuId, menuId, menuId }, new List<Guid>());
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        var order = await _context.Order.FirstOrDefaultAsync();
        order.Should().NotBeNull();
        order!.MenuIDs.Should().ContainSingle().Which.Should().Be(menuId);
        order.Price.Should().Be(10.00m); // Should only count once
    }

    [Fact]
    public async Task Given_MultipleMenusAndItems_When_Handle_Then_ShouldCalculateTotalCorrectly()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var user = new User { Id = clientId, Email = "client@mail.com", Username = "Client", PasswordHash = "hashedpw", Role = UserRole.Client, CreatedAt = DateTime.UtcNow };
        await _context.Users.AddAsync(user);
        
        var menu1Id = Guid.NewGuid();
        var menu1 = new Menu(menu1Id, "Burger", 12.50m, new List<Guid>(), MenuCategory.Meat, DietaryRestrictions.None, null);
        var menu2Id = Guid.NewGuid();
        var menu2 = new Menu(menu2Id, "Fries", 5.00m, new List<Guid>(), MenuCategory.Vegetarian, DietaryRestrictions.None, null);
        
        var item1Id = Guid.NewGuid();
        var item1 = new MenuItem(item1Id, "Cheese", 1.50m, null, null);
        var item2Id = Guid.NewGuid();
        var item2 = new MenuItem(item2Id, "Bacon", 2.00m, null, null);
        
        await _context.Menu.AddRangeAsync(menu1, menu2);
        await _context.MenuItem.AddRangeAsync(item1, item2);
        await _context.SaveChangesAsync();
        
        var request = new PlaceOrderRequest(clientId, new List<Guid> { menu1Id, menu2Id }, new List<Guid> { item1Id, item2Id });
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        var order = await _context.Order.FirstOrDefaultAsync();
        order.Should().NotBeNull();
        order!.Price.Should().Be(21.00m); // 12.50 + 5.00 + 1.50 + 2.00
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
