using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages.Menu;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace CampusEats.Test.FrontendTests.Pages;

public class CreateMenuItemPageTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public CreateMenuItemPageTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5298/");
        
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        
        var authService = new AuthService(httpClient, mockJsRuntime.Object);
        var menuItemService = new MenuItemService(httpClient);
        
        Services.AddSingleton(authService);
        Services.AddSingleton(menuItemService);
    }

    [Fact]
    public void CreateMenuItemPage_RendersForm()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("form");
    }

    [Fact]
    public void CreateMenuItemPage_HasNameInput()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("Nume");
    }

    [Fact]
    public void CreateMenuItemPage_HasPriceInput()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("Pre");
    }

    [Fact]
    public void CreateMenuItemPage_HasAllergensInput()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("Alergeni");
    }

    [Fact]
    public void CreateMenuItemPage_HasSubmitButton()
    {
        var cut = Render<CreateMenuItem>();
        
        var button = cut.Find("button[type='submit']");
        button.Should().NotBeNull();
    }

    [Fact]
    public void CreateMenuItemPage_HasCancelButton()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("Anule");
    }

    [Fact]
    public void CreateMenuItemPage_HasContainer()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void CreateMenuItemPage_HasCard()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("card");
    }

    [Fact]
    public void CreateMenuItemPage_HasShadow()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("shadow");
    }

    [Fact]
    public void CreateMenuItemPage_HasMarginTop()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("mt-4");
    }

    [Fact]
    public void CreateMenuItemPage_HasFormControl()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("form-control");
    }

    [Fact]
    public void CreateMenuItemPage_HasFormLabel()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("form-label");
    }

    [Fact]
    public void CreateMenuItemPage_HasCardBody()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("card-body");
    }

    [Fact]
    public void CreateMenuItemPage_HasRow()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("row");
    }

    [Fact]
    public void CreateMenuItemPage_HasJustifyCenter()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("justify-content-center");
    }

    [Fact]
    public void CreateMenuItemPage_HasMb3()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("mb-3");
    }

    [Fact]
    public void CreateMenuItemPage_HasContainerClass()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void CreateMenuItemPage_HasRowClass()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Find("div.row").Should().NotBeNull();
    }

    [Fact]
    public void CreateMenuItemPage_HasCardClass()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Find("div.card").Should().NotBeNull();
    }

    [Fact]
    public void CreateMenuItemPage_HasCardBodyClass()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Find("div.card-body").Should().NotBeNull();
    }

    [Fact]
    public void CreateMenuItemPage_HasFormElement()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Find("form").Should().NotBeNull();
    }

    [Fact]
    public void CreateMenuItemPage_HasInputElements()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.FindAll("input").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void CreateMenuItemPage_HasLabelElements()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.FindAll("label").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void CreateMenuItemPage_HasButtonElements()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.FindAll("button").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void CreateMenuItemPage_HasDFlex()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("d-flex");
    }

    [Fact]
    public void CreateMenuItemPage_HasBtnPrimary()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("btn-primary");
    }

    [Fact]
    public void CreateMenuItemPage_HasBtnOutlineSecondary()
    {
        var cut = Render<CreateMenuItem>();
        
        cut.Markup.Should().Contain("btn-outline-secondary");
    }
}
