using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;

namespace CampusEats.Test.FrontendTests.Pages;

public class HomePageTests : BunitContext
{
    public HomePageTests()
    {
        var mockHttpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5298/") };
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        var authService = new AuthService(mockHttpClient, mockJsRuntime.Object);
        
        Services.AddSingleton(authService);
    }

    [Fact]
    public void HomePage_RendersTitle()
    {
        var cut = Render<Home>();
        
        cut.Markup.Should().Contain("CampusEats");
    }

    [Fact]
    public void HomePage_DisplaysWelcomeMessage()
    {
        var cut = Render<Home>();
        
        cut.Markup.Should().Contain("Bine ai venit");
    }

    [Fact]
    public void HomePage_DisplaysFeatureCards()
    {
        var cut = Render<Home>();
        
        cut.Markup.Should().Contain("Recompense Loialitate");
    }

    [Fact]
    public void HomePage_DisplaysHowItWorksSection()
    {
        var cut = Render<Home>();
        
        cut.Markup.Should().Contain("Cum Func");
    }

    [Fact]
    public void HomePage_HasHeroSection()
    {
        var cut = Render<Home>();
        
        cut.Markup.Should().Contain("hero");
    }

    [Fact]
    public void HomePage_HasContainer()
    {
        var cut = Render<Home>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void HomePage_HasRow()
    {
        var cut = Render<Home>();
        
        cut.Markup.Should().Contain("row");
    }

    [Fact]
    public void HomePage_HasCol()
    {
        var cut = Render<Home>();
        
        cut.Markup.Should().Contain("col");
    }

    [Fact]
    public void HomePage_HasCard()
    {
        var cut = Render<Home>();
        
        cut.Markup.Should().Contain("card");
    }

    [Fact]
    public void HomePage_HasCardBody()
    {
        var cut = Render<Home>();
        
        cut.Markup.Should().Contain("card-body");
    }

    [Fact]
    public void HomePage_HasShadow()
    {
        var cut = Render<Home>();
        
        cut.Markup.Should().Contain("shadow");
    }

    [Fact]
    public void HomePage_HasTextCenter()
    {
        var cut = Render<Home>();
        
        cut.Markup.Should().Contain("text-center");
    }

    [Fact]
    public void HomePage_HasMb4()
    {
        var cut = Render<Home>();
        
        cut.Markup.Should().Contain("mb-4");
    }

    [Fact]
    public void HomePage_HasH1Element()
    {
        var cut = Render<Home>();
        
        cut.Find("h1").Should().NotBeNull();
    }

    [Fact]
    public void HomePage_HasHElements()
    {
        var cut = Render<Home>();
        
        // Has heading elements
        cut.Markup.Should().Contain("<h");
    }

    [Fact]
    public void HomePage_HasDivElements()
    {
        var cut = Render<Home>();
        
        cut.FindAll("div").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void HomePage_HasBootstrapIcons()
    {
        var cut = Render<Home>();
        
        cut.Markup.Should().Contain("bi-");
    }

    [Fact]
    public void HomePage_HasBtnPrimary()
    {
        var cut = Render<Home>();
        
        cut.Markup.Should().Contain("btn-primary");
    }

    [Fact]
    public void HomePage_HasPElements()
    {
        var cut = Render<Home>();
        
        cut.FindAll("p").Should().HaveCountGreaterThan(0);
    }
}
