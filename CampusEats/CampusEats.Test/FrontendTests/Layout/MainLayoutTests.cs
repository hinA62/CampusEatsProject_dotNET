using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Layout;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace CampusEats.Test.FrontendTests.Layout;

public class MainLayoutTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public MainLayoutTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5298/");
        
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        
        // MainLayout uses NavMenu and CartSidebar which need these services
        var authService = new AuthService(httpClient, mockJsRuntime.Object);
        var cartService = new CartService(mockJsRuntime.Object);
        var orderService = new OrderService(httpClient);
        
        Services.AddSingleton(authService);
        Services.AddSingleton(cartService);
        Services.AddSingleton(orderService);
    }

    [Fact]
    public void MainLayout_RendersPageContainer()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<MainLayout>();
        
        cut.Markup.Should().Contain("page");
    }

    [Fact]
    public void MainLayout_RendersSidebar()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<MainLayout>();
        
        cut.Markup.Should().Contain("sidebar");
    }

    [Fact]
    public void MainLayout_RendersMainContent()
    {
        _mockHttp.When("*").Respond("application/json", "{}");
        
        var cut = Render<MainLayout>();
        
        cut.Markup.Should().Contain("main");
        cut.Markup.Should().Contain("content");
    }
}
