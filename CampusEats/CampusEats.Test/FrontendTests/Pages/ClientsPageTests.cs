using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace CampusEats.Test.FrontendTests.Pages;

public class ClientsPageTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public ClientsPageTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5298/");
        
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        
        // Clients page injects: UserService, OrderService, AuthService
        var authService = new AuthService(httpClient, mockJsRuntime.Object);
        var userService = new UserService(httpClient);
        var orderService = new OrderService(httpClient);
        
        Services.AddSingleton(authService);
        Services.AddSingleton(userService);
        Services.AddSingleton(orderService);
    }

    [Fact]
    public void ClientsPage_RendersTitle()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Clients>();
        
        cut.Markup.Should().Contain("Gestiunea Utilizatorilor");
    }

    [Fact]
    public void ClientsPage_HasContainer()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Clients>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void ClientsPage_RendersMarkup()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Clients>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void ClientsPage_HasMarginTop()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Clients>();
        
        cut.Markup.Should().Contain("mt-4");
    }

    [Fact]
    public void ClientsPage_HasMarginBottom()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Clients>();
        
        cut.Markup.Should().Contain("mb-4");
    }

    [Fact]
    public void ClientsPage_HasHeading()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Clients>();
        
        cut.Markup.Should().Contain("h3");
    }

    [Fact]
    public void ClientsPage_ShowsLoadingSpinner()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Clients>();
        
        cut.Markup.Should().Contain("spinner-border");
    }

    [Fact]
    public void ClientsPage_HasTextCenter()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Clients>();
        
        cut.Markup.Should().Contain("text-center");
    }

    [Fact]
    public void ClientsPage_HasVisuallyHidden()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Clients>();
        
        cut.Markup.Should().Contain("visually-hidden");
    }

    [Fact]
    public void ClientsPage_HasRoleStatus()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Clients>();
        
        cut.Markup.Should().Contain("role=\"status\"");
    }

    [Fact]
    public void ClientsPage_HasDivElements()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Clients>();
        
        cut.FindAll("div").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void ClientsPage_HasH3Element()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Clients>();
        
        cut.Find("h3").Should().NotBeNull();
    }

    [Fact]
    public void ClientsPage_HasContainerClass()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Clients>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void ClientsPage_HasSpinnerBorderClass()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Clients>();
        
        cut.Find("div.spinner-border").Should().NotBeNull();
    }

    [Fact]
    public void ClientsPage_HasTextCenterClass()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Clients>();
        
        cut.Find("div.text-center").Should().NotBeNull();
    }

    [Fact]
    public void ClientsPage_HasSpanElements()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Clients>();
        
        cut.FindAll("span").Should().HaveCountGreaterThan(0);
    }
}
