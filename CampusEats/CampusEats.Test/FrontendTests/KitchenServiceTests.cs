using Xunit;
using FluentAssertions;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Models.Order;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace CampusEats.Test.FrontendTests;

public class KitchenServiceTests
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly HttpClient _httpClient;
    private readonly KitchenService _kitchenService;

    public KitchenServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClient = _mockHttp.ToHttpClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5298/");
        _kitchenService = new KitchenService(_httpClient);
    }

    [Fact]
    public async Task GetKitchenOrdersAsync_ReturnsOrderList()
    {
        var orders = new[] {
            new { Id = Guid.NewGuid(), ClientId = Guid.NewGuid(), Price = 25.00m, Status = 1 },
            new { Id = Guid.NewGuid(), ClientId = Guid.NewGuid(), Price = 35.00m, Status = 2 }
        };
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", JsonSerializer.Serialize(orders));

        var result = await _kitchenService.GetKitchenOrdersAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetKitchenOrdersAsync_WithStatusFilter_ReturnsFilteredOrders()
    {
        var orders = new[] {
            new { Id = Guid.NewGuid(), ClientId = Guid.NewGuid(), Price = 25.00m, Status = 0 }
        };
        _mockHttp.When("http://localhost:5298/api/kitchen/orders?status=Pending").Respond("application/json", JsonSerializer.Serialize(orders));

        var result = await _kitchenService.GetKitchenOrdersAsync("Pending");

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetKitchenOrdersAsync_WithNoOrders_ReturnsEmptyList()
    {
        var orders = Array.Empty<object>();
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", JsonSerializer.Serialize(orders));

        var result = await _kitchenService.GetKitchenOrdersAsync();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ToPreparing_ReturnsSuccess()
    {
        var orderId = Guid.NewGuid();
        _mockHttp.When($"http://localhost:5298/api/kitchen/orders/{orderId}/status?newStatus=Preparing").Respond(HttpStatusCode.OK);

        var result = await _kitchenService.UpdateOrderStatusAsync(orderId, OrderStatus.Preparing);

        result.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ToCompleted_ReturnsSuccess()
    {
        var orderId = Guid.NewGuid();
        _mockHttp.When($"http://localhost:5298/api/kitchen/orders/{orderId}/status?newStatus=Completed").Respond(HttpStatusCode.OK);

        var result = await _kitchenService.UpdateOrderStatusAsync(orderId, OrderStatus.Completed);

        result.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ToConfirmed_ReturnsSuccess()
    {
        var orderId = Guid.NewGuid();
        _mockHttp.When($"http://localhost:5298/api/kitchen/orders/{orderId}/status?newStatus=Confirmed").Respond(HttpStatusCode.OK);

        var result = await _kitchenService.UpdateOrderStatusAsync(orderId, OrderStatus.Confirmed);

        result.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WithInvalidOrder_ReturnsNotFound()
    {
        var orderId = Guid.NewGuid();
        _mockHttp.When($"http://localhost:5298/api/kitchen/orders/{orderId}/status?newStatus=Preparing").Respond(HttpStatusCode.NotFound);

        var result = await _kitchenService.UpdateOrderStatusAsync(orderId, OrderStatus.Preparing);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ToCancelled_ReturnsSuccess()
    {
        var orderId = Guid.NewGuid();
        _mockHttp.When($"http://localhost:5298/api/kitchen/orders/{orderId}/status?newStatus=Cancelled").Respond(HttpStatusCode.OK);

        var result = await _kitchenService.UpdateOrderStatusAsync(orderId, OrderStatus.Cancelled);

        result.IsSuccessStatusCode.Should().BeTrue();
    }
}
