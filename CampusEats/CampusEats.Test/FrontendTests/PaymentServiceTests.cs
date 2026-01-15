using Xunit;
using FluentAssertions;
using CampusEatsFrontend.Services;
using CampusEatsFrontend.Models.Payment;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace CampusEats.Test.FrontendTests;

public class PaymentServiceTests
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly HttpClient _httpClient;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClient = _mockHttp.ToHttpClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5298/");
        _paymentService = new PaymentService(_httpClient);
    }

    [Fact]
    public async Task CreateStripeCheckoutSessionAsync_ReturnsCheckoutUrl()
    {
        var request = new CreateStripeCheckoutSessionRequest { UserId = Guid.NewGuid(), OrderId = Guid.NewGuid() };
        var response = new { CheckoutUrl = "https://checkout.stripe.com/session123" };
        _mockHttp.When("http://localhost:5298/api/payments/stripe/checkout-session").Respond("application/json", JsonSerializer.Serialize(response));

        var result = await _paymentService.CreateStripeCheckoutSessionAsync(request);

        result.Should().NotBeNull();
        result.Should().Contain("stripe.com");
    }

    [Fact]
    public async Task CreateStripeCheckoutSessionAsync_WithFailure_ReturnsNull()
    {
        var request = new CreateStripeCheckoutSessionRequest { UserId = Guid.NewGuid(), OrderId = Guid.NewGuid() };
        _mockHttp.When("http://localhost:5298/api/payments/stripe/checkout-session").Respond(HttpStatusCode.BadRequest);

        var result = await _paymentService.CreateStripeCheckoutSessionAsync(request);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPaymentHistoryAsync_ReturnsPaymentList()
    {
        var userId = Guid.NewGuid();
        var payments = new[] {
            new { Id = Guid.NewGuid(), Amount = 25.00m, Status = "Succeeded" },
            new { Id = Guid.NewGuid(), Amount = 15.00m, Status = "Pending" }
        };
        _mockHttp.When($"http://localhost:5298/api/users/{userId}/payments").Respond("application/json", JsonSerializer.Serialize(payments));

        var result = await _paymentService.GetPaymentHistoryAsync(userId);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPaymentByIdAsync_WithValidId_ReturnsPayment()
    {
        var paymentId = Guid.NewGuid();
        var payment = new { Id = paymentId, Amount = 50.00m, Status = "Succeeded" };
        _mockHttp.When($"http://localhost:5298/api/payments/{paymentId}").Respond("application/json", JsonSerializer.Serialize(payment));

        var result = await _paymentService.GetPaymentByIdAsync(paymentId);

        result.Should().NotBeNull();
        result!.Amount.Should().Be(50.00m);
    }

    [Fact]
    public async Task CreatePaymentAsync_ReturnsSuccessResponse()
    {
        var request = new CreatePaymentRequest 
        { 
            UserId = Guid.NewGuid(), 
            OrderId = Guid.NewGuid(),
            Amount = 25.00m,
            Method = PaymentMethod.MockCard
        };
        _mockHttp.When("http://localhost:5298/api/payments").Respond(HttpStatusCode.Created);

        var result = await _paymentService.CreatePaymentAsync(request);

        result.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task CreatePaymentAsync_WithInvalidData_ReturnsBadRequest()
    {
        var request = new CreatePaymentRequest 
        { 
            UserId = Guid.NewGuid(), 
            OrderId = Guid.NewGuid(),
            Amount = -10.00m,
            Method = PaymentMethod.MockCard
        };
        _mockHttp.When("http://localhost:5298/api/payments").Respond(HttpStatusCode.BadRequest);

        var result = await _paymentService.CreatePaymentAsync(request);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateStripeCheckoutSessionAsync_WithPointsToUse_ReturnsUrl()
    {
        var request = new CreateStripeCheckoutSessionRequest 
        { 
            UserId = Guid.NewGuid(), 
            OrderId = Guid.NewGuid(),
            PointsToUse = 100
        };
        var response = new { CheckoutUrl = "https://checkout.stripe.com/session456" };
        _mockHttp.When("http://localhost:5298/api/payments/stripe/checkout-session").Respond("application/json", JsonSerializer.Serialize(response));

        var result = await _paymentService.CreateStripeCheckoutSessionAsync(request);

        result.Should().NotBeNull();
    }
}
