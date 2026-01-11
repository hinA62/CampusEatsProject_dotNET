using CampusEats.Features.Menu;
using CampusEats.Features.Menu.Handlers;
using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusEats.Test.MenuTests.IntegrationTests;

public class UpdateItemHandlerTests : IDisposable
{
    private readonly CampusEatsContext _context;
    private readonly UpdateItemHandler _handler;
    private readonly ILogger<UpdateItemHandler> _logger = new LoggerFactory().CreateLogger<UpdateItemHandler>();

    public UpdateItemHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new CampusEatsContext(options);
        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<UpdateItemHandler>();
        _handler = new UpdateItemHandler(_context, _logger);
    }

    [Fact]
    public async Task Given_ValidInput_When_UpdateItem_Then_ShouldPass()
    {
        //Arrange
        var itemId = Guid.NewGuid();
        var existingItem = new MenuItem(itemId, "Old Pizza", 8.00m, null, ["gluten"]);
        await _context.MenuItem.AddAsync(existingItem);
        await _context.SaveChangesAsync();
        
        var request = new UpdateItemRequest(itemId, "New Pizza", 10.00m, null,["gluten"]);
        
        //Act
        var result = await _handler.Handle(request);
        
        //Assert
        result.Should().BeOfType<Ok<MenuItem>>();
        
        var updatedItem = await _context.MenuItem.FindAsync(itemId);
        updatedItem!.Name.Should().Be("New Pizza");
        updatedItem.Price.Should().Be(10.00m);
    }

    [Fact]
    public async Task Given_ValidationFailure_When_Handle_Then_ShouldReturnBadRequest()
    {
        //Arrange
        var request = new UpdateItemRequest(Guid.Empty, "a", -7.2m, null, null);
        
        //Act
        var result = await _handler.Handle(request);
        
        //Assert
        result.Should().BeOfType<BadRequest<List<FluentValidation.Results.ValidationFailure>>>();
    }
    
    [Fact]
    public async Task Given_NonExistentItemId_When_Handle_Then_ShouldReturnNotFound()
    {
        //Arrange
        var request = new UpdateItemRequest(Guid.NewGuid(), "Updated Name", 12.00m, null, null);
        
        //Act
        var result = await _handler.Handle(request);
        
        //Assert
        result.Should().BeOfType<NotFound<string>>();
    }

    public void Dispose()
    {
        _context?.Database?.EnsureDeleted();
        _context?.Dispose();
        
        GC.SuppressFinalize(this);
    }
}