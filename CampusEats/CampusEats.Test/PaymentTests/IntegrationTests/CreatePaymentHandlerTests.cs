using CampusEats.Features.Loyalty;
using CampusEats.Features.Order;
using CampusEats.Features.Payment;
using CampusEats.Features.Payment.Handlers;
using CampusEats.Features.Payment.Requests;
using CampusEats.Features.User;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        // Adăugăm comanda dar NU și userul
        var order = new Order(orderId, Guid.NewGuid(), 50.00m, [], [], DateTime.UtcNow, OrderStatus.Pending);
        await _context.Order.AddAsync(order);
        await _context.SaveChangesAsync();
        
        var request = new CreatePaymentRequest(order.ClientId, orderId, 50.00m, PaymentMethod.MockCard, null);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>()
              .Which.Value.Should().Be("User not found");
    }

    [Fact]
    public async Task Handle_DiscountExceedsAmount_CapsDiscountToAmount()
    {
        // Arrange
        var userId = await SeedUser();
        var orderId = await SeedOrder(userId, 10.00m); // Comandă mică: $10
        
        var loyaltyAccount = new LoyaltyAccount
        {
            UserId = userId,
            Points = 5000, // Are $50 valoare în puncte
            CurrentTier = LoyaltyTier.Bronze
        };
        await _context.LoyaltyAccounts.AddAsync(loyaltyAccount);
        await _context.SaveChangesAsync();

        // Cerem să folosim 2000 puncte ($20 discount) pentru o comandă de $10
        var request = new CreatePaymentRequest(userId, orderId, 10.00m, PaymentMethod.MockCard, 2000);
        
        // Act
        var result = await _handler.Handle(request);
        
        // Assert
        var updatedAccount = await _context.LoyaltyAccounts.FirstAsync(a => a.UserId == userId);
        // Discount-ul a fost plafonat la $10, deci s-au consumat doar 1000 puncte
        // 5000 (inițial) - 1000 (folosite) = 4000
        updatedAccount.Points.Should().Be(4000);
        
        var payment = await _context.Payments.FirstAsync(p => p.OrderId == orderId);
        payment.Amount.Should().Be(0); // Plata finală a fost 0
    }

    [Fact]
    public async Task Handle_HighSpending_UpdatesTierToSilver()
    {
        // Arrange
        var userId = await SeedUser();
        // Scădem suma de la 600 la 200 pentru a nu sări direct la Gold
        var orderAmount = 200.00m; 
        var orderId = await SeedOrder(userId, orderAmount);
    
        var request = new CreatePaymentRequest(userId, orderId, orderAmount, PaymentMethod.MockCard, null);
    
        // Act
        await _handler.Handle(request);
    
        // Assert
        var account = await _context.LoyaltyAccounts.FirstAsync(a => a.UserId == userId);
    
        // Verificăm ce tier a calculat sistemul tău
        // Dacă tot Gold este, înseamnă că pragurile tale sunt foarte mici (ex: sub 200 pachetul Gold)
        account.CurrentTier.Should().Be(LoyaltyTier.Silver);
    }

    [Fact]
    public async Task Handle_PointsUsedAndCashbackEarned_VerifiesTransactions()
    {
        // Arrange
        var userId = await SeedUser();
        var orderId = await SeedOrder(userId, 100.00m);
        await _context.LoyaltyAccounts.AddAsync(new LoyaltyAccount { UserId = userId, Points = 1000 });
        await _context.SaveChangesAsync();

        var request = new CreatePaymentRequest(userId, orderId, 100.00m, PaymentMethod.MockCard, 500); // $5 discount
        
        // Act
        await _handler.Handle(request);

        // Assert
        var transactions = await _context.LoyaltyTransactions.Where(t => t.UserId == userId).ToListAsync();
        transactions.Should().HaveCount(2);
        transactions.Should().Contain(t => t.Type == LoyaltyTransactionType.Redeem && t.Points == -500);
        transactions.Should().Contain(t => t.Type == LoyaltyTransactionType.CashbackBonus);
    }

    // --- Helpers ---

    private async Task<Guid> SeedUser()
    {
        var user = new User { Id = Guid.NewGuid(), Email = $"{Guid.NewGuid()}@test.com", Username = "Test", PasswordHash = "hash", Role = UserRole.Client };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user.Id;
    }

    private async Task<Guid> SeedOrder(Guid userId, decimal price)
    {
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, userId, price, [], [], DateTime.UtcNow, OrderStatus.Pending);
        await _context.Order.AddAsync(order);
        await _context.SaveChangesAsync();
        return orderId;
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}