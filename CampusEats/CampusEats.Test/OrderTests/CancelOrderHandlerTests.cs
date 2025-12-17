using CampusEats.Features.Order;
using CampusEats.Features.Order.Requests;
using CampusEats.Features.Order.Handlers;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusEats.Test.OrderTests;

public class CancelOrderHandlerTests : IDisposable
{
    private readonly CampusEatsContext _context;
    private readonly CancelOrderHandler _handler;
    private readonly ILogger<CancelOrderHandler> _logger;
    
    public CancelOrderHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new CampusEatsContext(options);
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CancelOrderHandler>();
        _handler = new CancelOrderHandler(_context, _logger);
    }
    
    [Fact]
    public async Task Given_PendingOrder_When_Handle_Then_ShouldCancelOrder()
    {
        // Arrange
        var order = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            50.00m,
            new List<Guid>(),
            new List<Guid>(),
            DateTime.UtcNow,
            OrderStatus.Pending
        );
        await _context.Order.AddAsync(order);
        await _context.SaveChangesAsync();
        
        var request = new CancelOrderRequest(order.Id);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        var updatedOrder = await _context.Order.FindAsync(order.Id);
        Assert.Equal(OrderStatus.Cancelled, updatedOrder!.Status);
    }
    
    [Fact]
    public async Task Given_ConfirmedOrder_When_Handle_Then_ShouldReturnConflict()
    {
        // Arrange
        var order = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            50.00m,
            new List<Guid>(),
            new List<Guid>(),
            DateTime.UtcNow,
            OrderStatus.Confirmed
        );
        await _context.Order.AddAsync(order);
        await _context.SaveChangesAsync();
        
        var request = new CancelOrderRequest(order.Id);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Conflict<string>>(result);
    }
    
    [Fact]
    public async Task Given_PreparingOrder_When_Handle_Then_ShouldReturnConflict()
    {
        // Arrange
        var order = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            50.00m,
            new List<Guid>(),
            new List<Guid>(),
            DateTime.UtcNow,
            OrderStatus.Preparing
        );
        await _context.Order.AddAsync(order);
        await _context.SaveChangesAsync();
        
        var request = new CancelOrderRequest(order.Id);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Conflict<string>>(result);
    }
    
    [Fact]
    public async Task Given_CompletedOrder_When_Handle_Then_ShouldReturnConflict()
    {
        // Arrange
        var order = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            50.00m,
            new List<Guid>(),
            new List<Guid>(),
            DateTime.UtcNow,
            OrderStatus.Completed
        );
        await _context.Order.AddAsync(order);
        await _context.SaveChangesAsync();
        
        var request = new CancelOrderRequest(order.Id);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Conflict<string>>(result);
    }
    
    [Fact]
    public async Task Given_NonExistentOrder_When_Handle_Then_ShouldReturnNotFound()
    {
        // Arrange
        var request = new CancelOrderRequest(Guid.NewGuid());
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>(result);
    }
    
    [Fact]
    public async Task Given_AlreadyCancelledOrder_When_Handle_Then_ShouldReturnConflict()
    {
        // Arrange
        var order = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            50.00m,
            new List<Guid>(),
            new List<Guid>(),
            DateTime.UtcNow,
            OrderStatus.Cancelled
        );
        await _context.Order.AddAsync(order);
        await _context.SaveChangesAsync();
        
        var request = new CancelOrderRequest(order.Id);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Conflict<string>>(result);
    }
    
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
