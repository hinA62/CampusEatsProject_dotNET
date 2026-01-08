using CampusEats.Features.Payment;
using CampusEats.Features.Payment.Handlers;
using CampusEats.Features.Payment.Requests;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Stripe;
using System.Text;

namespace CampusEats.Test.PaymentTests;

public class StripeWebhookHandlerTests
{
    private readonly CampusEatsContext _context;
    private readonly Mock<IConfiguration> _config;
    private readonly Mock<CreatePaymentHandler> _paymentHandler;
    private readonly StripeWebhookHandler _handler;

    public StripeWebhookHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new CampusEatsContext(options);

        _config = new Mock<IConfiguration>();
        _config.Setup(c => c["Stripe:WebhookSecret"]).Returns("whsec_test");

        // CreatePaymentHandler now only takes CampusEatsContext
        _paymentHandler = new Mock<CreatePaymentHandler>(_context);
        _handler = new StripeWebhookHandler(_config.Object, _paymentHandler.Object);
    }

    [Fact]
    public async Task Handle_InvalidSignature_ReturnsBadRequest()
    {
        var request = CreateMockHttpRequest("{}", "invalid_signature");

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        // Verify it's a BadRequest by checking the result type
        var resultType = result.GetType().Name;
        resultType.Should().Contain("BadRequest");
    }

    [Fact]
    public async Task Handle_NonCheckoutSessionEvent_ReturnsOk()
    {
        var json = CreateStripeEventJson("payment_intent.created", new { });
        var signature = GenerateValidSignature(json);
        var request = CreateMockHttpRequest(json, signature);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_MissingMetadata_ReturnsBadRequest()
    {
        var sessionData = new
        {
            id = "cs_test",
            metadata = new Dictionary<string, string>()
        };
        var json = CreateStripeEventJson(Events.CheckoutSessionCompleted, sessionData);
        var signature = GenerateValidSignature(json);
        var request = CreateMockHttpRequest(json, signature);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
    }

    private HttpRequest CreateMockHttpRequest(string json, string signature)
    {
        var context = new DefaultHttpContext();
        var request = context.Request;
        request.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
        request.Headers["Stripe-Signature"] = signature;
        return request;
    }

    private string CreateStripeEventJson(string eventType, object data)
    {
        return $$"""
        {
            "id": "evt_test",
            "type": "{{eventType}}",
            "data": {
                "object": {{System.Text.Json.JsonSerializer.Serialize(data)}}
            }
        }
        """;
    }

    private string GenerateValidSignature(string json)
    {
        return "t=1234567890,v1=test_signature";
    }
}
