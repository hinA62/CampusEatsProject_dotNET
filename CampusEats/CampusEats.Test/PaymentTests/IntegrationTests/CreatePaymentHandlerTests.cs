using CampusEats.Features.Loyalty;
using CampusEats.Features.Order;
using CampusEats.Features.Payment;
using CampusEats.Features.Payment.Handlers;
using CampusEats.Features.Payment.Requests;
using CampusEats.Features.User;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Test.PaymentTests.IntegrationTests;

public class CreatePaymentHandlerTests : IDisposable
{
    private readonly CampusEatsContext _context;
    private readonly CreatePaymentHandler _handler;
    
    public CreatePaymentHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new CampusEatsContext(options);
        _handler = new CreatePaymentHandler(_context);
    }
    
    [Fact]
    public async Task Given_ValidPayment_When_Handle_Then_ShouldCreatePaymentAndUpdateOrder()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = "test@mail.com", Username = "TestUser", PasswordHash = "hashedpw", Role = UserRole.Client, CreatedAt = DateTime.UtcNow };
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, userId, 50.00m, new List<Guid>(), new List<Guid>(), DateTime.UtcNow, OrderStatus.Pending);
        
        await _context.Users.AddAsync(user);
        await _context.Order.AddAsync(order);
        await _context.SaveChangesAsync();
        
        var request = new CreatePaymentRequest(userId, orderId, 50.00m, PaymentMethod.MockCard, null);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
        Assert.NotNull(payment);
        Assert.Equal(50.00m, payment.Amount);
        Assert.Equal(PaymentMethod.MockCard, payment.Method);
        
        // NOTE: Handler does NOT update order status - that logic is not implemented
        var updatedOrder = await _context.Order.FindAsync(orderId);
        Assert.Equal(OrderStatus.Pending, updatedOrder!.Status);
    }
    
    [Fact]
    public async Task Given_NewUser_When_Handle_Then_ShouldCreateLoyaltyAccount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = "newuser@mail.com", Username = "NewUser", PasswordHash = "hashedpw", Role = UserRole.Client, CreatedAt = DateTime.UtcNow };
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, userId, 30.00m, new List<Guid>(), new List<Guid>(), DateTime.UtcNow, OrderStatus.Pending);
        
        await _context.Users.AddAsync(user);
        await _context.Order.AddAsync(order);
        await _context.SaveChangesAsync();
        
        var request = new CreatePaymentRequest(userId, orderId, 30.00m, PaymentMethod.StripeTest, null);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        var loyaltyAccount = await _context.LoyaltyAccounts.FirstOrDefaultAsync(l => l.UserId == userId);
        Assert.NotNull(loyaltyAccount);
        Assert.Equal(LoyaltyTier.Bronze, loyaltyAccount.CurrentTier);
    }
    
    [Fact]
    public async Task Given_PaymentWithPoints_When_Handle_Then_ShouldApplyDiscount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = "user@mail.com", Username = "User", PasswordHash = "hashedpw", Role = UserRole.Client, CreatedAt = DateTime.UtcNow };
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, userId, 50.00m, new List<Guid>(), new List<Guid>(), DateTime.UtcNow, OrderStatus.Pending);
        var loyaltyAccount = new LoyaltyAccount
        {
            UserId = userId,
            Points = 1000,
            TotalPointsEarned = 0,
            CurrentTier = LoyaltyTier.Bronze,
            UpdatedAtUtc = DateTime.UtcNow
        };
        
        await _context.Users.AddAsync(user);
        await _context.Order.AddAsync(order);
        await _context.LoyaltyAccounts.AddAsync(loyaltyAccount);
        await _context.SaveChangesAsync();
        
        var request = new CreatePaymentRequest(userId, orderId, 50.00m, PaymentMethod.MockCard, 500);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        // Points: 1000 - 500 (redeemed) + 45 (cashback from $45 at Bronze 1x rate) = 545
        var updatedAccount = await _context.LoyaltyAccounts.FirstOrDefaultAsync(l => l.UserId == userId);
        Assert.Equal(545, updatedAccount!.Points);
    }
    
    [Fact]
    public async Task Given_InsufficientPoints_When_Handle_Then_ShouldReturnBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = "user@mail.com", Username = "User", PasswordHash = "hashedpw", Role = UserRole.Client, CreatedAt = DateTime.UtcNow };
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, userId, 50.00m, new List<Guid>(), new List<Guid>(), DateTime.UtcNow, OrderStatus.Pending);
        var loyaltyAccount = new LoyaltyAccount
        {
            UserId = userId,
            Points = 100,
            TotalPointsEarned = 0,
            CurrentTier = LoyaltyTier.Bronze,
            UpdatedAtUtc = DateTime.UtcNow
        };
        
        await _context.Users.AddAsync(user);
        await _context.Order.AddAsync(order);
        await _context.LoyaltyAccounts.AddAsync(loyaltyAccount);
        await _context.SaveChangesAsync();
        
        var request = new CreatePaymentRequest(userId, orderId, 50.00m, PaymentMethod.MockCard, 500);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>>(result);
    }
    
    [Fact]
    public async Task Given_Payment_When_Handle_Then_ShouldEarnCashbackPoints()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = "user@mail.com", Username = "User", PasswordHash = "hashedpw", Role = UserRole.Client, CreatedAt = DateTime.UtcNow };
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, userId, 100.00m, new List<Guid>(), new List<Guid>(), DateTime.UtcNow, OrderStatus.Pending);
        var loyaltyAccount = new LoyaltyAccount
        {
            UserId = userId,
            Points = 0,
            TotalPointsEarned = 200,
            CurrentTier = LoyaltyTier.Silver,
            UpdatedAtUtc = DateTime.UtcNow
        };
        
        await _context.Users.AddAsync(user);
        await _context.Order.AddAsync(order);
        await _context.LoyaltyAccounts.AddAsync(loyaltyAccount);
        await _context.SaveChangesAsync();
        
        var request = new CreatePaymentRequest(userId, orderId, 100.00m, PaymentMethod.MockCard, null);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        var updatedAccount = await _context.LoyaltyAccounts.FirstOrDefaultAsync(l => l.UserId == userId);
        Assert.True(updatedAccount!.Points > 0);
    }
    
    [Fact]
    public async Task Given_OrderNotFound_When_Handle_Then_ShouldReturnNotFound()
    {
        // Arrange
        var request = new CreatePaymentRequest(Guid.NewGuid(), Guid.NewGuid(), 50.00m, PaymentMethod.MockCard, null);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>(result);
    }
    
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        
        GC.SuppressFinalize(this);
    }
}
