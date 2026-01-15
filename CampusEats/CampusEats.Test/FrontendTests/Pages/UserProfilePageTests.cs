using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace CampusEats.Test.FrontendTests.Pages;

public class UserProfilePageTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public UserProfilePageTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5298/");
        
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        
        var authService = new AuthService(httpClient, mockJsRuntime.Object);
        var userService = new UserService(httpClient);
        var orderService = new OrderService(httpClient);
        
        Services.AddSingleton(authService);
        Services.AddSingleton(userService);
        Services.AddSingleton(orderService);
    }

    [Fact]
    public void UserProfilePage_HasContainer()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<UserProfile>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void UserProfilePage_ShowsLoadingOrContent()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<UserProfile>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void UserProfilePage_RendersMarkup()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<UserProfile>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void UserProfilePage_ShowsLoadingSpinner()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<UserProfile>();
        
        cut.Markup.Should().Contain("spinner-border");
    }

    [Fact]
    public void UserProfilePage_HasMarginTop()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<UserProfile>();
        
        cut.Markup.Should().Contain("mt-4");
    }

    [Fact]
    public void UserProfilePage_HasTextCenter()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<UserProfile>();
        
        cut.Markup.Should().Contain("text-center");
    }

    [Fact]
    public void UserProfilePage_HasVisuallyHiddenText()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<UserProfile>();
        
        cut.Markup.Should().Contain("visually-hidden");
    }

    [Fact]
    public void UserProfilePage_ShowsLoadingText()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<UserProfile>();
        
        cut.Markup.Should().Contain("Se încarcă");
    }

    [Fact]
    public void UserProfilePage_HasRoleStatus()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<UserProfile>();
        
        cut.Markup.Should().Contain("role=\"status\"");
    }

    [Fact]
    public void UserProfilePage_HasDivElements()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<UserProfile>();
        
        cut.FindAll("div").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void UserProfilePage_HasSpanElements()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<UserProfile>();
        
        cut.FindAll("span").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void UserProfilePage_HasContainerClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<UserProfile>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void UserProfilePage_HasSpinnerBorderClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<UserProfile>();
        
        cut.Find("div.spinner-border").Should().NotBeNull();
    }

    [Fact]
    public void UserProfilePage_HasTextCenterClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<UserProfile>();
        
        cut.Find("div.text-center").Should().NotBeNull();
    }

    [Fact]
    public void UserProfilePage_HasMt4Class()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<UserProfile>();
        
        cut.Find("div.mt-4").Should().NotBeNull();
    }
}
