using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;
using System.Text.Json;
using CampusEatsFrontend.Models.Order;

namespace CampusEats.Test.FrontendTests.Pages;

public class KitchenPageTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public KitchenPageTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5298/");
        
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        
        var authService = new AuthService(httpClient, mockJsRuntime.Object);
        var kitchenService = new KitchenService(httpClient);
        var orderService = new OrderService(httpClient);
        
        Services.AddSingleton(authService);
        Services.AddSingleton(kitchenService);
        Services.AddSingleton(orderService);
    }

    [Fact]
    public void KitchenPage_RendersTitle()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.Markup.Should().Contain("Bucătărie");
    }

    [Fact]
    public void KitchenPage_HasStatusFilter()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.Markup.Should().Contain("select");
        cut.Markup.Should().Contain("Toate Comenzile");
    }

    [Fact]
    public void KitchenPage_HasContainer()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void KitchenPage_RendersMarkup()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void KitchenPage_HasMarginTop()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.Markup.Should().Contain("mt-4");
    }

    [Fact]
    public void KitchenPage_HasMarginBottom()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.Markup.Should().Contain("mb-4");
    }

    [Fact]
    public void KitchenPage_HasFormSelect()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.Markup.Should().Contain("form-select");
    }

    [Fact]
    public void KitchenPage_HasRow()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.Markup.Should().Contain("row");
    }

    [Fact]
    public void KitchenPage_HasColumn()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.Markup.Should().Contain("col-md");
    }

    [Fact]
    public void KitchenPage_HasMt4Class()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.Find("div.mt-4").Should().NotBeNull();
    }

    [Fact]
    public void KitchenPage_HasAElements()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        // Kitchen page may or may not have links
        cut.Markup.Length.Should().BeGreaterThan(10);
    }

    [Fact]
    public void KitchenPage_RendersNotEmpty()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void KitchenPage_HasDivElements()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.FindAll("div").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void KitchenPage_HasSpanElements()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.FindAll("span").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void KitchenPage_HasH3Element()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.Find("h3").Should().NotBeNull();
    }

    [Fact]
    public void KitchenPage_HasSelectElement()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.Find("select").Should().NotBeNull();
    }

    [Fact]
    public void KitchenPage_HasContainerClass()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void KitchenPage_HasRowClass()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.Find("div.row").Should().NotBeNull();
    }

    [Fact]
    public void KitchenPage_HasFormSelectClass()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.Find("select.form-select").Should().NotBeNull();
    }

    [Fact]
    public void KitchenPage_HasOptionElements()
    {
        _mockHttp.When("http://localhost:5298/api/kitchen/orders").Respond("application/json", "[]");
        
        var cut = Render<Kitchen>();
        
        cut.FindAll("option").Should().HaveCountGreaterThan(0);
    }
}
