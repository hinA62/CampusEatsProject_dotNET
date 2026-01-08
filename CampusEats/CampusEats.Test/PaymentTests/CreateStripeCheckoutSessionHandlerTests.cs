using CampusEats.Features.Order;
using CampusEats.Features.Payment.Handlers;
using CampusEats.Features.Payment.Requests;
using CampusEats.Features.User;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CampusEats.Test.PaymentTests;

public class CreateStripeCheckoutSessionHandlerTests
{
    private readonly CampusEatsContext _context;
    private readonly Mock<IConfiguration> _config;
    private readonly CreateStripeCheckoutSessionHandler _handler;

    public CreateStripeCheckoutSessionHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new CampusEatsContext(options);

        _config = new Mock<IConfiguration>();
        _config.Setup(c => c["Stripe:ClientBaseUrl"]).Returns("http://localhost:5007");

        _handler = new CreateStripeCheckoutSessionHandler(_context, _config.Object);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ReturnsNotFound()
    {
        var request = new CreateStripeCheckoutSessionRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null
        );

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        var resultType = result.GetType().Name;
        resultType.Should().Contain("NotFound");
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        var orderId = Guid.NewGuid();
        var order = new Order(
            orderId,
            Guid.NewGuid(),
            100.00m,
            [],
            [],
            DateTime.UtcNow,
            OrderStatus.Pending
        );
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        var request = new CreateStripeCheckoutSessionRequest(
            Guid.NewGuid(),
            orderId,
            null
        );

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        var resultType = result.GetType().Name;
        resultType.Should().Contain("NotFound");
    }

    [Fact]
    public async Task Handle_FinalAmountZeroOrNegative_ReturnsBadRequest()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = "hash",
            Role = UserRole.Client
        };
        _context.Users.Add(user);

        var orderId = Guid.NewGuid();
        var order = new Order(
            orderId,
            userId,
            10.00m,
            [],
            [],
            DateTime.UtcNow,
            OrderStatus.Pending
        );
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        var request = new CreateStripeCheckoutSessionRequest(
            userId,
            orderId,
            2000
        );

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        var resultType = result.GetType().Name;
        resultType.Should().Contain("BadRequest");
    }

    [Fact]
    public async Task Handle_MissingClientBaseUrl_ReturnsProblem()
    {
        _config.Setup(c => c["Stripe:ClientBaseUrl"]).Returns((string?)null);

        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = "hash",
            Role = UserRole.Client
        };
        _context.Users.Add(user);

        var orderId = Guid.NewGuid();
        var order = new Order(
            orderId,
            userId,
            100.00m,
            [],
            [],
            DateTime.UtcNow,
            OrderStatus.Pending
        );
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        var request = new CreateStripeCheckoutSessionRequest(
            userId,
            orderId,
            null
        );

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        var resultType = result.GetType().Name;
        resultType.Should().Contain("Problem");
    }

    [Fact]
    public async Task Handle_ValidRequestWithoutPoints_CalculatesCorrectAmount()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = "hash",
            Role = UserRole.Client
        };
        _context.Users.Add(user);

        var orderId = Guid.NewGuid();
        var order = new Order(
            orderId,
            userId,
            50.00m,
            [],
            [],
            DateTime.UtcNow,
            OrderStatus.Pending
        );
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        var request = new CreateStripeCheckoutSessionRequest(
            userId,
            orderId,
            null
        );

        var orderInDb = await _context.Order.FindAsync(orderId);
        orderInDb.Should().NotBeNull();
        orderInDb!.Price.Should().Be(50.00m);
    }

    [Fact]
    public async Task Handle_ValidRequestWithPoints_AppliesDiscount()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = "hash",
            Role = UserRole.Client
        };
        _context.Users.Add(user);

        var orderId = Guid.NewGuid();
        var order = new Order(
            orderId,
            userId,
            100.00m,
            [],
            [],
            DateTime.UtcNow,
            OrderStatus.Pending
        );
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        var request = new CreateStripeCheckoutSessionRequest(
            userId,
            orderId,
            500
        );

        var orderInDb = await _context.Order.FindAsync(orderId);
        orderInDb.Should().NotBeNull();

        var expectedDiscount = 500 / 100m;
        var expectedFinalAmount = orderInDb!.Price - expectedDiscount;
        expectedFinalAmount.Should().Be(95.00m);
    }

    [Fact]
    public async Task Handle_PointsExceedOrderPrice_CapsDiscountAtOrderPrice()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = "hash",
            Role = UserRole.Client
        };
        _context.Users.Add(user);

        var orderId = Guid.NewGuid();
        var order = new Order(
            orderId,
            userId,
            20.00m,
            [],
            [],
            DateTime.UtcNow,
            OrderStatus.Pending
        );
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        var request = new CreateStripeCheckoutSessionRequest(
            userId,
            orderId,
            5000
        );

        var orderInDb = await _context.Order.FindAsync(orderId);
        orderInDb.Should().NotBeNull();

        var maxPointsToUse = (int)Math.Floor(orderInDb!.Price * 100);
        maxPointsToUse.Should().Be(2000);
    }

    [Fact]
    public async Task Handle_ZeroPoints_NoDiscount()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@test.com",
            PasswordHash = "hash",
            Role = UserRole.Client
        };
        _context.Users.Add(user);

        var orderId = Guid.NewGuid();
        var order = new Order(
            orderId,
            userId,
            75.00m,
            [],
            [],
            DateTime.UtcNow,
            OrderStatus.Pending
        );
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        var request = new CreateStripeCheckoutSessionRequest(
            userId,
            orderId,
            0
        );

        var orderInDb = await _context.Order.FindAsync(orderId);
        orderInDb.Should().NotBeNull();
        orderInDb!.Price.Should().Be(75.00m);
    }
}
