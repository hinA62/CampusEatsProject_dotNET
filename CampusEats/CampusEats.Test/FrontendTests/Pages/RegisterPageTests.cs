using Bunit;
using Xunit;
using Moq;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Pages.Auth;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;

namespace CampusEats.Test.FrontendTests.Pages;

public class RegisterPageTests : BunitContext
{
    public RegisterPageTests()
    {
        var mockHttpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5298/") };
        var mockJsRuntime = new Mock<Microsoft.JSInterop.IJSRuntime>();
        var authService = new AuthService(mockHttpClient, mockJsRuntime.Object);
        
        Services.AddSingleton(authService);
    }

    [Fact]
    public void RegisterPage_RendersRegistrationForm()
    {
        var cut = Render<Register>();
        
        cut.Markup.Should().Contain("Înregistrare");
    }

    [Fact]
    public void RegisterPage_HasUsernameInput()
    {
        var cut = Render<Register>();
        
        var usernameInput = cut.Find("input#username");
        usernameInput.Should().NotBeNull();
    }

    [Fact]
    public void RegisterPage_HasEmailInput()
    {
        var cut = Render<Register>();
        
        var emailInput = cut.Find("input#email");
        emailInput.Should().NotBeNull();
    }

    [Fact]
    public void RegisterPage_HasPasswordInput()
    {
        var cut = Render<Register>();
        
        var passwordInput = cut.Find("input#password");
        passwordInput.Should().NotBeNull();
    }

    [Fact]
    public void RegisterPage_HasConfirmPasswordInput()
    {
        var cut = Render<Register>();
        
        var confirmPasswordInput = cut.Find("input#confirmPassword");
        confirmPasswordInput.Should().NotBeNull();
    }

    [Fact]
    public void RegisterPage_HasSubmitButton()
    {
        var cut = Render<Register>();
        
        var submitButton = cut.Find("button[type='submit']");
        submitButton.Should().NotBeNull();
    }

    [Fact]
    public void RegisterPage_HasLoginLink()
    {
        var cut = Render<Register>();
        
        cut.Markup.Should().Contain("Ai dega un cont?");
        cut.Markup.Should().Contain("/login");
    }

    [Fact]
    public void RegisterPage_DisplaysFormLabels()
    {
        var cut = Render<Register>();
        
        cut.Markup.Should().Contain("Nume de utilizator");
        cut.Markup.Should().Contain("Email");
        cut.Markup.Should().Contain("Parolă");
        cut.Markup.Should().Contain("Confirmă Parola");
    }

    [Fact]
    public void RegisterPage_HasContainer()
    {
        var cut = Render<Register>();
        
        cut.Markup.Should().Contain("container");
    }

    [Fact]
    public void RegisterPage_HasCard()
    {
        var cut = Render<Register>();
        
        cut.Markup.Should().Contain("card");
    }

    [Fact]
    public void RegisterPage_HasShadow()
    {
        var cut = Render<Register>();
        
        cut.Markup.Should().Contain("shadow");
    }

    [Fact]
    public void RegisterPage_HasCardBody()
    {
        var cut = Render<Register>();
        
        cut.Markup.Should().Contain("card-body");
    }

    [Fact]
    public void RegisterPage_HasFormControl()
    {
        var cut = Render<Register>();
        
        cut.Markup.Should().Contain("form-control");
    }

    [Fact]
    public void RegisterPage_HasFormLabel()
    {
        var cut = Render<Register>();
        
        cut.Markup.Should().Contain("form-label");
    }

    [Fact]
    public void RegisterPage_HasMb3()
    {
        var cut = Render<Register>();
        
        cut.Markup.Should().Contain("mb-3");
    }

    [Fact]
    public void RegisterPage_HasRow()
    {
        var cut = Render<Register>();
        
        cut.Markup.Should().Contain("row");
    }

    [Fact]
    public void RegisterPage_HasJustifyCenter()
    {
        var cut = Render<Register>();
        
        cut.Markup.Should().Contain("justify-content-center");
    }

    [Fact]
    public void RegisterPage_HasContainerClass()
    {
        var cut = Render<Register>();
        
        cut.Find("div.container").Should().NotBeNull();
    }

    [Fact]
    public void RegisterPage_HasRowClass()
    {
        var cut = Render<Register>();
        
        cut.Find("div.row").Should().NotBeNull();
    }

    [Fact]
    public void RegisterPage_HasCardClass()
    {
        var cut = Render<Register>();
        
        cut.Find("div.card").Should().NotBeNull();
    }

    [Fact]
    public void RegisterPage_HasFormElement()
    {
        var cut = Render<Register>();
        
        cut.Find("form").Should().NotBeNull();
    }

    [Fact]
    public void RegisterPage_HasInputElements()
    {
        var cut = Render<Register>();
        
        cut.FindAll("input").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void RegisterPage_HasLabelElements()
    {
        var cut = Render<Register>();
        
        cut.FindAll("label").Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void RegisterPage_HasBtnPrimary()
    {
        var cut = Render<Register>();
        
        cut.Markup.Should().Contain("btn-primary");
    }

    [Fact]
    public void RegisterPage_UsernameInput_AcceptsValue()
    {
        var cut = Render<Register>();
        
        var usernameInput = cut.Find("input#username");
        usernameInput.Change("testuser");
        
        usernameInput.GetAttribute("value").Should().Be("testuser");
    }

    [Fact]
    public void RegisterPage_EmailInput_AcceptsValue()
    {
        var cut = Render<Register>();
        
        var emailInput = cut.Find("input#email");
        emailInput.Change("test@example.com");
        
        emailInput.GetAttribute("value").Should().Be("test@example.com");
    }

    [Fact]
    public void RegisterPage_PasswordInput_AcceptsValue()
    {
        var cut = Render<Register>();
        
        var passwordInput = cut.Find("input#password");
        passwordInput.Change("password123");
        
        passwordInput.GetAttribute("value").Should().Be("password123");
    }

    [Fact]
    public void RegisterPage_ConfirmPasswordInput_AcceptsValue()
    {
        var cut = Render<Register>();
        
        var confirmPasswordInput = cut.Find("input#confirmPassword");
        confirmPasswordInput.Change("password123");
        
        confirmPasswordInput.GetAttribute("value").Should().Be("password123");
    }

    [Fact]
    public void RegisterPage_AllInputs_AcceptValues()
    {
        var cut = Render<Register>();
        
        cut.Find("input#username").Change("testuser");
        cut.Find("input#email").Change("test@example.com");
        cut.Find("input#password").Change("password123");
        cut.Find("input#confirmPassword").Change("password123");
        
        cut.Find("input#username").GetAttribute("value").Should().Be("testuser");
        cut.Find("input#email").GetAttribute("value").Should().Be("test@example.com");
    }

    [Fact]
    public void RegisterPage_HasCardTitle()
    {
        var cut = Render<Register>();
        
        cut.Markup.Should().Contain("card-title");
    }

    [Fact]
    public void RegisterPage_HasTextCenter()
    {
        var cut = Render<Register>();
        
        cut.Markup.Should().Contain("text-center");
    }

    [Fact]
    public void RegisterPage_HasMt5()
    {
        var cut = Render<Register>();
        
        cut.Markup.Should().Contain("mt-5");
    }

    [Fact]
    public void RegisterPage_HasCol()
    {
        var cut = Render<Register>();
        
        cut.Markup.Should().Contain("col");
    }
}
