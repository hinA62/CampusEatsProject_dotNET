using CampusEats.Features.Menu;
using CampusEats.Features.Order;
using CampusEats.Features.Order.Handlers;
using CampusEats.Features.Order.Requests;
using CampusEats.Features.User;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusEats.Test.OrderTests.IntegrationTests;

public class PlaceOrderHandlerTests : IDisposable
{
    private readonly CampusEatsContext _context;
    private readonly PlaceOrderHandler _handler;

    public PlaceOrderHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new CampusEatsContext(options);
        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<PlaceOrderHandler>();
        _handler = new PlaceOrderHandler(_context, logger);
    }

    [Fact]
    public async Task Given_ValidRequest_When_Handle_Then_ShouldCreateOrder()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var user = new User { Id = clientId, Email = "client@mail.com", Username = "Client", PasswordHash = "hashedpw", Role = UserRole.Client, CreatedAt = DateTime.UtcNow };
        await _context.Users.AddAsync(user);
        
        var menuId = Guid.NewGuid();
        var menu = new Menu(menuId, "Lunch Special", 15.99m, [], MenuCategory.Meat, DietaryRestrictions.None, null);
        await _context.Menu.AddAsync(menu);
        
        var itemId = Guid.NewGuid();
        var item = new MenuItem(itemId, "Extra Cheese", 2.50m, null, null);
        await _context.MenuItem.AddAsync(item);
        
        await _context.SaveChangesAsync();
        
        var request = new PlaceOrderRequest(clientId, [menuId], [itemId]);
        
        // Act
        var result = await _handler.Handle(request);
        var order = await _context.Order.FirstOrDefaultAsync();
        
        // Assert
        result.Should().NotBeNull();
        
        order.Should().NotBeNull();
        order.ClientId.Should().Be(clientId);
        order.MenuIDs.Should().ContainSingle().Which.Should().Be(menuId);
        order.ItemIDs.Should().ContainSingle().Which.Should().Be(itemId);
        order.Price.Should().Be(18.49m); // 15.99 + 2.50
        order.Status.Should().Be(OrderStatus.Pending);
    }

    [Fact]
    public async Task Given_FailedValidation_When_Handle_Then_ShouldReturnBadRequest()
    {
        //Arrange
        var request = new PlaceOrderRequest(Guid.Empty, [], []); // Empty clientId should fail validation
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults
            .BadRequest<List<FluentValidation.Results.ValidationFailure>>>();
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
        var request = new PlaceOrderRequest(clientId, [nonExistentMenuId], new List<Guid>());
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        var statusCodeResult = result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(400);

        var valueResult = result.Should().BeAssignableTo<IValueHttpResult>().Subject;
        valueResult.Value.Should().NotBeNull();
        
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
        var request = new PlaceOrderRequest(clientId, [], [nonExistentItemId]);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        var statusCodeResult = result.Should().BeAssignableTo<IStatusCodeHttpResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(400);

        var valueResult = result.Should().BeAssignableTo<IValueHttpResult>().Subject;
        valueResult.Value.Should().NotBeNull();
        
        var orders = await _context.Order.ToListAsync();
        orders.Should().BeEmpty();
    }

    [Fact]
    public async Task Given_DuplicateIds_When_Handle_Then_ShouldKeepDuplicatesForQuantity()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var user = new User { Id = clientId, Email = "client@mail.com", Username = "Client", PasswordHash = "hashedpw", Role = UserRole.Client, CreatedAt = DateTime.UtcNow };
        await _context.Users.AddAsync(user);
        
        var menuId = Guid.NewGuid();
        var menu = new Menu(menuId, "Burger", 10.00m, [], MenuCategory.Meat, DietaryRestrictions.None, null);
        await _context.Menu.AddAsync(menu);
        
        await _context.SaveChangesAsync();
        
        // Request with duplicate menuIds (quantity = 3)
        var request = new PlaceOrderRequest(clientId, [menuId, menuId, menuId], new List<Guid>());
        
        // Act
        var result = await _handler.Handle(request);
        var order = await _context.Order.FirstOrDefaultAsync();
        
        // Assert
        result.Should().NotBeNull();
        
        order.Should().NotBeNull();
        order.MenuIDs.Should().HaveCount(3); // Keep duplicates for quantity
        order.MenuIDs.Should().OnlyContain(id => id == menuId);
        order.Price.Should().Be(30.00m); // 3 x 10.00 = 30.00
    }

    [Fact]
    public async Task Given_MultipleMenusAndItems_When_Handle_Then_ShouldCalculateTotalCorrectly()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var user = new User { Id = clientId, Email = "client@mail.com", Username = "Client", PasswordHash = "hashedpw", Role = UserRole.Client, CreatedAt = DateTime.UtcNow };
        await _context.Users.AddAsync(user);
        
        var menu1Id = Guid.NewGuid();
        var menu1 = new Menu(menu1Id, "Burger", 12.50m, [], MenuCategory.Meat, DietaryRestrictions.None, null);
        var menu2Id = Guid.NewGuid();
        var menu2 = new Menu(menu2Id, "Fries", 5.00m, [], MenuCategory.Vegetarian, DietaryRestrictions.None, null);
        
        var item1Id = Guid.NewGuid();
        var item1 = new MenuItem(item1Id, "Cheese", 1.50m, null, null);
        var item2Id = Guid.NewGuid();
        var item2 = new MenuItem(item2Id, "Bacon", 2.00m, null, null);
        
        await _context.Menu.AddRangeAsync(menu1, menu2);
        await _context.MenuItem.AddRangeAsync(item1, item2);
        await _context.SaveChangesAsync();
        
        var request = new PlaceOrderRequest(clientId, [menu1Id, menu2Id], [item1Id, item2Id]);
        
        // Act
        var result = await _handler.Handle(request);
        var order = await _context.Order.FirstOrDefaultAsync();
        
        // Assert
        result.Should().NotBeNull();
        
        order.Should().NotBeNull();
        order.Price.Should().Be(21.00m); // 12.50 + 5.00 + 1.50 + 2.00
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
