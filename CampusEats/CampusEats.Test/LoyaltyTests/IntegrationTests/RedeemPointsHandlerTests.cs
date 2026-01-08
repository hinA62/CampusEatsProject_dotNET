using CampusEats.Features.Loyalty;
using CampusEats.Features.Loyalty.Handlers;
using CampusEats.Features.Loyalty.Requests;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusEats.Test.LoyaltyTests.IntegrationTests;

public class RedeemPointsHandlerTests : IDisposable
{
    private readonly CampusEatsContext _context;
    private readonly RedeemPointsHandler _handler;
    private readonly ILogger<RedeemPointsHandler> _logger;

    public RedeemPointsHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;
        _context = new CampusEatsContext(options);
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<RedeemPointsHandler>();
        _handler = new RedeemPointsHandler(_context, _logger);
    }

    [Fact]
    public async Task Given_ValidRedeemPointsRequest_When_Handle_Then_ShouldRedeemPoints()
    {
        //Arrange
        var userId = Guid.NewGuid();

        var account = new LoyaltyAccount
        {
            UserId = userId,
            Points = 100,
            TotalPointsEarned = 500,
            CurrentTier = LoyaltyTier.Gold,
            UpdatedAtUtc = DateTime.UtcNow
        };
        await _context.LoyaltyAccounts.AddAsync(account);
        await _context.SaveChangesAsync();

        var request = new RedeemPointsRequest(userId, 500);
     
        //Act
        var result = await _handler.Handle(request, CancellationToken.None);
        
        //Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Given_ValidationFailure_When_Handle_Then_ShouldReturnBadRequest()
    {
        //Arrange
        var request = new RedeemPointsRequest(Guid.Empty, -10);
        
        //Act
        var result = await _handler.Handle(request, CancellationToken.None);
        
        //Assert
        result.Should().BeOfType<BadRequest<List<FluentValidation.Results.ValidationFailure>>>();
    }

    [Fact]
    public async Task Given_NonExistentUserId_When_Handle_Then_ShouldReturnNotFound()
    {
        //Arrange
        var request = new RedeemPointsRequest(Guid.NewGuid(), 100);
        
        //Act
        var result = await _handler.Handle(request, CancellationToken.None);
        
        //Assert
        var badRequest = result as BadRequest<string>;
        badRequest.Should().NotBeNull();
        badRequest!.Value.Should().Be("Loyalty account not found.");
    }

    [Fact]
    public async Task Given_MorePointsThanAvailable_When_Handle_Then_ShouldReturnInsufficientPoints()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var account = new LoyaltyAccount
        {
            UserId = userId,
            Points = 100,
            TotalPointsEarned = 300,
            CurrentTier = LoyaltyTier.Bronze,
            UpdatedAtUtc = DateTime.UtcNow
        };
        await _context.LoyaltyAccounts.AddAsync(account);
        await _context.SaveChangesAsync();
        
        var request = new RedeemPointsRequest(userId, 500);
        
        //Act
        var result = await _handler.Handle(request, CancellationToken.None);
        
        //Assert
        var badRequest = result as BadRequest<string>;
        badRequest.Should().NotBeNull();
        badRequest!.Value.Should().Be("Not enough points.");
    }

    [Fact]
    public async Task Given_ExactPointsAvailable_When_Handle_Then_ShouldRedeemAllPoints()
    {
        var userId = Guid.NewGuid();
        var account = new LoyaltyAccount
        {
            UserId = userId,
            Points = 250,
            TotalPointsEarned = 500,
            CurrentTier = LoyaltyTier.Silver,
            UpdatedAtUtc = DateTime.UtcNow
        };
        await _context.LoyaltyAccounts.AddAsync(account);
        await _context.SaveChangesAsync();

        var request = new RedeemPointsRequest(userId, 250);
        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        var resultType = result.GetType().Name;
        resultType.Should().Contain("Ok");
        var updatedAccount = await _context.LoyaltyAccounts.FirstAsync(a => a.UserId == userId);
        updatedAccount.Points.Should().Be(0);
    }

    [Fact]
    public async Task Given_SuccessfulRedemption_When_Handle_Then_ShouldCreateTransaction()
    {
        var userId = Guid.NewGuid();
        var account = new LoyaltyAccount
        {
            UserId = userId,
            Points = 300,
            TotalPointsEarned = 600,
            CurrentTier = LoyaltyTier.Gold,
            UpdatedAtUtc = DateTime.UtcNow
        };
        await _context.LoyaltyAccounts.AddAsync(account);
        await _context.SaveChangesAsync();

        var request = new RedeemPointsRequest(userId, 150);
        var result = await _handler.Handle(request, CancellationToken.None);

        var transaction = await _context.LoyaltyTransactions
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Type == LoyaltyTransactionType.Redeem);
        
        transaction.Should().NotBeNull();
        transaction!.Points.Should().Be(150);
        transaction.Description.Should().Be("Redeemed points");
    }

    [Fact]
    public async Task Given_SuccessfulRedemption_When_Handle_Then_ShouldUpdateTimestamp()
    {
        var userId = Guid.NewGuid();
        var oldTimestamp = DateTime.UtcNow.AddDays(-5);
        var account = new LoyaltyAccount
        {
            UserId = userId,
            Points = 200,
            TotalPointsEarned = 400,
            CurrentTier = LoyaltyTier.Bronze,
            UpdatedAtUtc = oldTimestamp
        };
        await _context.LoyaltyAccounts.AddAsync(account);
        await _context.SaveChangesAsync();

        var request = new RedeemPointsRequest(userId, 50);
        var result = await _handler.Handle(request, CancellationToken.None);

        var updatedAccount = await _context.LoyaltyAccounts.FirstAsync(a => a.UserId == userId);
        updatedAccount.UpdatedAtUtc.Should().BeAfter(oldTimestamp);
    }

    [Fact]
    public async Task Given_ZeroPointRedemption_When_Handle_Then_ShouldReturnBadRequest()
    {
        var userId = Guid.NewGuid();
        var account = new LoyaltyAccount
        {
            UserId = userId,
            Points = 100,
            TotalPointsEarned = 200,
            CurrentTier = LoyaltyTier.Bronze,
            UpdatedAtUtc = DateTime.UtcNow
        };
        await _context.LoyaltyAccounts.AddAsync(account);
        await _context.SaveChangesAsync();

        var request = new RedeemPointsRequest(userId, 0);
        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().BeOfType<BadRequest<List<FluentValidation.Results.ValidationFailure>>>();
    }

    [Fact]
    public async Task Given_MultipleRedemptions_When_Handle_Then_ShouldDecrementPointsCorrectly()
    {
        var userId = Guid.NewGuid();
        var account = new LoyaltyAccount
        {
            UserId = userId,
            Points = 500,
            TotalPointsEarned = 1000,
            CurrentTier = LoyaltyTier.Gold,
            UpdatedAtUtc = DateTime.UtcNow
        };
        await _context.LoyaltyAccounts.AddAsync(account);
        await _context.SaveChangesAsync();

        await _handler.Handle(new RedeemPointsRequest(userId, 100), CancellationToken.None);
        await _handler.Handle(new RedeemPointsRequest(userId, 150), CancellationToken.None);

        var updatedAccount = await _context.LoyaltyAccounts.FirstAsync(a => a.UserId == userId);
        updatedAccount.Points.Should().Be(250);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}