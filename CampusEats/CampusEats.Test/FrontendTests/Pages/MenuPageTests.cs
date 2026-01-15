using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages.Menu;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace CampusEats.Test.FrontendTests.Pages;

public class MenuPageTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public MenuPageTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5298/");
        
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        
        // Menu page injects: MenuService, MenuItemService, CartService, AuthService
        var authService = new AuthService(httpClient, mockJsRuntime.Object);
        var menuService = new MenuService(httpClient);
        var menuItemService = new MenuItemService(httpClient);
        var cartService = new CartService(mockJsRuntime.Object);
        
        Services.AddSingleton(authService);
        Services.AddSingleton(menuService);
        Services.AddSingleton(menuItemService);
        Services.AddSingleton(cartService);
    }

    [Fact]
    public void MenuPage_RendersTitle()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Menu>();
        
        cut.Markup.Should().Contain("Meniuri");
    }

    [Fact]
    public void MenuPage_HasContainer()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Menu>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void MenuPage_RendersMarkup()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Menu>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void MenuPage_HasMarginTop()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Menu>();
        
        cut.Markup.Should().Contain("mt-4");
    }

    [Fact]
    public void MenuPage_HasMarginBottom()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Menu>();
        
        cut.Markup.Should().Contain("mb-4");
    }

    [Fact]
    public void MenuPage_HasFlexLayout()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Menu>();
        
        cut.Markup.Should().Contain("d-flex");
    }

    [Fact]
    public void MenuPage_HasJustifyBetween()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Menu>();
        
        cut.Markup.Should().Contain("justify-content-between");
    }

    [Fact]
    public void MenuPage_HasHeading()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Menu>();
        
        cut.Markup.Should().Contain("h3");
    }

    [Fact]
    public void MenuPage_HasAlignItemsCenter()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Menu>();
        
        cut.Markup.Should().Contain("align-items-center");
    }

    [Fact]
    public void MenuPage_HasDivElements()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Menu>();
        
        cut.FindAll("div").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void MenuPage_HasSpanElements()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Menu>();
        
        cut.FindAll("span").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void MenuPage_HasH3Element()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Menu>();
        
        cut.Find("h3").Should().NotBeNull();
    }

    [Fact]
    public void MenuPage_HasContainerClass()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Menu>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void MenuPage_RendersNotEmpty()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Menu>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void MenuPage_ShowsEmptyStateOrContent()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Menu>();
        
        // May show loading or content
        cut.Markup.Length.Should().BeGreaterThan(10);
    }
}
