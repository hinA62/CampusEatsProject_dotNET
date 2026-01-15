using Xunit;
using FluentAssertions;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Models.Menu;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace CampusEats.Test.FrontendTests;

public class MenuServiceTests
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly HttpClient _httpClient;
    private readonly MenuService _menuService;

    public MenuServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClient = _mockHttp.ToHttpClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5298/");
        _menuService = new MenuService(_httpClient);
    }

    [Fact]
    public async Task GetAllMenusAsync_ReturnsMenuList()
    {
        var menus = new[] {
            new { Id = Guid.NewGuid(), Name = "Breakfast", Price = 15.00m },
            new { Id = Guid.NewGuid(), Name = "Lunch", Price = 20.00m }
        };
        _mockHttp.When("http://localhost:5298/api/menu").Respond("application/json", JsonSerializer.Serialize(menus));

        var result = await _menuService.GetAllMenusAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result!.First().Name.Should().Be("Breakfast");
    }

    [Fact]
    public async Task GetMenuByIdAsync_WithValidId_ReturnsMenu()
    {
        var menuId = Guid.NewGuid();
        var menu = new { Id = menuId, Name = "Special Menu", Price = 30.00m };
        _mockHttp.When($"http://localhost:5298/api/menu/{menuId}").Respond("application/json", JsonSerializer.Serialize(menu));

        var result = await _menuService.GetMenuByIdAsync(menuId);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Special Menu");
        result.Price.Should().Be(30.00m);
    }

    [Fact]
    public async Task CreateMenuAsync_ReturnsCreatedMenu()
    {
        var createRequest = new CreateMenuRequest { Name = "New Menu", Price = 25.00m, ItemIds = new List<Guid>() };
        var createdMenu = new { Id = Guid.NewGuid(), Name = "New Menu", Price = 25.00m };
        _mockHttp.When("http://localhost:5298/api/menu").Respond("application/json", JsonSerializer.Serialize(createdMenu));

        var result = await _menuService.CreateMenuAsync(createRequest);

        result.Should().NotBeNull();
        result.Name.Should().Be("New Menu");
    }

    [Fact]
    public async Task UpdateMenuAsync_WithValidData_ReturnsSuccess()
    {
        var updateRequest = new UpdateMenuRequest { Id = Guid.NewGuid(), Name = "Updated Menu", Price = 35.00m };
        _mockHttp.When($"http://localhost:5298/api/menu/{updateRequest.Id}").Respond(HttpStatusCode.OK);

        var response = await _menuService.UpdateMenuAsync(updateRequest);

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteMenuAsync_WithValidId_ReturnsSuccess()
    {
        var menuId = Guid.NewGuid();
        _mockHttp.When($"http://localhost:5298/api/menu/{menuId}").Respond(HttpStatusCode.NoContent);

        var response = await _menuService.DeleteMenuAsync(menuId);

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteMenuAsync_WithNonExistentId_ReturnsNotFound()
    {
        var menuId = Guid.NewGuid();
        _mockHttp.When($"http://localhost:5298/api/menu/{menuId}").Respond(HttpStatusCode.NotFound);

        var response = await _menuService.DeleteMenuAsync(menuId);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
