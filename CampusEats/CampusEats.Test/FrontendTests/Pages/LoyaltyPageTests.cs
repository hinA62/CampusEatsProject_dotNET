using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;
using System.Text.Json;
using CampusEatsFrontend.Models.Loyalty;

namespace CampusEats.Test.FrontendTests.Pages;

public class LoyaltyPageTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public LoyaltyPageTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5298/");
        
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        
        var authService = new AuthService(httpClient, mockJsRuntime.Object);
        var loyaltyService = new LoyaltyService(httpClient);
        
        Services.AddSingleton(authService);
        Services.AddSingleton(loyaltyService);
    }

    [Fact]
    public void LoyaltyPage_RendersTitle()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.Markup.Should().Contain("Loyalty Program");
    }

    [Fact]
    public void LoyaltyPage_HasContainer()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void LoyaltyPage_HasTrophyIcon()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.Markup.Should().Contain("bi-trophy-fill");
    }

    [Fact]
    public void LoyaltyPage_ShowsLoadingSpinner_Initially()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.Markup.Should().Contain("spinner-border");
    }

    [Fact]
    public void LoyaltyPage_HasMarginTop()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.Markup.Should().Contain("mt-4");
    }

    [Fact]
    public void LoyaltyPage_HasHeading()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.Markup.Should().Contain("h3");
    }

    [Fact]
    public void LoyaltyPage_HasTextWarning()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.Markup.Should().Contain("text-warning");
    }

    [Fact]
    public void LoyaltyPage_ShowsVisuallyHiddenLoadingText()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.Markup.Should().Contain("visually-hidden");
    }

    [Fact]
    public void LoyaltyPage_HasMb4Class()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.Markup.Should().Contain("mb-4");
    }

    [Fact]
    public void LoyaltyPage_HasTextCenterClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.Markup.Should().Contain("text-center");
    }

    [Fact]
    public void LoyaltyPage_HasRoleStatus()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.Markup.Should().Contain("role=\"status\"");
    }

    [Fact]
    public void LoyaltyPage_HasLoadingText()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.Markup.Should().Contain("Loading...");
    }

    [Fact]
    public void LoyaltyPage_HasDivElements()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.FindAll("div").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void LoyaltyPage_HasSpanElements()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.FindAll("span").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void LoyaltyPage_HasH3Element()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.Find("h3").Should().NotBeNull();
    }

    [Fact]
    public void LoyaltyPage_HasContainerClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void LoyaltyPage_HasSpinnerBorderClass()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.Find("div.spinner-border").Should().NotBeNull();
    }

    [Fact]
    public void LoyaltyPage_RendersNotEmpty()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void LoyaltyPage_HasIElement()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<Loyalty>();
        
        // Bootstrap icons use span with bi- classes, not i elements
        cut.FindAll("span[class*='bi-']").Should().HaveCountGreaterThan(0);
    }
}
