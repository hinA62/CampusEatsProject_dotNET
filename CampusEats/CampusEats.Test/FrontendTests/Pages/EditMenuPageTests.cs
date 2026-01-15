using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages.Menu;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace CampusEats.Test.FrontendTests.Pages;

public class EditMenuPageTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public EditMenuPageTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5298/");
        
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        
        var authService = new AuthService(httpClient, mockJsRuntime.Object);
        var menuService = new MenuService(httpClient);
        var menuItemService = new MenuItemService(httpClient);
        
        Services.AddSingleton(authService);
        Services.AddSingleton(menuService);
        Services.AddSingleton(menuItemService);
    }

    [Fact]
    public void EditMenuPage_HasContainer()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenu>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void EditMenuPage_ShowsLoadingOrContent()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenu>();
        
        // Shows either loading spinner or error for missing menu
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void EditMenuPage_HasCard()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenu>();
        
        cut.Markup.Should().Contain("card");
    }

    [Fact]
    public void EditMenuPage_HasShadow()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenu>();
        
        cut.Markup.Should().Contain("shadow");
    }

    [Fact]
    public void EditMenuPage_HasCardBody()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenu>();
        
        cut.Markup.Should().Contain("card-body");
    }

    [Fact]
    public void EditMenuPage_HasJustifyCenter()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenu>();
        
        cut.Markup.Should().Contain("justify-content-center");
    }

    [Fact]
    public void EditMenuPage_HasRow()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenu>();
        
        cut.Markup.Should().Contain("row");
    }

    [Fact]
    public void EditMenuPage_HasMarginTop()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenu>();
        
        cut.Markup.Should().Contain("mt-4");
    }

    [Fact]
    public void EditMenuPage_HasMb4()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenu>();
        
        cut.Markup.Should().Contain("mb-4");
    }

    [Fact]
    public void EditMenuPage_HasDivElements()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenu>();
        
        cut.FindAll("div").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void EditMenuPage_HasContainerClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenu>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void EditMenuPage_HasRowClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenu>();
        
        cut.Find("div.row").Should().NotBeNull();
    }

    [Fact]
    public void EditMenuPage_HasCardClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenu>();
        
        cut.Find("div.card").Should().NotBeNull();
    }

    [Fact]
    public void EditMenuPage_HasCardBodyClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenu>();
        
        cut.Find("div.card-body").Should().NotBeNull();
    }

    [Fact]
    public void EditMenuPage_HasCol()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenu>();
        
        cut.Markup.Should().Contain("col");
    }

    [Fact]
    public void EditMenuPage_RendersNotEmpty()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenu>();
        
        cut.Markup.Should().NotBeEmpty();
    }
}
