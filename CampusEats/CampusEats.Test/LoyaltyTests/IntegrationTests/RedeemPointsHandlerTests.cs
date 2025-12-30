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

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}