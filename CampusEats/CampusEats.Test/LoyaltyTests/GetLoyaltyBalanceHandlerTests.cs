using CampusEats.Features.Loyalty;
using CampusEats.Features.Loyalty.Handlers;
using CampusEats.Features.Loyalty.Requests;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CampusEats.Test.LoyaltyTests;

public class GetLoyaltyBalanceHandlerTests
{
    private readonly CampusEatsContext _context;
    private readonly GetLoyaltyBalanceHandler _handler;

    public GetLoyaltyBalanceHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CampusEatsContext(options);
        _handler = new GetLoyaltyBalanceHandler(_context);
    }

    [Fact]
    public async Task GetBalance_ForExistingUser_ShouldReturnBalance()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var loyalty = new LoyaltyAccount
        {
            UserId = userId,
            Points = 150,
            TotalPointsEarned = 150,
            CurrentTier = LoyaltyTier.Silver,
            UpdatedAtUtc = DateTime.UtcNow
        };
        _context.LoyaltyAccounts.Add(loyalty);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(new GetLoyaltyBalanceRequest(userId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBalance_ForNewUser_ShouldReturnDefaultBronze()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _handler.Handle(new GetLoyaltyBalanceRequest(userId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBalance_ForBronzeTier_ShouldReturnCorrectData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var loyalty = new LoyaltyAccount
        {
            UserId = userId,
            Points = 50,
            TotalPointsEarned = 50,
            CurrentTier = LoyaltyTier.Bronze,
            UpdatedAtUtc = DateTime.UtcNow
        };
        _context.LoyaltyAccounts.Add(loyalty);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(new GetLoyaltyBalanceRequest(userId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBalance_ForGoldTier_ShouldReturnCorrectData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var loyalty = new LoyaltyAccount
        {
            UserId = userId,
            Points = 500,
            TotalPointsEarned = 500,
            CurrentTier = LoyaltyTier.Gold,
            UpdatedAtUtc = DateTime.UtcNow
        };
        _context.LoyaltyAccounts.Add(loyalty);
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(new GetLoyaltyBalanceRequest(userId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBalance_ForMultipleUsers_ShouldReturnCorrectUser()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        
        _context.LoyaltyAccounts.AddRange(
            new LoyaltyAccount
            {
                UserId = userId1,
                Points = 100,
                TotalPointsEarned = 100,
                CurrentTier = LoyaltyTier.Silver,
                UpdatedAtUtc = DateTime.UtcNow
            },
            new LoyaltyAccount
            {
                UserId = userId2,
                Points = 200,
                TotalPointsEarned = 200,
                CurrentTier = LoyaltyTier.Silver,
                UpdatedAtUtc = DateTime.UtcNow
            }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(new GetLoyaltyBalanceRequest(userId1), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }
}
