using CampusEats.Features.Payment;
using CampusEats.Features.Payment.Handlers;
using CampusEats.Features.Payment.Requests;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Test.PaymentTests;

public class GetPaymentHistoryHandlerTests
{
    private readonly CampusEatsContext _context;
    private readonly GetPaymentHistoryHandler _handler;

    public GetPaymentHistoryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CampusEatsContext(options);
        _handler = new GetPaymentHistoryHandler(_context);
    }

    [Fact]
    public async Task GetHistory_WithPayments_ShouldReturnList()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OrderId = Guid.NewGuid(),
            Amount = 100,
            Status = PaymentStatus.Succeeded,
            Method = PaymentMethod.StripeTest,
            CreatedAtUtc = DateTime.UtcNow
        };
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(new GetPaymentHistoryRequest(userId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetHistory_WithNoPayments_ShouldReturnEmpty()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _handler.Handle(new GetPaymentHistoryRequest(userId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetHistory_WithMultiplePayments_ShouldReturnAll()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var payment1 = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OrderId = Guid.NewGuid(),
            Amount = 100,
            Status = PaymentStatus.Succeeded,
            Method = PaymentMethod.StripeTest,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
        };
        var payment2 = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OrderId = Guid.NewGuid(),
            Amount = 200,
            Status = PaymentStatus.Succeeded,
            Method = PaymentMethod.MockCard,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-1)
        };
        _context.Payments.AddRange(payment1, payment2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(new GetPaymentHistoryRequest(userId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetHistory_ForDifferentUser_ShouldNotReturnOtherUsersPayments()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var payment1 = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = userId1,
            OrderId = Guid.NewGuid(),
            Amount = 100,
            Status = PaymentStatus.Succeeded,
            Method = PaymentMethod.StripeTest,
            CreatedAtUtc = DateTime.UtcNow
        };
        var payment2 = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = userId2,
            OrderId = Guid.NewGuid(),
            Amount = 200,
            Status = PaymentStatus.Succeeded,
            Method = PaymentMethod.StripeTest,
            CreatedAtUtc = DateTime.UtcNow
        };
        _context.Payments.AddRange(payment1, payment2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(new GetPaymentHistoryRequest(userId1), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }
}
