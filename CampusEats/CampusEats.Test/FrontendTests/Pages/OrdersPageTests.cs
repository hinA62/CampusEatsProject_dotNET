using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages.Orders;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace CampusEats.Test.FrontendTests.Pages;

public class OrdersPageTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public OrdersPageTests()
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
    public void OrdersPage_RendersTitle()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Orders>();
        
        cut.Markup.Should().Contain("Orders");
    }

    [Fact]
    public void OrdersPage_HasContainer()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Orders>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void OrdersPage_ShowsContentOrLoading()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Orders>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void OrdersPage_HasMarginTop()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Orders>();
        
        cut.Markup.Should().Contain("mt-4");
    }

    [Fact]
    public void OrdersPage_HasMarginBottom()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Orders>();
        
        cut.Markup.Should().Contain("mb-3");
    }

    [Fact]
    public void OrdersPage_HasHeading()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Orders>();
        
        cut.Markup.Should().Contain("h3");
    }

    [Fact]
    public void OrdersPage_HasRow()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Orders>();
        
        cut.Markup.Should().Contain("row");
    }

    [Fact]
    public void OrdersPage_HasFormSelect()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Orders>();
        
        cut.Markup.Should().Contain("form-select");
    }

    [Fact]
    public void OrdersPage_HasDivElements()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Orders>();
        
        cut.FindAll("div").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void OrdersPage_RendersNotEmpty()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Orders>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void OrdersPage_HasMarkupLength()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Orders>();
        
        cut.Markup.Length.Should().BeGreaterThan(10);
    }

    [Fact]
    public void OrdersPage_HasH3Element()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Orders>();
        
        cut.Find("h3").Should().NotBeNull();
    }

    [Fact]
    public void OrdersPage_HasContainerClass()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Orders>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void OrdersPage_HasRowClass()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Orders>();
        
        cut.Find("div.row").Should().NotBeNull();
    }

    [Fact]
    public void OrdersPage_HasSelectElement()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Orders>();
        
        cut.Find("select").Should().NotBeNull();
    }

    [Fact]
    public void OrdersPage_HasFormSelectClass()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Orders>();
        
        cut.Find("select.form-select").Should().NotBeNull();
    }

    [Fact]
    public void OrdersPage_HasOptionElements()
    {
        _mockHttp.When("*").Respond("application/json", "[]");
        
        var cut = Render<Orders>();
        
        cut.FindAll("option").Should().HaveCountGreaterThan(0);
    }
}
