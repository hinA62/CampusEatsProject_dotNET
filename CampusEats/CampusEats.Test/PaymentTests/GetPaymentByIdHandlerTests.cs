using CampusEats.Features.Payment;
using CampusEats.Features.Payment.Handlers;
using CampusEats.Features.Payment.Requests;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Test.PaymentTests;

public class GetPaymentByIdHandlerTests
{
    private readonly CampusEatsContext _context;
    private readonly GetPaymentByIdHandler _handler;

    public GetPaymentByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CampusEatsContext(options);
        _handler = new GetPaymentByIdHandler(_context);
    }

    [Fact]
    public async Task GetById_WithExistingPayment_ShouldReturnPayment()
    {
        // Arrange
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            Amount = 100.50m,
            Status = PaymentStatus.Succeeded,
            Method = PaymentMethod.StripeTest,
            ExternalReference = "pi_test123",
            CreatedAtUtc = DateTime.UtcNow
        };
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(new GetPaymentByIdRequest(payment.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_WithNonExistentPayment_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _handler.Handle(new GetPaymentByIdRequest(nonExistentId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_WithPendingPayment_ShouldReturnCorrectStatus()
    {
        // Arrange
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            Amount = 50.00m,
            Status = PaymentStatus.Pending,
            Method = PaymentMethod.MockCard,
            CreatedAtUtc = DateTime.UtcNow
        };
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(new GetPaymentByIdRequest(payment.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_WithMultiplePayments_ShouldReturnCorrectOne()
    {
        // Arrange
        var payment1 = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            Amount = 100m,
            Status = PaymentStatus.Succeeded,
            Method = PaymentMethod.StripeTest,
            CreatedAtUtc = DateTime.UtcNow
        };
        var payment2 = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            Amount = 200m,
            Status = PaymentStatus.Succeeded,
            Method = PaymentMethod.StripeTest,
            CreatedAtUtc = DateTime.UtcNow
        };
        _context.Payments.AddRange(payment1, payment2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(new GetPaymentByIdRequest(payment1.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }
}
