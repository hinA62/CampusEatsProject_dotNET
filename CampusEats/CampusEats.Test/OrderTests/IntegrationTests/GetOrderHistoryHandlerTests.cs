using CampusEats.Features.Order;
using CampusEats.Features.Order.Handlers;
using CampusEats.Features.Order.Requests;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusEats.Test.OrderTests.IntegrationTests;

public class GetOrderHistoryHandlerTests : IDisposable
{
    private readonly CampusEatsContext _context;
    private readonly GetOrderHistoryHandler _handler;
    private readonly ILogger<GetOrderHistoryHandler> _logger;
    
    public GetOrderHistoryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new CampusEatsContext(options);
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<GetOrderHistoryHandler>();
        _handler = new GetOrderHistoryHandler(_context, _logger);
    }
    
    [Fact]
    public async Task Given_ClientWithOrders_When_Handle_Then_ShouldReturnClientOrders()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var order1 = new Order(
            Guid.NewGuid(),
            clientId,
            50.00m,
            new List<Guid>(),
            new List<Guid>(),
            DateTime.UtcNow.AddDays(-2),
            OrderStatus.Completed
        );
        var order2 = new Order(
            Guid.NewGuid(),
            clientId,
            30.00m,
            new List<Guid>(),
            new List<Guid>(),
            DateTime.UtcNow.AddDays(-1),
            OrderStatus.Pending
        );
        var otherClientOrder = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            25.00m,
            new List<Guid>(),
            new List<Guid>(),
            DateTime.UtcNow,
            OrderStatus.Completed
        );
        
        await _context.Order.AddRangeAsync(order1, order2, otherClientOrder);
        await _context.SaveChangesAsync();
        
        var request = new GetOrderHistoryRequest(clientId);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<List<Order>>>(result);
        var okResult = result as Microsoft.AspNetCore.Http.HttpResults.Ok<List<Order>>;
        Assert.Equal(2, okResult!.Value!.Count);
        Assert.All(okResult.Value, o => Assert.Equal(clientId, o.ClientId));
    }
    
    [Fact]
    public async Task Given_ClientWithNoOrders_When_Handle_Then_ShouldReturnEmptyList()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var request = new GetOrderHistoryRequest(clientId);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<List<Order>>>(result);
        var okResult = result as Microsoft.AspNetCore.Http.HttpResults.Ok<List<Order>>;
        Assert.Empty(okResult!.Value!);
    }
    
    [Fact]
    public async Task Given_ClientWithOrders_When_Handle_Then_ShouldReturnOrdersSortedByDateDescending()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var oldOrder = new Order(
            Guid.NewGuid(),
            clientId,
            50.00m,
            new List<Guid>(),
            new List<Guid>(),
            DateTime.UtcNow.AddDays(-5),
            OrderStatus.Completed
        );
        var newOrder = new Order(
            Guid.NewGuid(),
            clientId,
            30.00m,
            new List<Guid>(),
            new List<Guid>(),
            DateTime.UtcNow.AddDays(-1),
            OrderStatus.Pending
        );
        
        await _context.Order.AddRangeAsync(oldOrder, newOrder);
        await _context.SaveChangesAsync();
        
        var request = new GetOrderHistoryRequest(clientId);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        var okResult = result as Microsoft.AspNetCore.Http.HttpResults.Ok<List<Order>>;
        Assert.Equal(newOrder.Id, okResult!.Value![0].Id);
        Assert.Equal(oldOrder.Id, okResult.Value[1].Id);
    }
    
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        
        GC.SuppressFinalize(this);
    }
}
