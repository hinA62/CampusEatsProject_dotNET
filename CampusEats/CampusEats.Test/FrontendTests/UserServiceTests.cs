using Xunit;
using FluentAssertions;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Models.User;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace CampusEats.Test.FrontendTests;

public class UserServiceTests
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly HttpClient _httpClient;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClient = _mockHttp.ToHttpClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5298/");
        _userService = new UserService(_httpClient);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsUserList()
    {
        var users = new[] {
            new { Id = Guid.NewGuid(), Username = "user1", Email = "user1@test.com", Role = "Customer" },
            new { Id = Guid.NewGuid(), Username = "user2", Email = "user2@test.com", Role = "Admin" }
        };
        _mockHttp.When("http://localhost:5298/api/users").Respond("application/json", JsonSerializer.Serialize(users));

        var result = await _userService.GetAllUsersAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result!.First().Username.Should().Be("user1");
    }

    [Fact]
    public async Task GetAllUsersAsync_WithNoUsers_ReturnsEmptyList()
    {
        var users = Array.Empty<object>();
        _mockHttp.When("http://localhost:5298/api/users").Respond("application/json", JsonSerializer.Serialize(users));

        var result = await _userService.GetAllUsersAsync();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllUsersAsync_OnError_ReturnsEmptyList()
    {
        _mockHttp.When("http://localhost:5298/api/users").Respond(HttpStatusCode.InternalServerError);

        var result = await _userService.GetAllUsersAsync();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ReturnsUser()
    {
        var userId = Guid.NewGuid();
        var user = new { Id = userId, Username = "testuser", Email = "test@test.com", Role = "Customer" };
        _mockHttp.When($"http://localhost:5298/api/users/{userId}").Respond("application/json", JsonSerializer.Serialize(user));

        var result = await _userService.GetUserByIdAsync(userId);

        result.Should().NotBeNull();
        result!.Username.Should().Be("testuser");
        result.Email.Should().Be("test@test.com");
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidId_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        _mockHttp.When($"http://localhost:5298/api/users/{userId}").Respond(HttpStatusCode.NotFound);

        var result = await _userService.GetUserByIdAsync(userId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserByIdAsync_OnError_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        _mockHttp.When($"http://localhost:5298/api/users/{userId}").Respond(HttpStatusCode.InternalServerError);

        var result = await _userService.GetUserByIdAsync(userId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserByIdAsync_WithAdminRole_ReturnsAdminUser()
    {
        var userId = Guid.NewGuid();
        var user = new { Id = userId, Username = "admin", Email = "admin@test.com", Role = "Admin" };
        _mockHttp.When($"http://localhost:5298/api/users/{userId}").Respond("application/json", JsonSerializer.Serialize(user));

        var result = await _userService.GetUserByIdAsync(userId);

        result.Should().NotBeNull();
        result!.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task GetAllUsersAsync_WithMixedRoles_ReturnsAllUsers()
    {
        var users = new[] {
            new { Id = Guid.NewGuid(), Username = "admin", Email = "admin@test.com", Role = "Admin" },
            new { Id = Guid.NewGuid(), Username = "kitchen", Email = "kitchen@test.com", Role = "KitchenStaff" },
            new { Id = Guid.NewGuid(), Username = "customer", Email = "customer@test.com", Role = "Customer" }
        };
        _mockHttp.When("http://localhost:5298/api/users").Respond("application/json", JsonSerializer.Serialize(users));

        var result = await _userService.GetAllUsersAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result!.Select(u => u.Role).Should().Contain(new[] { "Admin", "KitchenStaff", "Customer" });
    }
}
