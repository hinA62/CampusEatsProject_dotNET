using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages.Payments;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;
using System.Text.Json;
using CampusEatsFrontend.Models.Loyalty;

namespace CampusEats.Test.FrontendTests.Pages;

public class PaymentPageTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public PaymentPageTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5298/");
        
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        
        var authService = new AuthService(httpClient, mockJsRuntime.Object);
        var paymentService = new PaymentService(httpClient);
        var orderService = new OrderService(httpClient);
        var loyaltyService = new LoyaltyService(httpClient);
        var menuService = new MenuService(httpClient);
        var menuItemService = new MenuItemService(httpClient);
        
        Services.AddSingleton(authService);
        Services.AddSingleton(paymentService);
        Services.AddSingleton(orderService);
        Services.AddSingleton(loyaltyService);
        Services.AddSingleton(menuService);
        Services.AddSingleton(menuItemService);
    }

    [Fact]
    public void PaymentPage_HasContainer()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void PaymentPage_RendersMarkup()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void PaymentPage_ShowsLoadingSpinner()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        // Initially shows loading spinner
        cut.Markup.Should().Contain("spinner-border");
    }

    [Fact]
    public void PaymentPage_HasPaymentPageClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        cut.Markup.Should().Contain("payment-page");
    }

    [Fact]
    public void PaymentPage_HasRowLayout()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        cut.Markup.Should().Contain("row");
    }

    [Fact]
    public void PaymentPage_HasJustifyCenterLayout()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        cut.Markup.Should().Contain("justify-content-center");
    }

    [Fact]
    public void PaymentPage_HasVisuallyHidden()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        cut.Markup.Should().Contain("visually-hidden");
    }

    [Fact]
    public void PaymentPage_HasTextCenter()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        cut.Markup.Should().Contain("text-center");
    }

    [Fact]
    public void PaymentPage_HasColMd8()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        cut.Markup.Should().Contain("col-md-8");
    }

    [Fact]
    public void PaymentPage_HasMt4()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        cut.Markup.Should().Contain("mt-4");
    }

    [Fact]
    public void PaymentPage_HasDivElements()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        cut.FindAll("div").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void PaymentPage_HasSpanElement()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        cut.FindAll("span").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void PaymentPage_HasRoleStatus()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        cut.Markup.Should().Contain("role=\"status\"");
    }

    [Fact]
    public void PaymentPage_HasLoadingText()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        cut.Markup.Should().Contain("Se încarccă...");
    }

    [Fact]
    public void PaymentPage_InitiallyShowsLoader()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        // Loader should be present at init
        cut.Find("div.spinner-border").Should().NotBeNull();
    }

    [Fact]
    public void PaymentPage_HasContainerClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void PaymentPage_HasRowElement()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        cut.Find("div.row").Should().NotBeNull();
    }

    [Fact]
    public void PaymentPage_HasColElement()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Payment>();
        
        cut.FindAll("div[class*='col']").Should().HaveCountGreaterThan(0);
    }
}
