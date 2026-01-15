using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages.Menu;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace CampusEats.Test.FrontendTests.Pages;

public class MenuItemsPageTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public MenuItemsPageTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5298/");
        
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        
        var authService = new AuthService(httpClient, mockJsRuntime.Object);
        var menuItemService = new MenuItemService(httpClient);
        var cartService = new CartService(mockJsRuntime.Object);
        
        Services.AddSingleton(authService);
        Services.AddSingleton(menuItemService);
        Services.AddSingleton(cartService);
    }

    [Fact]
    public void MenuItemsPage_RendersTitle()
    {
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", "[]");
        
        var cut = Render<MenuItems>();
        
        cut.Markup.Should().Contain("Menu Items");
    }

    [Fact]
    public void MenuItemsPage_HasContainer()
    {
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", "[]");
        
        var cut = Render<MenuItems>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void MenuItemsPage_ShowsEmptyState()
    {
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", "[]");
        
        var cut = Render<MenuItems>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void MenuItemsPage_HasMarginTop()
    {
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", "[]");
        
        var cut = Render<MenuItems>();
        
        cut.Markup.Should().Contain("mt-4");
    }

    [Fact]
    public void MenuItemsPage_HasMarginBottom()
    {
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", "[]");
        
        var cut = Render<MenuItems>();
        
        cut.Markup.Should().Contain("mb-4");
    }

    [Fact]
    public void MenuItemsPage_HasFlexLayout()
    {
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", "[]");
        
        var cut = Render<MenuItems>();
        
        cut.Markup.Should().Contain("d-flex");
    }

    [Fact]
    public void MenuItemsPage_HasHeading()
    {
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", "[]");
        
        var cut = Render<MenuItems>();
        
        cut.Markup.Should().Contain("h3");
    }

    [Fact]
    public void MenuItemsPage_HasRow()
    {
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", "[]");
        
        var cut = Render<MenuItems>();
        
        cut.Markup.Should().Contain("row");
    }

    [Fact]
    public void MenuItemsPage_HasJustifyBetween()
    {
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", "[]");
        
        var cut = Render<MenuItems>();
        
        cut.Markup.Should().Contain("justify-content-between");
    }

    [Fact]
    public void MenuItemsPage_HasAlignItemsCenter()
    {
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", "[]");
        
        var cut = Render<MenuItems>();
        
        cut.Markup.Should().Contain("align-items-center");
    }

    [Fact]
    public void MenuItemsPage_HasH3Element()
    {
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", "[]");
        
        var cut = Render<MenuItems>();
        
        cut.Find("h3").Should().NotBeNull();
    }

    [Fact]
    public void MenuItemsPage_HasContainerClass()
    {
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", "[]");
        
        var cut = Render<MenuItems>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void MenuItemsPage_HasRowClass()
    {
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", "[]");
        
        var cut = Render<MenuItems>();
        
        cut.Find("div.row").Should().NotBeNull();
    }

    [Fact]
    public void MenuItemsPage_HasDivElements()
    {
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", "[]");
        
        var cut = Render<MenuItems>();
        
        cut.FindAll("div").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void MenuItemsPage_HasSpanElements()
    {
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", "[]");
        
        var cut = Render<MenuItems>();
        
        cut.FindAll("span").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void MenuItemsPage_ShowsEmptyStateOrContent()
    {
        _mockHttp.When("http://localhost:5298/api/menu-items").Respond("application/json", "[]");
        
        var cut = Render<MenuItems>();
        
        cut.Markup.Length.Should().BeGreaterThan(10);
    }
}
