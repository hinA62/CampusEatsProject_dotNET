using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages.Orders;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace CampusEats.Test.FrontendTests.Pages;

public class PlaceOrderPageTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public PlaceOrderPageTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5298/");
        
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        
        var authService = new AuthService(httpClient, mockJsRuntime.Object);
        var menuService = new MenuService(httpClient);
        var menuItemService = new MenuItemService(httpClient);
        var orderService = new OrderService(httpClient);
        
        Services.AddSingleton(authService);
        Services.AddSingleton(menuService);
        Services.AddSingleton(menuItemService);
        Services.AddSingleton(orderService);
    }

    [Fact]
    public void PlaceOrderPage_RendersTitle()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.Markup.Should().Contain("Plasează comanda");
    }

    [Fact]
    public void PlaceOrderPage_HasContainer()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void PlaceOrderPage_RendersMarkup()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void PlaceOrderPage_HasCartIcon()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.Markup.Should().Contain("bi-cart-plus");
    }

    [Fact]
    public void PlaceOrderPage_ShowsLoadingSpinner()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.Markup.Should().Contain("spinner-border");
    }

    [Fact]
    public void PlaceOrderPage_HasMarginTop()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.Markup.Should().Contain("mt-4");
    }

    [Fact]
    public void PlaceOrderPage_HasMarginBottom()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.Markup.Should().Contain("mb-4");
    }

    [Fact]
    public void PlaceOrderPage_HasHeading()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.Markup.Should().Contain("h3");
    }

    [Fact]
    public void PlaceOrderPage_HasTextCenter()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.Markup.Should().Contain("text-center");
    }

    [Fact]
    public void PlaceOrderPage_HasVisuallyHidden()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.Markup.Should().Contain("visually-hidden");
    }

    [Fact]
    public void PlaceOrderPage_HasRoleStatus()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.Markup.Should().Contain("role=\"status\"");
    }

    [Fact]
    public void PlaceOrderPage_HasLoadingText()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.Markup.Should().Contain("Se încarcă...");
    }

    [Fact]
    public void PlaceOrderPage_HasDivElements()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.FindAll("div").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void PlaceOrderPage_HasSpanElements()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.FindAll("span").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void PlaceOrderPage_HasH3Element()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.Find("h3").Should().NotBeNull();
    }

    [Fact]
    public void PlaceOrderPage_HasContainerClass()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void PlaceOrderPage_HasSpinnerBorderClass()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.Find("div.spinner-border").Should().NotBeNull();
    }

    [Fact]
    public void PlaceOrderPage_HasBootstrapIcon()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<PlaceOrder>();
        
        cut.FindAll("span[class*='bi-']").Should().HaveCountGreaterThan(0);
    }
}
