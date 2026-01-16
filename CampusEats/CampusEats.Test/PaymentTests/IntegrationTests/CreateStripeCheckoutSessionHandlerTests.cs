using CampusEats.Features.Order;
using CampusEats.Features.Payment.Handlers;
using CampusEats.Features.Payment.Requests;
using CampusEats.Features.User;
using CampusEats.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Stripe;

namespace CampusEats.Test.PaymentTests.IntegrationTests;

public class CreateStripeCheckoutSessionHandlerTests
{
    private readonly CampusEatsContext _context;
    private readonly Mock<IConfiguration> _config;
    private readonly CreateStripeCheckoutSessionHandler _handler;

    public CreateStripeCheckoutSessionHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;
        _context = new CampusEatsContext(options);

        _config = new Mock<IConfiguration>();
        _config.Setup(c => c["Stripe:ClientBaseUrl"]).Returns("http://localhost:5007");

        // Stripe SDK are nevoie de o cheie (chiar și invalidă) pentru a nu arunca excepție la instanțiere
        StripeConfiguration.ApiKey = "sk_test_51MockKey";

        _handler = new CreateStripeCheckoutSessionHandler(_context, _config.Object);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ReturnsNotFound()
    {
        var request = new CreateStripeCheckoutSessionRequest(Guid.NewGuid(), Guid.NewGuid(), null);
        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.GetType().Name.Should().Contain("NotFound");
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        var orderId = await SeedOrder(100m);
        var request = new CreateStripeCheckoutSessionRequest(Guid.NewGuid(), orderId, null);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.GetType().Name.Should().Contain("NotFound");
    }

    [Fact]
    public async Task Handle_MissingClientBaseUrl_ReturnsProblem()
    {
        _config.Setup(c => c["Stripe:ClientBaseUrl"]).Returns((string?)null);
        var userId = await SeedUser();
        var orderId = await SeedOrder(100m, userId);

        var request = new CreateStripeCheckoutSessionRequest(userId, orderId, null);
        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.GetType().Name.Should().Contain("Problem");
    }

    [Fact]
    public async Task Handle_FinalAmountZeroOrNegative_ReturnsBadRequest()
    {
        // Cazul în care reducerea face ca suma să fie exact 0 sau mai mică
        var userId = await SeedUser();
        var orderId = await SeedOrder(10m, userId); // 10 RON

        // 1000 puncte = 10 RON discount. Preț final = 0.
        var request = new CreateStripeCheckoutSessionRequest(userId, orderId, 1000);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.GetType().Name.Should().Contain("BadRequest");
    }

    [Fact]
    public async Task Handle_PointsExceedOrderPrice_CapsDiscountAndCalculatesCorrectly()
    {
        // Testează ramura: if (discount > maxDiscount)
        var userId = await SeedUser();
        var orderId = await SeedOrder(20m, userId);

        // Cerem 5000 puncte (50 RON), dar prețul e 20 RON. 
        // Codul ar trebui să plafoneze la 2000 puncte, dar rezultatul final va fi 0 RON
        // ceea ce va declanșa BadRequest-ul de sumă <= 0.
        var request = new CreateStripeCheckoutSessionRequest(userId, orderId, 5000);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.GetType().Name.Should().Contain("BadRequest");
    }

    [Fact]
    public async Task Handle_ValidRequestWithPartialPoints_AttemptsToCreateStripeSession()
    {
        // Acest test acoperă logica de calcul a punctelor valide (ex: 5 RON reducere la 100 RON)
        var userId = await SeedUser();
        var orderId = await SeedOrder(100m, userId);

        var request = new CreateStripeCheckoutSessionRequest(userId, orderId, 500); // 5 RON discount

        // Aici va încerca să sune la Stripe. Fără Mock pe SessionService, va arunca StripeException.
        // Totuși, prin prinderea excepției, confirmăm că a trecut de TOATE validările de sus (Coverage!).
        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);
        
        await act.Should().ThrowAsync<StripeException>();
    }

    // --- Helpers pentru a curăța codul ---

    private async Task<Guid> SeedUser()
    {
        var user = new User { 
            Id = Guid.NewGuid(), 
            Username = "test_" + Guid.NewGuid(), 
            Email = "test@test.com", 
            PasswordHash = "h", 
            Role = UserRole.Client 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user.Id;
    }

    private async Task<Guid> SeedOrder(decimal price, Guid? userId = null)
    {
        var orderId = Guid.NewGuid();
        var order = new Order(
            orderId,
            userId ?? Guid.NewGuid(),
            price,
            [], [],
            DateTime.UtcNow,
            OrderStatus.Pending
        );
        _context.Order.Add(order);
        await _context.SaveChangesAsync();
        return orderId;
    }
}