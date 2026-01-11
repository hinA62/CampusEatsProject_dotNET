using CampusEats.Features.Auth;
using CampusEats.Features.Auth.Handlers;
using CampusEats.Features.Inventory.Handlers;
using CampusEats.Features.Kitchen.Handlers;
using CampusEats.Features.Loyalty.Handlers;
using CampusEats.Features.Menu.Handlers;
using CampusEats.Features.Order.Handlers;
using CampusEats.Features.Payment.Handlers;

namespace CampusEats;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Menu Handlers
        services.AddScoped<CreateMenuHandler>();
        services.AddScoped<UpdateMenuHandler>();
        services.AddScoped<DeleteMenuHandler>();
        services.AddScoped<CreateItemHandler>();
        services.AddScoped<UpdateItemHandler>();
        services.AddScoped<DeleteItemHandler>();

        // Order & Kitchen Handlers
        services.AddScoped<PlaceOrderHandler>();
        services.AddScoped<GetOrderHistoryHandler>();
        services.AddScoped<GetOrderByIdHandler>();
        services.AddScoped<CancelOrderHandler>();
        services.AddScoped<GetPendingOrdersHandler>();
        services.AddScoped<UpdateOrderStatusHandler>();

        // System & Auth
        services.AddScoped<InventoryHandler>();
        services.AddScoped<JwtService>();
        services.AddScoped<ChangePasswordHandler>();

        // Payment & Loyalty
        services.AddScoped<CreatePaymentHandler>();
        services.AddScoped<GetPaymentByIdHandler>();
        services.AddScoped<GetPaymentHistoryHandler>();
        services.AddScoped<GetLoyaltyBalanceHandler>();
        services.AddScoped<RedeemPointsHandler>();
        services.AddScoped<CreateStripeCheckoutSessionHandler>();
        services.AddScoped<StripeWebhookHandler>();

        return services;
    }
}