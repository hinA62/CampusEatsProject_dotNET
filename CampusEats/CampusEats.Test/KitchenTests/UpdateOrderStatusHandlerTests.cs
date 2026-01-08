using CampusEats.Features.Kitchen.Handlers;
using CampusEats.Features.Kitchen.Requests;
using CampusEats.Features.Order;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CampusEats.Test.KitchenTests;

public class UpdateOrderStatusHandlerTests
{
    private readonly CampusEatsContext _context;
    private readonly UpdateOrderStatusHandler _handler;

    public UpdateOrderStatusHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new CampusEatsContext(options);

        var logger = new Mock<ILogger<UpdateOrderStatusHandler>>();
        _handler = new UpdateOrderStatusHandler(_context, logger.Object);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ReturnsNotFound()
    {
        var request = new UpdateOrderStatusRequest(Guid.NewGuid(), OrderStatus.Confirmed);

        var result = await _handler.Handle(request);

        result.Should().NotBeNull();
        var resultType = result.GetType().Name;
        resultType.Should().Contain("NotFound");
    }

    [Fact]
    public async Task Handle_ValidTransition_PendingToConfirmed_UpdatesStatus()
    {
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var order = new Order(
            orderId,
            clientId,
            100.00m,
            [],
            [],
            DateTime.UtcNow,
            OrderStatus.Pending
        );
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        var request = new UpdateOrderStatusRequest(orderId, OrderStatus.Confirmed);
        var result = await _handler.Handle(request);

        var updatedOrder = await _context.Order.FindAsync(orderId);
        updatedOrder.Should().NotBeNull();
        updatedOrder!.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task Handle_ValidTransition_ConfirmedToPreparing_UpdatesStatus()
    {
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var order = new Order(
            orderId,
            clientId,
            100.00m,
            [],
            [],
            DateTime.UtcNow,
            OrderStatus.Confirmed
        );
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        var request = new UpdateOrderStatusRequest(orderId, OrderStatus.Preparing);
        var result = await _handler.Handle(request);

        var updatedOrder = await _context.Order.FindAsync(orderId);
        updatedOrder.Should().NotBeNull();
        updatedOrder!.Status.Should().Be(OrderStatus.Preparing);
    }

    [Fact]
    public async Task Handle_ValidTransition_PreparingToCompleted_UpdatesStatus()
    {
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var order = new Order(
            orderId,
            clientId,
            100.00m,
            [],
            [],
            DateTime.UtcNow,
            OrderStatus.Preparing
        );
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        var request = new UpdateOrderStatusRequest(orderId, OrderStatus.Completed);
        var result = await _handler.Handle(request);

        var updatedOrder = await _context.Order.FindAsync(orderId);
        updatedOrder.Should().NotBeNull();
        updatedOrder!.Status.Should().Be(OrderStatus.Completed);
    }

    [Fact]
    public async Task Handle_InvalidTransition_PreparingToPending_ReturnsBadRequest()
    {
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var order = new Order(
            orderId,
            clientId,
            100.00m,
            [],
            [],
            DateTime.UtcNow,
            OrderStatus.Preparing
        );
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        var request = new UpdateOrderStatusRequest(orderId, OrderStatus.Pending);
        var result = await _handler.Handle(request);

        result.Should().NotBeNull();
        var resultType = result.GetType().Name;
        resultType.Should().Contain("BadRequest");

        var orderAfter = await _context.Order.FindAsync(orderId);
        orderAfter!.Status.Should().Be(OrderStatus.Preparing);
    }

    [Fact]
    public async Task Handle_InvalidTransition_CompletedToConfirmed_ReturnsBadRequest()
    {
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var order = new Order(
            orderId,
            clientId,
            100.00m,
            [],
            [],
            DateTime.UtcNow,
            OrderStatus.Completed
        );
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        var request = new UpdateOrderStatusRequest(orderId, OrderStatus.Confirmed);
        var result = await _handler.Handle(request);

        result.Should().NotBeNull();
        var resultType = result.GetType().Name;
        resultType.Should().Contain("BadRequest");

        var orderAfter = await _context.Order.FindAsync(orderId);
        orderAfter!.Status.Should().Be(OrderStatus.Completed);
    }

    [Fact]
    public async Task Handle_SameStatus_AllowsNoOp()
    {
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var order = new Order(
            orderId,
            clientId,
            100.00m,
            [],
            [],
            DateTime.UtcNow,
            OrderStatus.Confirmed
        );
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        var request = new UpdateOrderStatusRequest(orderId, OrderStatus.Confirmed);
        var result = await _handler.Handle(request);

        var updatedOrder = await _context.Order.FindAsync(orderId);
        updatedOrder.Should().NotBeNull();
        updatedOrder!.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task Handle_EmptyOrderId_ReturnsBadRequest()
    {
        var request = new UpdateOrderStatusRequest(Guid.Empty, OrderStatus.Confirmed);

        var result = await _handler.Handle(request);

        result.Should().NotBeNull();
        var resultType = result.GetType().Name;
        resultType.Should().Contain("BadRequest");
    }
}
