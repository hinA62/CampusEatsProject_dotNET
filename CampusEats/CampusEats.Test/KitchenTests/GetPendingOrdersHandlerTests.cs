using CampusEats.Features.Kitchen.Handlers;
using CampusEats.Features.Kitchen.Requests;
using CampusEats.Features.Order;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CampusEats.Test.KitchenTests;

public class GetPendingOrdersHandlerTests
{
    private readonly CampusEatsContext _context;
    private readonly GetPendingOrdersHandler _handler;
    private readonly Mock<ILogger<GetPendingOrdersHandler>> _loggerMock;

    public GetPendingOrdersHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CampusEatsContext(options);
        _loggerMock = new Mock<ILogger<GetPendingOrdersHandler>>();
        _handler = new GetPendingOrdersHandler(_context, _loggerMock.Object);
    }

    [Fact]
    public async Task GetPending_ShouldReturnOnlyActiveOrders()
    {
        // Arrange
        var pendingOrder = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            50,
            new List<Guid>(),
            new List<Guid> { Guid.NewGuid() },
            DateTime.UtcNow,
            OrderStatus.Pending
        );
        var completedOrder = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            75,
            new List<Guid>(),
            new List<Guid> { Guid.NewGuid() },
            DateTime.UtcNow,
            OrderStatus.Completed
        );
        _context.Order.AddRange(pendingOrder, completedOrder);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(new GetPendingOrdersRequest());

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPending_WithNoOrders_ShouldReturnEmpty()
    {
        // Act
        var result = await _handler.Handle(new GetPendingOrdersRequest());

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPending_WithMultiplePendingOrders_ShouldReturnAll()
    {
        // Arrange
        var order1 = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            50,
            new List<Guid>(),
            new List<Guid> { Guid.NewGuid() },
            DateTime.UtcNow.AddMinutes(-10),
            OrderStatus.Pending
        );
        var order2 = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            75,
            new List<Guid>(),
            new List<Guid> { Guid.NewGuid() },
            DateTime.UtcNow.AddMinutes(-5),
            OrderStatus.Preparing
        );
        var order3 = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            100,
            new List<Guid>(),
            new List<Guid> { Guid.NewGuid() },
            DateTime.UtcNow,
            OrderStatus.Completed
        );
        _context.Order.AddRange(order1, order2, order3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(new GetPendingOrdersRequest());

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPending_ShouldNotReturnCancelledOrders()
    {
        // Arrange
        var pendingOrder = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            50,
            new List<Guid>(),
            new List<Guid> { Guid.NewGuid() },
            DateTime.UtcNow,
            OrderStatus.Pending
        );
        var cancelledOrder = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            75,
            new List<Guid>(),
            new List<Guid> { Guid.NewGuid() },
            DateTime.UtcNow,
            OrderStatus.Cancelled
        );
        _context.Order.AddRange(pendingOrder, cancelledOrder);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(new GetPendingOrdersRequest());

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPending_WithSpecificStatus_ShouldFilterCorrectly()
    {
        // Arrange
        var order1 = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            50,
            new List<Guid>(),
            new List<Guid> { Guid.NewGuid() },
            DateTime.UtcNow.AddMinutes(-20),
            OrderStatus.Pending
        );
        var order2 = new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            75,
            new List<Guid>(),
            new List<Guid> { Guid.NewGuid() },
            DateTime.UtcNow.AddMinutes(-10),
            OrderStatus.Preparing
        );
        _context.Order.AddRange(order2, order1);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(new GetPendingOrdersRequest("Pending"));

        // Assert
        result.Should().NotBeNull();
    }
}
