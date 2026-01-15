using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages.Menu;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace CampusEats.Test.FrontendTests.Pages;

public class CreateMenuPageTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public CreateMenuPageTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5298/");
        
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        
        // CreateMenu page injects: MenuService, MenuItemService, AuthService
        var authService = new AuthService(httpClient, mockJsRuntime.Object);
        var menuService = new MenuService(httpClient);
        var menuItemService = new MenuItemService(httpClient);
        
        Services.AddSingleton(authService);
        Services.AddSingleton(menuService);
        Services.AddSingleton(menuItemService);
    }

    [Fact]
    public void CreateMenuPage_HasContainer()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<CreateMenu>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void CreateMenuPage_RendersMarkup()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<CreateMenu>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void CreateMenuPage_HasCard()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<CreateMenu>();
        
        cut.Markup.Should().Contain("card");
    }

    [Fact]
    public void CreateMenuPage_HasShadow()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<CreateMenu>();
        
        cut.Markup.Should().Contain("shadow");
    }

    [Fact]
    public void CreateMenuPage_HasCardBody()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<CreateMenu>();
        
        cut.Markup.Should().Contain("card-body");
    }

    [Fact]
    public void CreateMenuPage_HasRow()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<CreateMenu>();
        
        cut.Markup.Should().Contain("row");
    }

    [Fact]
    public void CreateMenuPage_HasMarginTop()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<CreateMenu>();
        
        cut.Markup.Should().Contain("mt-4");
    }

    [Fact]
    public void CreateMenuPage_HasMb4()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<CreateMenu>();
        
        cut.Markup.Should().Contain("mb-4");
    }

    [Fact]
    public void CreateMenuPage_HasJustifyCenter()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<CreateMenu>();
        
        cut.Markup.Should().Contain("justify-content-center");
    }

    [Fact]
    public void CreateMenuPage_HasCol()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<CreateMenu>();
        
        cut.Markup.Should().Contain("col");
    }

    [Fact]
    public void CreateMenuPage_HasContainerClass()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<CreateMenu>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void CreateMenuPage_HasRowClass()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<CreateMenu>();
        
        cut.Find("div.row").Should().NotBeNull();
    }

    [Fact]
    public void CreateMenuPage_HasCardClass()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<CreateMenu>();
        
        cut.Find("div.card").Should().NotBeNull();
    }

    [Fact]
    public void CreateMenuPage_HasCardBodyClass()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<CreateMenu>();
        
        cut.Find("div.card-body").Should().NotBeNull();
    }

    [Fact]
    public void CreateMenuPage_RendersNotEmpty()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<CreateMenu>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void CreateMenuPage_HasDivElements()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<CreateMenu>();
        
        cut.FindAll("div").Should().HaveCountGreaterThan(0);
    }
}
