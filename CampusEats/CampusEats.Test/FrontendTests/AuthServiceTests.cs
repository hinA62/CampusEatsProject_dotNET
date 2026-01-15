using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.JSInterop;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Models.Auth;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace CampusEats.Test.FrontendTests;

public class AuthServiceTests
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly HttpClient _httpClient;
    private readonly Mock<IJSRuntime> _mockJs;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClient = _mockHttp.ToHttpClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5298/");
        _mockJs = new Mock<IJSRuntime>();
        _authService = new AuthService(_httpClient, _mockJs.Object);
    }

    [Fact]
    public void CurrentUser_WhenNotAuthenticated_ReturnsNull()
    {
        _authService.CurrentUser.Should().BeNull();
        _authService.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsTrue()
    {
        var loginRequest = new LoginRequest { Email = "testuser@test.com", Password = "password123" };
        var loginResponse = new { Token = "jwt-token-here", UserId = Guid.NewGuid(), Username = "testuser", Role = "Client" };

        _mockHttp.When("http://localhost:5298/api/auth/login")
            .Respond("application/json", JsonSerializer.Serialize(loginResponse));

        var result = await _authService.LoginAsync(loginRequest);

        result.Should().BeTrue();
        _authService.IsAuthenticated.Should().BeTrue();
        _authService.CurrentUser!.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ReturnsFalse()
    {
        var loginRequest = new LoginRequest { Email = "wrong@test.com", Password = "wrongpass" };
        _mockHttp.When("http://localhost:5298/api/auth/login").Respond(HttpStatusCode.Unauthorized);

        var result = await _authService.LoginAsync(loginRequest);

        result.Should().BeFalse();
        _authService.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ReturnsSuccess()
    {
        var registerRequest = new RegisterRequest { Username = "newuser", Email = "new@test.com", Password = "password123", ConfirmPassword = "password123" };
        _mockHttp.When("http://localhost:5298/api/auth/register").Respond(HttpStatusCode.OK);

        var result = await _authService.RegisterAsync(registerRequest);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidData_ReturnsError()
    {
        var registerRequest = new RegisterRequest { Username = "existinguser", Email = "existing@test.com", Password = "pass", ConfirmPassword = "pass" };
        _mockHttp.When("http://localhost:5298/api/auth/register").Respond(HttpStatusCode.BadRequest, "application/json", "\"User already exists\"");

        var result = await _authService.RegisterAsync(registerRequest);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeEmpty();
    }

    [Fact]
    public async Task LogoutAsync_ClearsCurrentUser()
    {
        var loginResponse = new { Token = "jwt-token-here", UserId = Guid.NewGuid(), Username = "testuser", Role = "Client" };
        _mockHttp.When("http://localhost:5298/api/auth/login").Respond("application/json", JsonSerializer.Serialize(loginResponse));
        _mockHttp.When("http://localhost:5298/api/auth/logout").Respond(HttpStatusCode.OK);

        await _authService.LoginAsync(new LoginRequest { Email = "test@test.com", Password = "test" });
        _authService.IsAuthenticated.Should().BeTrue();

        await _authService.LogoutAsync();

        _authService.IsAuthenticated.Should().BeFalse();
        _authService.CurrentUser.Should().BeNull();
    }

    [Fact]
    public async Task InitializeAsync_WithStoredToken_RestoresSession()
    {
        var userId = Guid.NewGuid();
        _mockJs.Setup(x => x.InvokeAsync<string?>("localStorage.getItem", new object[] { "authToken" })).ReturnsAsync("stored-token");
        _mockJs.Setup(x => x.InvokeAsync<string?>("localStorage.getItem", new object[] { "userId" })).ReturnsAsync(userId.ToString());
        _mockJs.Setup(x => x.InvokeAsync<string?>("localStorage.getItem", new object[] { "username" })).ReturnsAsync("storeduser");
        _mockJs.Setup(x => x.InvokeAsync<string?>("localStorage.getItem", new object[] { "role" })).ReturnsAsync("Client");

        await _authService.InitializeAsync();

        _authService.IsAuthenticated.Should().BeTrue();
        _authService.CurrentUser!.Username.Should().Be("storeduser");
    }

    [Fact]
    public async Task InitializeAsync_WithNoStoredToken_RemainsUnauthenticated()
    {
        _mockJs.Setup(x => x.InvokeAsync<string?>("localStorage.getItem", new object[] { "authToken" })).ReturnsAsync((string?)null);

        await _authService.InitializeAsync();

        _authService.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public async Task ChangePasswordAsync_WithValidData_ReturnsSuccess()
    {
        var request = new ChangePasswordRequest { CurrentPassword = "oldpass", NewPassword = "newpass123" };
        _mockHttp.When("http://localhost:5298/api/auth/change-password").Respond(HttpStatusCode.OK);

        var (success, errorMessage) = await _authService.ChangePasswordAsync(request);

        success.Should().BeTrue();
        errorMessage.Should().BeNull();
    }

    [Fact]
    public async Task ChangePasswordAsync_WithInvalidPassword_ReturnsError()
    {
        var request = new ChangePasswordRequest { CurrentPassword = "wrongpass", NewPassword = "newpass123" };
        _mockHttp.When("http://localhost:5298/api/auth/change-password").Respond(HttpStatusCode.BadRequest, "application/json", "\"Invalid current password\"");

        var (success, errorMessage) = await _authService.ChangePasswordAsync(request);

        success.Should().BeFalse();
        errorMessage.Should().NotBeEmpty();
    }
}
