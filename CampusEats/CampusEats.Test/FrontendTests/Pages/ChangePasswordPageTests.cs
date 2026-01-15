using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages.Auth;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace CampusEats.Test.FrontendTests.Pages;

public class ChangePasswordPageTests : BunitContext
{
    private readonly MockHttpMessageHandler _mockHttp;

    public ChangePasswordPageTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5298/");
        
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        
        var authService = new AuthService(httpClient, mockJsRuntime.Object);
        
        Services.AddSingleton(authService);
    }

    [Fact]
    public void ChangePasswordPage_RendersTitle()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("Parol");
    }

    [Fact]
    public void ChangePasswordPage_HasCurrentPasswordInput()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("Parola Curent");
    }

    [Fact]
    public void ChangePasswordPage_HasNewPasswordInput()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("Parola Nou");
    }

    [Fact]
    public void ChangePasswordPage_HasConfirmPasswordInput()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("Confirm");
    }

    [Fact]
    public void ChangePasswordPage_HasSubmitButton()
    {
        var cut = Render<ChangePassword>();
        
        var button = cut.Find("button[type='submit']");
        button.Should().NotBeNull();
    }

    [Fact]
    public void ChangePasswordPage_HasForm()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("form");
    }

    [Fact]
    public void ChangePasswordPage_HasContainer()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void ChangePasswordPage_HasCard()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("card");
    }

    [Fact]
    public void ChangePasswordPage_HasShadow()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("shadow");
    }

    [Fact]
    public void ChangePasswordPage_HasRow()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("row");
    }

    [Fact]
    public void ChangePasswordPage_HasFormControl()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("form-control");
    }

    [Fact]
    public void ChangePasswordPage_HasFormLabel()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("form-label");
    }

    [Fact]
    public void ChangePasswordPage_HasMb3()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("mb-3");
    }

    [Fact]
    public void ChangePasswordPage_HasJustifyCenter()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("justify-content-center");
    }

    [Fact]
    public void ChangePasswordPage_HasCardBody()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("card-body");
    }

    [Fact]
    public void ChangePasswordPage_HasContainerClass()
    {
        var cut = Render<ChangePassword>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void ChangePasswordPage_HasRowClass()
    {
        var cut = Render<ChangePassword>();
        
        cut.Find("div.row").Should().NotBeNull();
    }

    [Fact]
    public void ChangePasswordPage_HasCardClass()
    {
        var cut = Render<ChangePassword>();
        
        cut.Find("div.card").Should().NotBeNull();
    }

    [Fact]
    public void ChangePasswordPage_HasFormElement()
    {
        var cut = Render<ChangePassword>();
        
        cut.Find("form").Should().NotBeNull();
    }

    [Fact]
    public void ChangePasswordPage_HasInputElements()
    {
        var cut = Render<ChangePassword>();
        
        cut.FindAll("input").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void ChangePasswordPage_HasLabelElements()
    {
        var cut = Render<ChangePassword>();
        
        cut.FindAll("label").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void ChangePasswordPage_HasBtnPrimary()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("btn-primary");
    }

    [Fact]
    public void ChangePasswordPage_HasPasswordType()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("type=\"password\"");
    }

    [Fact]
    public void ChangePasswordPage_HasCurrentPasswordInputElement()
    {
        var cut = Render<ChangePassword>();
        
        var inputs = cut.FindAll("input[type='password']");
        inputs.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void ChangePasswordPage_HasTextCenter()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("text-center");
    }

    [Fact]
    public void ChangePasswordPage_HasMt5()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("mt-5");
    }

    [Fact]
    public void ChangePasswordPage_HasCol()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("col");
    }

    [Fact]
    public void ChangePasswordPage_HasCardTitle()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("card-title");
    }

    [Fact]
    public void ChangePasswordPage_HasW100Button()
    {
        var cut = Render<ChangePassword>();
        
        cut.Markup.Should().Contain("w-100");
    }
}
