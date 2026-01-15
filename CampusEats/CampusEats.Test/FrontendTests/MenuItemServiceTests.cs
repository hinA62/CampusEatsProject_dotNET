using Xunit;
using FluentAssertions;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Models.MenuItem;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace CampusEats.Test.FrontendTests;

public class MenuItemServiceTests
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly HttpClient _httpClient;
    private readonly MenuItemService _menuItemService;

    public MenuItemServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClient = _mockHttp.ToHttpClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5298/");
        _menuItemService = new MenuItemService(_httpClient);
    }

    [Fact]
    public async Task GetAllMenuItemsAsync_ReturnsItemList()
    {
        var items = new[] {
            new { Id = Guid.NewGuid(), Name = "Burger", Price = 8.99m },
            new { Id = Guid.NewGuid(), Name = "Fries", Price = 3.99m }
        };
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", JsonSerializer.Serialize(items));

        var result = await _menuItemService.GetAllMenuItemsAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result!.First().Name.Should().Be("Burger");
    }

    [Fact]
    public async Task GetMenuItemByIdAsync_WithValidId_ReturnsItem()
    {
        var itemId = Guid.NewGuid();
        var item = new { Id = itemId, Name = "Pizza", Price = 12.99m };
        _mockHttp.When($"http://localhost:5298/api/menu-items/{itemId}").Respond("application/json", JsonSerializer.Serialize(item));

        var result = await _menuItemService.GetMenuItemByIdAsync(itemId);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Pizza");
        result.Price.Should().Be(12.99m);
    }

    [Fact]
    public async Task CreateMenuItemAsync_ReturnsCreatedResponse()
    {
        var createRequest = new CreateMenuItemRequest { Name = "Salad", Price = 7.99m };
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond(HttpStatusCode.Created);

        var result = await _menuItemService.CreateMenuItemAsync(createRequest);

        result.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task CreateMenuItemAsync_WithAllergens_ReturnsSuccess()
    {
        var createRequest = new CreateMenuItemRequest 
        { 
            Name = "Peanut Salad", 
            Price = 8.99m,
            Allergens = new List<string> { "Peanuts", "Soy" }
        };
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond(HttpStatusCode.Created);

        var result = await _menuItemService.CreateMenuItemAsync(createRequest);

        result.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateMenuItemAsync_WithValidData_ReturnsSuccess()
    {
        var updateRequest = new UpdateMenuItemRequest { Id = Guid.NewGuid(), Name = "Updated Burger", Price = 9.99m };
        _mockHttp.When($"http://localhost:5298/api/menu-items/{updateRequest.Id}").Respond(HttpStatusCode.OK);

        var response = await _menuItemService.UpdateMenuItemAsync(updateRequest);

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateMenuItemAsync_WithInvalidId_ReturnsNotFound()
    {
        var updateRequest = new UpdateMenuItemRequest { Id = Guid.NewGuid(), Name = "Invalid Item", Price = 5.99m };
        _mockHttp.When($"http://localhost:5298/api/menu-items/{updateRequest.Id}").Respond(HttpStatusCode.NotFound);

        var response = await _menuItemService.UpdateMenuItemAsync(updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteMenuItemAsync_WithValidId_ReturnsSuccess()
    {
        var itemId = Guid.NewGuid();
        _mockHttp.When($"http://localhost:5298/api/menu-items/{itemId}").Respond(HttpStatusCode.NoContent);

        var response = await _menuItemService.DeleteMenuItemAsync(itemId);

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteMenuItemAsync_WithNonExistentId_ReturnsNotFound()
    {
        var itemId = Guid.NewGuid();
        _mockHttp.When($"http://localhost:5298/api/menu-items/{itemId}").Respond(HttpStatusCode.NotFound);

        var response = await _menuItemService.DeleteMenuItemAsync(itemId);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllMenuItemsAsync_WithEmptyList_ReturnsEmptyList()
    {
        var items = Array.Empty<object>();
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", JsonSerializer.Serialize(items));

        var result = await _menuItemService.GetAllMenuItemsAsync();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
