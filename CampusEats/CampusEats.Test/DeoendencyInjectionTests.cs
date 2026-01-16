using CampusEats.Features.Auth;
using CampusEats.Features.Auth.Handlers;
using CampusEats.Features.Inventory.Handlers;
using CampusEats.Features.Kitchen.Handlers;
using CampusEats.Features.Loyalty.Handlers;
using CampusEats.Features.Menu.Handlers;
using CampusEats.Features.Order.Handlers;
using CampusEats.Features.Payment.Handlers;
using CampusEats.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using FluentAssertions;

namespace CampusEats.Test;

public class DependencyInjectionTests
{
    [Fact]
    public void AddApplicationServices_ShouldRegisterAllHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Handlers-ele au dependențe (DbContext, IConfiguration, etc.)
        // Trebuie să le adăugăm mock-uite pentru a permite instanțierea handler-elor
        SetupMockDependencies(services);

        // Act
        services.AddApplicationServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert - Verificăm manual câteva categorii cheie
        
        // Menu
        serviceProvider.GetService<CreateMenuHandler>().Should().NotBeNull();
        serviceProvider.GetService<CreateItemHandler>().Should().NotBeNull();

        // Order
        serviceProvider.GetService<PlaceOrderHandler>().Should().NotBeNull();
        serviceProvider.GetService<UpdateOrderStatusHandler>().Should().NotBeNull();

        // Payment & Loyalty
        serviceProvider.GetService<CreatePaymentHandler>().Should().NotBeNull();
        serviceProvider.GetService<StripeWebhookHandler>().Should().NotBeNull();
        serviceProvider.GetService<GetLoyaltyBalanceHandler>().Should().NotBeNull();

        // Auth
        serviceProvider.GetService<JwtService>().Should().NotBeNull();
    }

    [Theory]
    [InlineData(typeof(CreateMenuHandler))]
    [InlineData(typeof(UpdateMenuHandler))]
    [InlineData(typeof(DeleteMenuHandler))]
    [InlineData(typeof(PlaceOrderHandler))]
    [InlineData(typeof(InventoryHandler))]
    [InlineData(typeof(JwtService))]
    [InlineData(typeof(CreatePaymentHandler))]
    [InlineData(typeof(StripeWebhookHandler))]
    [InlineData(typeof(CreateStripeCheckoutSessionHandler))]
    public void Handler_ShouldBeRegisteredAsScoped(Type handlerType)
    {
        // Arrange
        var services = new ServiceCollection();
        SetupMockDependencies(services);
        services.AddApplicationServices();
        
        // Verificăm dacă serviciul este înregistrat cu ServiceDescriptor corect
        var descriptor = services.FirstOrDefault(d => d.ServiceType == handlerType);
        
        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    private static void SetupMockDependencies(IServiceCollection services)
    {
        services.AddLogging();

        var options = new DbContextOptionsBuilder<CampusEatsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        services.AddScoped(_ => new CampusEatsContext(options));

        var mockConfig = new Mock<IConfiguration>();
        services.AddSingleton(mockConfig.Object);
    
    }
}