using CampusEats.Features.Loyalty;
using CampusEats.Features.Loyalty.Handlers;
using CampusEats.Features.Loyalty.Requests;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampusEats.Test.LoyaltyTests.IntegrationTests;

public class GetLoyaltyBalanceHandlerTests : IDisposable
{
    private readonly CampusEatsContext _context;
    private readonly GetLoyaltyBalanceHandler _handler;
    
    public GetLoyaltyBalanceHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        _context = new CampusEatsContext(options);
        _handler = new GetLoyaltyBalanceHandler(_context);
    }

    [Fact]
    public async Task Given_ValidGetLoyaltyBalanceRequest_When_Handle_Then_ShouldReturnLoyaltyBalance()
    {
        //Arrange
        var userId = Guid.NewGuid();
        
        var account = new LoyaltyAccount
        {
            UserId = userId,
            Points = 150,
            TotalPointsEarned = 1200,
            CurrentTier = LoyaltyTier.Silver,
            UpdatedAtUtc = DateTime.UtcNow
        };
        await _context.LoyaltyAccounts.AddAsync(account);
        await _context.SaveChangesAsync();
        
        var request = new GetLoyaltyBalanceRequest(userId);
        
        //Act
        var result = await _handler.Handle(request);
        
        //Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Given_NonExistentUserId_When_Handle_Then_ShouldReturnDefaultBronzeValues()
    {
        //Arrange
        var request = new GetLoyaltyBalanceRequest(Guid.NewGuid());
        
        //Act
        var result = await _handler.Handle(request, CancellationToken.None);
        
        //Assert
        var ok = result.Should().BeAssignableTo<IValueHttpResult>().Subject;
        var value = ok.Value!;

        value.Should().BeEquivalentTo(new 
        { 
            userId = request.UserId, 
            points = 0,
            totalPointsEarned = 0,
            currentTier = LoyaltyTier.Bronze,
            cashbackRate = LoyaltyTierHelper.GetCashbackRate(LoyaltyTier.Bronze),
            nextTier = LoyaltyTier.Silver,
            pointsToNextTier = LoyaltyTierHelper.GetTierThreshold(LoyaltyTier.Silver)
        });
    }

    [Fact]
    public async Task Given_ExistingAccountWithoutMaximalTier_When_Handle_Then_ShouldReturnCorrectValues()
    {
        //Arrange
        var userId = Guid.NewGuid();

        _context.LoyaltyAccounts.Add(new LoyaltyAccount
        {
            UserId = userId,
            Points = 50,
            TotalPointsEarned = 120,
            CurrentTier = LoyaltyTier.Silver
        });
        await _context.SaveChangesAsync();

        var request = new GetLoyaltyBalanceRequest(userId);
        
        //Act
        var result = await _handler.Handle(request);
        
        //Assert
        var ok = result.Should().BeAssignableTo<IValueHttpResult>().Subject;

        ok.Value.Should().BeEquivalentTo(new
        {
            userId,
            points = 50,
            totalPointsEarned = 120,
            currentTier = LoyaltyTier.Silver,
            cashbackRate = LoyaltyTierHelper.GetCashbackRate(LoyaltyTier.Silver),
            nextTier = LoyaltyTierHelper.GetNextTier(LoyaltyTier.Silver),
            pointsToNextTier = LoyaltyTierHelper.GetPointsToNextTier(120, LoyaltyTier.Silver)
        });
    }

    [Fact]
    public async Task Given_ExistingAccountWithMaximalTier_When_Handle_Then_ShouldReturnNoNextTier()
    {
        //Arrange
        var userId = Guid.NewGuid();
        _context.LoyaltyAccounts.Add(new LoyaltyAccount
        {
            UserId = userId,
            Points = 200,
            TotalPointsEarned = 5000,
            CurrentTier = LoyaltyTier.Platinum
        });
        await _context.SaveChangesAsync();
        
        //Act
        var result = await _handler.Handle(new GetLoyaltyBalanceRequest(userId));
        
        //Assert
        result.Should().BeAssignableTo<IValueHttpResult>().Subject.Value.Should().BeEquivalentTo(new
        {
            userId,
            points = 200,
            totalPointsEarned = 5000,
            currentTier = LoyaltyTier.Platinum,
        });
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        
        GC.SuppressFinalize(this);
    }
}