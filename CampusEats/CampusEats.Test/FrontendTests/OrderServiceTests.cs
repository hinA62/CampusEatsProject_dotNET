using Xunit;
using FluentAssertions;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Models.Order;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace CampusEats.Test.FrontendTests;

public class OrderServiceTests
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly HttpClient _httpClient;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClient = _mockHttp.ToHttpClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5298/");
        _orderService = new OrderService(_httpClient);
    }

    [Fact]
    public async Task GetOrderByIdAsync_WithValidId_ReturnsOrder()
    {
        var orderId = Guid.NewGuid();
        var order = new { Id = orderId, ClientId = Guid.NewGuid(), Price = 50.00m, Status = "Pending", CreatedAt = DateTime.UtcNow, MenuIDs = new[] { Guid.NewGuid() }, ItemIDs = Array.Empty<Guid>() };
        _mockHttp.When($"http://localhost:5298/api/orders/{orderId}").Respond("application/json", JsonSerializer.Serialize(order));

        var result = await _orderService.GetOrderByIdAsync(orderId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(orderId);
        result.Price.Should().Be(50.00m);
    }

    [Fact]
    public async Task GetOrderHistoryAsync_ReturnsOrderList()
    {
        var clientId = Guid.NewGuid();
        var orders = new[] {
            new { Id = Guid.NewGuid(), ClientId = clientId, Price = 25.00m, Status = "Completed", CreatedAt = DateTime.UtcNow, MenuIDs = Array.Empty<Guid>(), ItemIDs = Array.Empty<Guid>() },
            new { Id = Guid.NewGuid(), ClientId = clientId, Price = 35.00m, Status = "Pending", CreatedAt = DateTime.UtcNow, MenuIDs = Array.Empty<Guid>(), ItemIDs = Array.Empty<Guid>() }
        };
        _mockHttp.When($"http://localhost:5298/api/clients/{clientId}/orders").Respond("application/json", JsonSerializer.Serialize(orders));

        var result = await _orderService.GetOrderHistoryAsync(clientId);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task PlaceOrderAsync_WithValidRequest_ReturnsSuccessResponse()
    {
        var request = new PlaceOrderRequest { ClientId = Guid.NewGuid(), MenuIDs = new List<Guid> { Guid.NewGuid() }, ItemIDs = new List<Guid>() };
        _mockHttp.When("http://localhost:5298/api/orders").Respond(HttpStatusCode.Created);

        var response = await _orderService.PlaceOrderAsync(request);

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task PlaceOrderAsync_WithInvalidRequest_ReturnsBadRequest()
    {
        var request = new PlaceOrderRequest { ClientId = Guid.Empty, MenuIDs = new List<Guid>(), ItemIDs = new List<Guid>() };
        _mockHttp.When("http://localhost:5298/api/orders").Respond(HttpStatusCode.BadRequest);

        var response = await _orderService.PlaceOrderAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CancelOrderAsync_WithValidId_ReturnsSuccess()
    {
        var orderId = Guid.NewGuid();
        _mockHttp.When($"http://localhost:5298/api/orders/{orderId}/cancel").Respond(HttpStatusCode.OK);

        var response = await _orderService.CancelOrderAsync(orderId);

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task CancelOrderAsync_WithNonExistentId_ReturnsNotFound()
    {
        var orderId = Guid.NewGuid();
        _mockHttp.When($"http://localhost:5298/api/orders/{orderId}/cancel").Respond(HttpStatusCode.NotFound);

        var response = await _orderService.CancelOrderAsync(orderId);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllOrdersAsync_ReturnsAllOrders()
    {
        var orders = new[] {
            new { Id = Guid.NewGuid(), ClientId = Guid.NewGuid(), Price = 25.00m, Status = "Pending", CreatedAt = DateTime.UtcNow, MenuIDs = Array.Empty<Guid>(), ItemIDs = Array.Empty<Guid>() },
            new { Id = Guid.NewGuid(), ClientId = Guid.NewGuid(), Price = 35.00m, Status = "Completed", CreatedAt = DateTime.UtcNow, MenuIDs = Array.Empty<Guid>(), ItemIDs = Array.Empty<Guid>() }
        };
        _mockHttp.When("http://localhost:5298/api/orders").Respond("application/json", JsonSerializer.Serialize(orders));

        var result = await _orderService.GetAllOrdersAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllOrdersAsync_WhenFails_ReturnsEmptyList()
    {
        _mockHttp.When("http://localhost:5298/api/orders").Respond(HttpStatusCode.InternalServerError);

        var result = await _orderService.GetAllOrdersAsync();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetOrderDetailsAsync_WithValidId_ReturnsDetails()
    {
        var orderId = Guid.NewGuid();
        var details = new { Id = orderId, ClientId = Guid.NewGuid(), ClientUsername = "testuser", ClientEmail = "test@test.com", Price = 75.00m, Status = "Confirmed", CreatedAt = DateTime.UtcNow };
        _mockHttp.When($"http://localhost:5298/api/orders/{orderId}/details").Respond("application/json", JsonSerializer.Serialize(details));

        var result = await _orderService.GetOrderDetailsAsync(orderId);

        result.Should().NotBeNull();
        result!.ClientUsername.Should().Be("testuser");
    }

    [Fact]
    public async Task GetOrderDetailsAsync_WhenNotFound_ReturnsNull()
    {
        var orderId = Guid.NewGuid();
        _mockHttp.When($"http://localhost:5298/api/orders/{orderId}/details").Respond(HttpStatusCode.NotFound);

        var result = await _orderService.GetOrderDetailsAsync(orderId);

        result.Should().BeNull();
    }
}
