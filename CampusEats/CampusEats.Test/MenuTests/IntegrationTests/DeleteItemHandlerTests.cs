using CampusEats.Features.Menu;
using CampusEats.Features.Menu.Handlers;
using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusEats.Test.MenuTests.IntegrationTests;

public class DeleteItemHandlerTests : IDisposable
{
    private readonly CampusEatsContext _context;
    private readonly DeleteItemHandler _handler;
    private readonly ILogger<DeleteItemHandler> _logger = new LoggerFactory().CreateLogger<DeleteItemHandler>();
    
    public DeleteItemHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new CampusEatsContext(options);
        LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<DeleteItemHandler>();
        _handler = new DeleteItemHandler(_context, _logger);
    }
    
    [Fact]
    public async Task Given_ValidDeleteItemRequest_When_Handle_Then_ShouldDeleteItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var menuItem = new MenuItem(itemId, "Burger", 8.99m, null, null);
        await _context.MenuItem.AddAsync(menuItem);
        
        var menuId = Guid.NewGuid();
        var menu = new Menu(menuId, "Lunch Menu", 0m, [itemId], 
            MenuCategory.Meat, DietaryRestrictions.None, null);
        await _context.Menu.AddAsync(menu);
        
        await _context.SaveChangesAsync();
        
        var request = new DeleteItemRequest(itemId);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        var noContentResult = result.Should()
            .BeOfType<Microsoft.AspNetCore.Http.HttpResults.NoContent>().Subject;
        
        var deletedItem = await _context.MenuItem.FindAsync(itemId);
        deletedItem.Should().BeNull();
        
        var updatedMenu = await _context.Menu.FindAsync(menuId);
        updatedMenu?.ItemId.Should().NotContain(itemId);
    }

    [Fact]
    public async Task Given_NullMenuItem_When_Handle_Then_ShouldReturnNotFound()
    {
        //Arrange
        var request = new DeleteItemRequest(Guid.NewGuid());
        
        //Act
        var result = await _handler.Handle(request);
        
        //Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>(result);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}