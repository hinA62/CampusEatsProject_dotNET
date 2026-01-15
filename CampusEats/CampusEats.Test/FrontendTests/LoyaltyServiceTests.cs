using Xunit;
using FluentAssertions;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Models.Loyalty;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace CampusEats.Test.FrontendTests;

public class LoyaltyServiceTests
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly HttpClient _httpClient;
    private readonly LoyaltyService _loyaltyService;

    public LoyaltyServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClient = _mockHttp.ToHttpClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5298/");
        _loyaltyService = new LoyaltyService(_httpClient);
    }

    [Fact]
    public async Task GetLoyaltyBalanceAsync_WithValidUser_ReturnsBalance()
    {
        var userId = Guid.NewGuid();
        var balance = new { UserId = userId, Points = 500, CurrentTier = 2 }; // 2 = Gold enum
        _mockHttp.When($"http://localhost:5298/api/loyalty/{userId}/balance").Respond("application/json", JsonSerializer.Serialize(balance));

        var result = await _loyaltyService.GetLoyaltyBalanceAsync(userId);

        result.Should().NotBeNull();
        result!.Points.Should().Be(500);
    }

    [Fact]
    public async Task GetLoyaltyBalanceAsync_WithNewUser_ReturnsZeroBalance()
    {
        var userId = Guid.NewGuid();
        var balance = new { UserId = userId, Points = 0, CurrentTier = 0 }; // 0 = Bronze
        _mockHttp.When($"http://localhost:5298/api/loyalty/{userId}/balance").Respond("application/json", JsonSerializer.Serialize(balance));

        var result = await _loyaltyService.GetLoyaltyBalanceAsync(userId);

        result.Should().NotBeNull();
        result!.Points.Should().Be(0);
    }

    [Fact]
    public async Task RedeemPointsAsync_WithEnoughPoints_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        var request = new RedeemPointsRequest { UserId = userId, PointsToRedeem = 100 };
        _mockHttp.When("http://localhost:5298/api/loyalty/redeem").Respond(HttpStatusCode.OK);

        var result = await _loyaltyService.RedeemPointsAsync(request);

        result.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task RedeemPointsAsync_WithNotEnoughPoints_ReturnsBadRequest()
    {
        var userId = Guid.NewGuid();
        var request = new RedeemPointsRequest { UserId = userId, PointsToRedeem = 10000 };
        _mockHttp.When("http://localhost:5298/api/loyalty/redeem").Respond(HttpStatusCode.BadRequest);

        var result = await _loyaltyService.RedeemPointsAsync(request);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetLoyaltyTransactionsAsync_ReturnsTransactionList()
    {
        var userId = Guid.NewGuid();
        var transactions = new[] {
            new { Id = Guid.NewGuid(), UserId = userId, Type = 0, Points = 50, Description = "Order completed" },
            new { Id = Guid.NewGuid(), UserId = userId, Type = 1, Points = -20, Description = "Points redeemed" }
        };
        _mockHttp.When($"http://localhost:5298/api/loyalty/{userId}/transactions").Respond("application/json", JsonSerializer.Serialize(transactions));

        var result = await _loyaltyService.GetLoyaltyTransactionsAsync(userId);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetLoyaltyTransactionsAsync_WithNoTransactions_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();
        var transactions = Array.Empty<object>();
        _mockHttp.When($"http://localhost:5298/api/loyalty/{userId}/transactions").Respond("application/json", JsonSerializer.Serialize(transactions));

        var result = await _loyaltyService.GetLoyaltyTransactionsAsync(userId);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetLoyaltyBalanceAsync_WithPlatinumTier_ReturnsCorrectTier()
    {
        var userId = Guid.NewGuid();
        var balance = new { UserId = userId, Points = 5000, CurrentTier = 3, TotalPointsEarned = 10000 }; // 3 = Platinum
        _mockHttp.When($"http://localhost:5298/api/loyalty/{userId}/balance").Respond("application/json", JsonSerializer.Serialize(balance));

        var result = await _loyaltyService.GetLoyaltyBalanceAsync(userId);

        result.Should().NotBeNull();
        result!.CurrentTier.Should().Be(LoyaltyTier.Platinum);
    }

    [Fact]
    public async Task RedeemPointsAsync_WithZeroPoints_ReturnsBadRequest()
    {
        var userId = Guid.NewGuid();
        var request = new RedeemPointsRequest { UserId = userId, PointsToRedeem = 0 };
        _mockHttp.When("http://localhost:5298/api/loyalty/redeem").Respond(HttpStatusCode.BadRequest);

        var result = await _loyaltyService.RedeemPointsAsync(request);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
