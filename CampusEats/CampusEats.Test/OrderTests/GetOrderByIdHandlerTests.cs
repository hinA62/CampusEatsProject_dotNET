using CampusEats.Features.Order;
using CampusEats.Features.Order.Requests;
using CampusEats.Features.Order.Handlers;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusEats.Test.OrderTests;

public class GetOrderByIdHandlerTests : IDisposable
{
    private readonly CampusEatsContext _context;
    private readonly GetOrderByIdHandler _handler;
    private readonly ILogger<GetOrderByIdHandler> _logger;
    
    public GetOrderByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new CampusEatsContext(options);
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<GetOrderByIdHandler>();
        _handler = new GetOrderByIdHandler(_context, _logger);
    }
    
    [Fact]
    public async Task Given_ExistingOrder_When_Handle_Then_ShouldReturnOrder()
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
        
        var request = new GetOrderByIdRequest(order.Id);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<Order>>(result);
        var okResult = result as Microsoft.AspNetCore.Http.HttpResults.Ok<Order>;
        Assert.Equal(order.Id, okResult!.Value!.Id);
    }
    
    [Fact]
    public async Task Given_NonExistentOrder_When_Handle_Then_ShouldReturnNotFound()
    {
        // Arrange
        var request = new GetOrderByIdRequest(Guid.NewGuid());
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>(result);
    }
    
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
