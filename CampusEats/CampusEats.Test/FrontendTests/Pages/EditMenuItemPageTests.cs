using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages.Menu;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace CampusEats.Test.FrontendTests.Pages;

public class EditMenuItemPageTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public EditMenuItemPageTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5298/");
        
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        
        var authService = new AuthService(httpClient, mockJsRuntime.Object);
        var menuItemService = new MenuItemService(httpClient);
        
        Services.AddSingleton(authService);
        Services.AddSingleton(menuItemService);
    }

    [Fact]
    public void EditMenuItemPage_HasContainer()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenuItem>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void EditMenuItemPage_ShowsLoadingOrContent()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenuItem>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void EditMenuItemPage_HasCard()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenuItem>();
        
        cut.Markup.Should().Contain("card");
    }

    [Fact]
    public void EditMenuItemPage_HasShadow()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenuItem>();
        
        cut.Markup.Should().Contain("shadow");
    }

    [Fact]
    public void EditMenuItemPage_HasCardBody()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenuItem>();
        
        cut.Markup.Should().Contain("card-body");
    }

    [Fact]
    public void EditMenuItemPage_HasJustifyCenter()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenuItem>();
        
        cut.Markup.Should().Contain("justify-content-center");
    }

    [Fact]
    public void EditMenuItemPage_HasRow()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenuItem>();
        
        cut.Markup.Should().Contain("row");
    }

    [Fact]
    public void EditMenuItemPage_HasMarginTop()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenuItem>();
        
        cut.Markup.Should().Contain("mt-4");
    }

    [Fact]
    public void EditMenuItemPage_HasMb4()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenuItem>();
        
        cut.Markup.Should().Contain("mb-4");
    }

    [Fact]
    public void EditMenuItemPage_HasCol()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenuItem>();
        
        cut.Markup.Should().Contain("col");
    }

    [Fact]
    public void EditMenuItemPage_HasDivElements()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenuItem>();
        
        cut.FindAll("div").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void EditMenuItemPage_HasContainerClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenuItem>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void EditMenuItemPage_HasRowClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenuItem>();
        
        cut.Find("div.row").Should().NotBeNull();
    }

    [Fact]
    public void EditMenuItemPage_HasCardClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenuItem>();
        
        cut.Find("div.card").Should().NotBeNull();
    }

    [Fact]
    public void EditMenuItemPage_HasCardBodyClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenuItem>();
        
        cut.Find("div.card-body").Should().NotBeNull();
    }

    [Fact]
    public void EditMenuItemPage_RendersNotEmpty()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<EditMenuItem>();
        
        cut.Markup.Should().NotBeEmpty();
    }
}
