using Bunit;
using Xunit;
using CampusEatsFrontend.Pages.Payments;
using FluentAssertions;

namespace CampusEats.Test.FrontendTests.Pages;

public class PaymentSuccessPageTests : BunitContext
{
    [Fact]
    public void PaymentSuccessPage_RendersSuccessTitle()
    {
        var cut = Render<PaymentSuccess>();
        
        cut.Markup.Should().Contain("Plată realizată cu succes");
    }

    [Fact]
    public void PaymentSuccessPage_RendersSuccessMessage()
    {
        var cut = Render<PaymentSuccess>();
        
        cut.Markup.Should().Contain("Plata a fost procesată");
    }

    [Fact]
    public void PaymentSuccessPage_HasGoToOrdersButton()
    {
        var cut = Render<PaymentSuccess>();
        
        cut.Markup.Should().Contain("btn");
        cut.Markup.Should().Contain("Mergi la Comenzi");
    }

    [Fact]
    public void PaymentSuccessPage_ButtonCanBeClicked()
    {
        var cut = Render<PaymentSuccess>();
        
        var button = cut.Find("button");
        button.Should().NotBeNull();
    }
}
