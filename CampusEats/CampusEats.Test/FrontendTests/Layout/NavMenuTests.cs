using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Layout;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace CampusEats.Test.FrontendTests.Layout;

public class NavMenuTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public NavMenuTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5298/");
        
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        
        var authService = new AuthService(httpClient, mockJsRuntime.Object);
        var cartService = new CartService(mockJsRuntime.Object);
        
        Services.AddSingleton(authService);
        Services.AddSingleton(cartService);
    }

    [Fact]
    public void NavMenu_RendersBrandName()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Markup.Should().Contain("CampusEats");
    }

    [Fact]
    public void NavMenu_RendersNavbar()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Markup.Should().Contain("navbar");
    }

    [Fact]
    public void NavMenu_HasHomeLink()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Markup.Should().Contain("Home");
    }

    [Fact]
    public void NavMenu_HasNavigationToggle()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Markup.Should().Contain("navbar-toggler");
    }

    [Fact]
    public void NavMenu_HasTopRow()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Markup.Should().Contain("top-row");
    }

    [Fact]
    public void NavMenu_HasNavbarDark()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Markup.Should().Contain("navbar-dark");
    }

    [Fact]
    public void NavMenu_HasContainerFluid()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Markup.Should().Contain("container-fluid");
    }

    [Fact]
    public void NavMenu_HasNavScrollable()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Markup.Should().Contain("nav-scrollable");
    }

    [Fact]
    public void NavMenu_HasNavLink()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Markup.Should().Contain("nav-link");
    }

    [Fact]
    public void NavMenu_HasNavItem()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Markup.Should().Contain("nav-item");
    }

    [Fact]
    public void NavMenu_HasHomeIcon()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Markup.Should().Contain("bi-house-door-fill-nav-menu");
    }

    [Fact]
    public void NavMenu_HasSvgLogo()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Markup.Should().Contain("svg");
    }

    [Fact]
    public void NavMenu_HasPx3()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Markup.Should().Contain("px-3");
    }

    [Fact]
    public void NavMenu_HasNavElement()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Find("nav").Should().NotBeNull();
    }

    [Fact]
    public void NavMenu_HasDivElements()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.FindAll("div").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void NavMenu_HasAElements()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.FindAll("a").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void NavMenu_HasButtonElement()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Find("button").Should().NotBeNull();
    }

    [Fact]
    public void NavMenu_HasNavbarTogglerClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Find("button.navbar-toggler").Should().NotBeNull();
    }

    [Fact]
    public void NavMenu_HasBrandText()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Markup.Should().Contain("CampusEats");
    }

    [Fact]
    public void NavMenu_HasTopRowClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Find("div.top-row").Should().NotBeNull();
    }

    [Fact]
    public void NavMenu_HasNavElement2()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.FindAll("nav").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void NavMenu_HasFlexColumn()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Markup.Should().Contain("flex-column");
    }

    [Fact]
    public void NavMenu_HasSpanElements()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.FindAll("span").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void NavMenu_RendersNotEmpty()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<NavMenu>();
        
        cut.Markup.Should().NotBeEmpty();
    }
}
