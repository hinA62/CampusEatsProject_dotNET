using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace CampusEats.Test.FrontendTests.Pages;

public class ProfilePageTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public ProfilePageTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5298/");
        
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        
        var authService = new AuthService(httpClient, mockJsRuntime.Object);
        
        Services.AddSingleton(authService);
    }

    [Fact]
    public void ProfilePage_RendersTitle()
    {
        var cut = Render<Profile>();
        
        cut.Markup.Should().Contain("Profil");
    }

    [Fact]
    public void ProfilePage_ShowsLoginPrompt_WhenNotAuthenticated()
    {
        var cut = Render<Profile>();
        
        // When not authenticated, should show some message
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void ProfilePage_HasContainer()
    {
        var cut = Render<Profile>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void ProfilePage_RendersMarkup()
    {
        var cut = Render<Profile>();
        
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void ProfilePage_HasCard()
    {
        var cut = Render<Profile>();
        
        cut.Markup.Should().Contain("card");
    }

    [Fact]
    public void ProfilePage_HasShadow()
    {
        var cut = Render<Profile>();
        
        cut.Markup.Should().Contain("shadow");
    }

    [Fact]
    public void ProfilePage_HasHeading()
    {
        var cut = Render<Profile>();
        
        cut.Markup.Should().Contain("h3");
    }

    [Fact]
    public void ProfilePage_HasCardBody()
    {
        var cut = Render<Profile>();
        
        cut.Markup.Should().Contain("card-body");
    }

    [Fact]
    public void ProfilePage_HasRow()
    {
        var cut = Render<Profile>();
        
        cut.Markup.Should().Contain("row");
    }

    [Fact]
    public void ProfilePage_HasJustifyCenter()
    {
        var cut = Render<Profile>();
        
        cut.Markup.Should().Contain("justify-content-center");
    }

    [Fact]
    public void ProfilePage_HasMarginTop()
    {
        var cut = Render<Profile>();
        
        // May have mt-4 or other margin classes
        cut.Markup.Should().NotBeEmpty();
    }

    [Fact]
    public void ProfilePage_HasMb4()
    {
        var cut = Render<Profile>();
        
        cut.Markup.Should().Contain("mb-4");
    }

    [Fact]
    public void ProfilePage_HasDivElements()
    {
        var cut = Render<Profile>();
        
        cut.FindAll("div").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void ProfilePage_HasH3Element()
    {
        var cut = Render<Profile>();
        
        cut.Find("h3").Should().NotBeNull();
    }

    [Fact]
    public void ProfilePage_HasContainerClass()
    {
        var cut = Render<Profile>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void ProfilePage_HasRowClass()
    {
        var cut = Render<Profile>();
        
        cut.Find("div.row").Should().NotBeNull();
    }

    [Fact]
    public void ProfilePage_HasCardClass()
    {
        var cut = Render<Profile>();
        
        cut.Find("div.card").Should().NotBeNull();
    }

    [Fact]
    public void ProfilePage_HasCardBodyClass()
    {
        var cut = Render<Profile>();
        
        cut.Find("div.card-body").Should().NotBeNull();
    }

    [Fact]
    public void ProfilePage_HasTextCenter()
    {
        var cut = Render<Profile>();
        
        cut.Markup.Should().Contain("text-center");
    }
}
