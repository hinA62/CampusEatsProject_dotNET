using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages.Auth;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;

namespace CampusEats.Test.FrontendTests.Pages;

public class LoginPageTests : BunitContext
{
    public LoginPageTests()
    {
        var mockHttpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5298/") };
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        var authService = new AuthService(mockHttpClient, mockJsRuntime.Object);
        
        Services.AddSingleton(authService);
    }

    [Fact]
    public void LoginPage_RendersLoginForm()
    {
        var cut = Render<Login>();
        
        cut.Markup.Should().Contain("Login");
        cut.Markup.Should().Contain("Email");
        cut.Markup.Should().Contain("Parolă");
    }

    [Fact]
    public void LoginPage_HasEmailInput()
    {
        var cut = Render<Login>();
        
        var emailInput = cut.Find("input#email");
        emailInput.Should().NotBeNull();
    }

    [Fact]
    public void LoginPage_HasPasswordInput()
    {
        var cut = Render<Login>();
        
        var passwordInput = cut.Find("input#password");
        passwordInput.Should().NotBeNull();
    }

    [Fact]
    public void LoginPage_HasSubmitButton()
    {
        var cut = Render<Login>();
        
        var submitButton = cut.Find("button[type='submit']");
        submitButton.Should().NotBeNull();
        submitButton.TextContent.Should().Contain("Login");
    }

    [Fact]
    public void LoginPage_HasRegisterLink()
    {
        var cut = Render<Login>();
        
        cut.Markup.Should().Contain("Nu ai un cont?");
        cut.Markup.Should().Contain("/register");
    }

    [Fact]
    public void LoginPage_EmailInput_AcceptsValue()
    {
        var cut = Render<Login>();
        
        var emailInput = cut.Find("input#email");
        emailInput.Change("test@example.com");
        
        emailInput.GetAttribute("value").Should().Be("test@example.com");
    }

    [Fact]
    public void LoginPage_HasContainer()
    {
        var cut = Render<Login>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void LoginPage_HasCard()
    {
        var cut = Render<Login>();
        
        cut.Markup.Should().Contain("card");
    }

    [Fact]
    public void LoginPage_HasShadow()
    {
        var cut = Render<Login>();
        
        cut.Markup.Should().Contain("shadow");
    }

    [Fact]
    public void LoginPage_HasCardBody()
    {
        var cut = Render<Login>();
        
        cut.Markup.Should().Contain("card-body");
    }

    [Fact]
    public void LoginPage_HasFormControl()
    {
        var cut = Render<Login>();
        
        cut.Markup.Should().Contain("form-control");
    }

    [Fact]
    public void LoginPage_HasFormLabel()
    {
        var cut = Render<Login>();
        
        cut.Markup.Should().Contain("form-label");
    }

    [Fact]
    public void LoginPage_HasMb3()
    {
        var cut = Render<Login>();
        
        cut.Markup.Should().Contain("mb-3");
    }

    [Fact]
    public void LoginPage_HasRow()
    {
        var cut = Render<Login>();
        
        cut.Markup.Should().Contain("row");
    }

    [Fact]
    public void LoginPage_HasJustifyCenter()
    {
        var cut = Render<Login>();
        
        cut.Markup.Should().Contain("justify-content-center");
    }

    [Fact]
    public void LoginPage_HasContainerClass()
    {
        var cut = Render<Login>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void LoginPage_HasRowClass()
    {
        var cut = Render<Login>();
        
        cut.Find("div.row").Should().NotBeNull();
    }

    [Fact]
    public void LoginPage_HasCardClass()
    {
        var cut = Render<Login>();
        
        cut.Find("div.card").Should().NotBeNull();
    }

    [Fact]
    public void LoginPage_HasFormElement()
    {
        var cut = Render<Login>();
        
        cut.Find("form").Should().NotBeNull();
    }

    [Fact]
    public void LoginPage_HasInputElements()
    {
        var cut = Render<Login>();
        
        cut.FindAll("input").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void LoginPage_HasLabelElements()
    {
        var cut = Render<Login>();
        
        cut.FindAll("label").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void LoginPage_HasBtnPrimary()
    {
        var cut = Render<Login>();
        
        cut.Markup.Should().Contain("btn-primary");
    }

    [Fact]
    public void LoginPage_PasswordInput_AcceptsValue()
    {
        var cut = Render<Login>();
        
        var passwordInput = cut.Find("input#password");
        passwordInput.Change("testPassword123");
        
        passwordInput.GetAttribute("value").Should().Be("testPassword123");
    }

    [Fact]
    public void LoginPage_SubmitButton_HasCorrectText()
    {
        var cut = Render<Login>();
        
        var submitButton = cut.Find("button[type='submit']");
        submitButton.TextContent.Should().Contain("Login");
    }

    [Fact]
    public void LoginPage_Form_HasEditFormElement()
    {
        var cut = Render<Login>();
        
        cut.Find("form").Should().NotBeNull();
    }

    [Fact]
    public void LoginPage_EmailAndPasswordInputs_AcceptValues()
    {
        var cut = Render<Login>();
        
        var emailInput = cut.Find("input#email");
        var passwordInput = cut.Find("input#password");
        
        emailInput.Change("test@example.com");
        passwordInput.Change("password123");
        
        emailInput.GetAttribute("value").Should().Be("test@example.com");
        passwordInput.GetAttribute("value").Should().Be("password123");
    }

    [Fact]
    public void LoginPage_HasCardTitle()
    {
        var cut = Render<Login>();
        
        cut.Markup.Should().Contain("card-title");
    }

    [Fact]
    public void LoginPage_HasTextCenter()
    {
        var cut = Render<Login>();
        
        cut.Markup.Should().Contain("text-center");
    }

    [Fact]
    public void LoginPage_HasMt5()
    {
        var cut = Render<Login>();
        
        cut.Markup.Should().Contain("mt-5");
    }

    [Fact]
    public void LoginPage_HasColMd4()
    {
        var cut = Render<Login>();
        
        cut.Markup.Should().Contain("col-md-4");
    }
}
