using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages.Orders;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace CampusEats.Test.FrontendTests.Pages;

public class OrderDetailsPageTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public OrderDetailsPageTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5298/");
        
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        
        var authService = new AuthService(httpClient, mockJsRuntime.Object);
        var orderService = new OrderService(httpClient);
        
        Services.AddSingleton(authService);
        Services.AddSingleton(orderService);
    }

    [Fact]
    public void OrderDetailsPage_HasContainer()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<OrderDetails>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void OrderDetailsPage_ShowsLoadingOrContent()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<OrderDetails>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void OrderDetailsPage_RendersMarkup()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<OrderDetails>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void OrderDetailsPage_ShowsLoadingSpinner()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<OrderDetails>();
        
        cut.Markup.Should().Contain("spinner-border");
    }

    [Fact]
    public void OrderDetailsPage_HasMarginTop()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<OrderDetails>();
        
        cut.Markup.Should().Contain("mt-4");
    }

    [Fact]
    public void OrderDetailsPage_HasTextCenter()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<OrderDetails>();
        
        cut.Markup.Should().Contain("text-center");
    }

    [Fact]
    public void OrderDetailsPage_HasVisuallyHiddenText()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<OrderDetails>();
        
        cut.Markup.Should().Contain("visually-hidden");
    }

    [Fact]
    public void OrderDetailsPage_ShowsLoadingText()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<OrderDetails>();
        
        cut.Markup.Should().Contain("Se încarcă");
    }

    [Fact]
    public void OrderDetailsPage_HasRoleStatus()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<OrderDetails>();
        
        cut.Markup.Should().Contain("role=\"status\"");
    }

    [Fact]
    public void OrderDetailsPage_HasDivElements()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<OrderDetails>();
        
        cut.FindAll("div").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void OrderDetailsPage_HasSpanElements()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<OrderDetails>();
        
        cut.FindAll("span").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void OrderDetailsPage_HasContainerClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<OrderDetails>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void OrderDetailsPage_HasSpinnerBorderClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<OrderDetails>();
        
        cut.Find("div.spinner-border").Should().NotBeNull();
    }

    [Fact]
    public void OrderDetailsPage_HasTextCenterClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<OrderDetails>();
        
        cut.Find("div.text-center").Should().NotBeNull();
    }

    [Fact]
    public void OrderDetailsPage_HasMt4Class()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<OrderDetails>();
        
        cut.Find("div.mt-4").Should().NotBeNull();
    }

    [Fact]
    public void OrderDetailsPage_HasVisuallyHiddenClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<OrderDetails>();
        
        cut.Find("span.visually-hidden").Should().NotBeNull();
    }
}
