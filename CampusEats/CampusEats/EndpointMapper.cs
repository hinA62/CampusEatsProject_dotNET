using CampusEats.Features.Auth;
using CampusEats.Features.Auth.Handlers;
using CampusEats.Features.Auth.Requests;
using CampusEats.Features.Inventory.Handlers;
using CampusEats.Features.Kitchen.Handlers;
using CampusEats.Features.Kitchen.Requests;
using CampusEats.Features.Loyalty.Handlers;
using CampusEats.Features.Loyalty.Requests;
using CampusEats.Features.Menu.Handlers;
using CampusEats.Features.Menu.Requests;
using CampusEats.Features.Order;
using CampusEats.Features.Order.Handlers;
using CampusEats.Features.Order.Requests;
using CampusEats.Features.Payment.Handlers;
using CampusEats.Features.Payment.Requests;
using CampusEats.Features.User;
using CampusEats.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CampusEats;

public static class EndpointMapper
{
    public static void MapAllEndpoints(this WebApplication app)
    {
        MapMenuEndpoints(app);
        MapMenuItemsEndpoints(app);
        MapOrderEndpoints(app);
        MapKitchenEndpoints(app);
        MapInventoryEndpoints(app);
        MapPaymentEndpoints(app);
        MapLoyaltyEndpoints(app);
        MapUserEndpoints(app);
        MapAuthEndpoints(app);
        MapHealthEndpoints(app);
    }

    private static void MapMenuEndpoints(WebApplication app)
    {
        var publicGroup = app.MapGroup("/api/menu").WithTags("Menu");
        var adminGroup = app.MapGroup("/api/menu").WithTags("Menu")
            .RequireAuthorization(p => p.RequireRole("Admin"));
        
        // Create Menu (Admin only)
        adminGroup.MapPost("/", async (CreateMenuRequest request, CreateMenuHandler handler) => 
                await handler.Handle(request))
            .WithName("CreateMenu")
            .Produces(201)
            .Produces(400)
            .Produces(401)
            .Produces(403);


        // Update Menu (Admin only)
        adminGroup.MapPut("/{id:guid}", async (Guid id, UpdateMenuRequest request, UpdateMenuHandler handler) =>
            {
                // Ensure ID from route matches request
                var requestWithId = request with { Id = id };
                return await handler.Handle(requestWithId);
            })
            .WithName("UpdateMenu")
            .Produces(200)
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(404);


        // Delete Menu (Admin only)
        adminGroup.MapDelete("/{id:guid}", async (Guid id, DeleteMenuHandler handler) => 
                await handler.Handle(new DeleteMenuRequest(id)))
            .WithName("DeleteMenu")
            .Produces(200)
            .Produces(401)
            .Produces(403)
            .Produces(404);


        // Get All Menus (Public - no auth required)
        publicGroup.MapGet("/", async (CampusEatsContext db) =>
            {
                var menus = await db.Menu.ToListAsync();
                return Results.Ok(menus);
            })
            .WithName("GetAllMenus")
            .Produces(200);


        // Get Menu by ID (Public - no auth required)
        publicGroup.MapGet("/{id:guid}", async (Guid id, CampusEatsContext db) =>
            {
                var menu = await db.Menu.FindAsync(id);
                return menu is not null ? Results.Ok(menu) : Results.NotFound();
            })
            .WithName("GetMenuById")
            .Produces(200)
            .Produces(404);
    }
    
    private static void MapMenuItemsEndpoints(WebApplication app)
    {
        var publicGroup = app.MapGroup("/api/menu-items").WithTags("MenuItems");
        var adminGroup = app.MapGroup("/api/menu-items").WithTags("MenuItems")
            .RequireAuthorization(p => p.RequireRole("Admin"));
        
        // Create Menu Item (Admin only)
        adminGroup.MapPost("/", async (CreateItemRequest request, CreateItemHandler handler) =>
                await handler.Handle(request))
            .WithName("CreateMenuItem")
            .Produces(201)
            .Produces(400)
            .Produces(401)
            .Produces(403);


        // Update Menu Item (Admin only)
        adminGroup.MapPut("/{id:guid}", async (Guid id, UpdateItemRequest request, UpdateItemHandler handler) =>
            {
                // Ensure ID from route matches request
                var requestWithId = request with { Id = id };
                return await handler.Handle(requestWithId);
            })
            .WithName("UpdateMenuItem")
            .Produces(200)
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(404);


        // Delete Menu Item (Admin only)
        adminGroup.MapDelete("/{id:guid}", async (Guid id, DeleteItemHandler handler) =>
                await handler.Handle(new DeleteItemRequest(id)))
            .WithName("DeleteMenuItem")
            .Produces(204)
            .Produces(401)
            .Produces(403)
            .Produces(404);


        // Get All Menu Items (Public - no auth required)
        publicGroup.MapGet("/", async (CampusEatsContext db) =>
            {
                var items = await db.MenuItem.ToListAsync();
                return Results.Ok(items);
            })
            .WithName("GetAllMenuItems")
            .Produces(200);


        // Get Menu Item by ID (Public - no auth required)
        publicGroup.MapGet("/{id:guid}", async (Guid id, CampusEatsContext db) =>
            {
                var item = await db.MenuItem.FindAsync(id);
                return item is not null ? Results.Ok(item) : Results.NotFound();
            })
            .WithName("GetMenuItemById")
            .Produces(200)
            .Produces(404);
    }

    private static void MapOrderEndpoints(WebApplication app)
    {
        var publicGroup = app.MapGroup("/api/orders").WithTags("Orders");
        var adminGroup = app.MapGroup("/api/orders").WithTags("Orders")
            .RequireAuthorization(p => p.RequireRole("Admin"));
        var clientGroup = app.MapGroup("/api/orders").WithTags("Orders")
            .RequireAuthorization(p => p.RequireRole("Client", "Admin"));
        
        // Place order (Client only - authenticated users)
        clientGroup.MapPost("/", async (PlaceOrderRequest request, PlaceOrderHandler handler) => 
                await handler.Handle(request))
            .WithName("PlaceOrder")
            .Produces(201)
            .Produces(400)
            .Produces(401)
            .Produces(403);


        // Get all orders (Admin only)
        adminGroup.MapGet("/", async (CampusEatsContext db) =>
        {
            var orders = await db.Order
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            return Results.Ok(orders);
        })
            .WithName("GetAllOrders")
            .Produces(200)
            .Produces(401)
            .Produces(403);

        // Get order by id (Client can see their own orders, Admin can see all)
        publicGroup.MapGet("/{id:guid}", async (Guid id, GetOrderByIdHandler handler) =>
                await handler.Handle(new GetOrderByIdRequest(id)))
            .WithName("GetOrderById")
            .Produces(200)
            .Produces(401)
            .Produces(403)
            .Produces(404);

        // Get order details with client info and items
        publicGroup.MapGet("/{id:guid}/details", async (
            Guid id,
            CampusEatsContext db,
            CancellationToken ct) =>
        {
            var order = await db.Order.FindAsync(new object[] { id }, ct);
            
            if (order == null)
                return Results.NotFound($"Order with ID: {id} not found");

            var client = await db.Users.FindAsync(new object[] { order.ClientId }, ct);
            
            if (client == null)
                return Results.NotFound($"Client not found for order {id}");

            var menus = await db.Menu
                .Where(m => order.MenuIDs.Contains(m.Id))
                .Select(m => new { m.Id, m.Name, m.Price })
                .ToListAsync(ct);
            
            var items = await db.MenuItem
                .Where(i => order.ItemIDs.Contains(i.Id))
                .Select(i => new { i.Id, i.Name, i.Price })
                .ToListAsync(ct);

            var result = new
            {
                order.Id,
                order.ClientId,
                ClientUsername = client.Username,
                ClientEmail = client.Email,
                order.Price,
                order.MenuIDs,
                order.ItemIDs,
                order.CreatedAt,
                Status = order.Status.ToString(),
                Menus = menus,
                Items = items
            };

            return Results.Ok(result);
        })
        .WithName("GetOrderDetails")
        .Produces(200)
        .Produces(401)
        .Produces(403)
        .Produces(404);


    // Get order history for a client (Client can see own, Admin can see all)
    app.MapGet("/api/clients/{clientId:guid}/orders", async (Guid clientId, GetOrderHistoryHandler handler) => 
            await handler.Handle(new GetOrderHistoryRequest(clientId)))
        .WithName("GetOrderHistory")
        .RequireAuthorization(p => p.RequireRole("Client", "Admin"))
        .WithTags("Orders")
        .Produces(200)
        .Produces(401)
        .Produces(403);


    // Cancel pending order (Client can cancel their own orders, Admin can cancel any)
    clientGroup.MapPost("/{id:guid}/cancel", async (Guid id, CancelOrderHandler handler) =>
            await handler.Handle(new CancelOrderRequest(id)))
        .WithName("CancelOrder")
        .Produces(200)
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .Produces(409);
    }
    
    private static void MapKitchenEndpoints(WebApplication app)
    {
        var staffGroup = app.MapGroup("/api/kitchen/orders").WithTags("Kitchen")
            .RequireAuthorization(p => p.RequireRole("Kitchen", "Admin"));
        
        // Get pending/active orders for kitchen view (Kitchen staff only)
        staffGroup.MapGet("/", async (string? status, GetPendingOrdersHandler handler) => 
                await handler.Handle(new GetPendingOrdersRequest(status)))
            .WithName("GetKitchenOrders")
            .Produces(200)
            .Produces(400)
            .Produces(401)
            .Produces(403);


        // Update order status (Kitchen staff only)
        staffGroup.MapPatch("/{id:guid}/status", async (Guid id, OrderStatus newStatus, UpdateOrderStatusHandler handler) =>
                await handler.Handle(new UpdateOrderStatusRequest(id, newStatus)))
            .WithName("UpdateOrderStatus")
            .Produces(200)
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(404);
    }

    private static void MapInventoryEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/api/inventory/{date}").WithTags("Inventory");
        
        // Rebuild inventory (Admin only)
        group.MapPost("/rebuild", async (string date, InventoryHandler svc) =>
            {
                if (!DateOnly.TryParse(date, out var d)) return Results.BadRequest("Invalid date (YYYY-MM-DD).");
                var day = await svc.RebuildAsync(d);
                return Results.Ok(new {
                    day.Date,
                    day.GeneratedAtUtc,
                    Items = day.Items.OrderByDescending(i => i.Count).ThenBy(i => i.Name)
                });
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("RebuildInventory")
            .Produces(200)
            .Produces(400)
            .Produces(401)
            .Produces(403);

        // Get inventory (Kitchen and Admin can view)
        group.MapGet("/", async (string date, InventoryHandler svc) =>
            {
                if (!DateOnly.TryParse(date, out var d)) return Results.BadRequest("Invalid date (YYYY-MM-DD).");
                var day = await svc.GetAsync(d);
                return day is null
                    ? Results.NotFound()
                    : Results.Ok(new {
                        day.Date,
                        day.GeneratedAtUtc,
                        Items = day.Items.OrderByDescending(i => i.Count).ThenBy(i => i.Name)
                    });
            })
            .RequireAuthorization(policy => policy.RequireRole("Kitchen", "Admin"))
            .WithName("GetInventory")
            .Produces(200)
            .Produces(404)
            .Produces(400);
    }
    
    private static void MapPaymentEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/api/payments").WithTags("Payments");
        var clientGroup = app.MapGroup("/api/payments").WithTags("Payments")
            .RequireAuthorization(p => p.RequireRole("Client", "Admin"));
        
        // Create payment (Client/Admin)
        clientGroup.MapPost("/", async (
                CreatePaymentRequest request,
                CreatePaymentHandler handler,
                IValidator<CreatePaymentRequest> validator,
                CancellationToken ct) =>
            {
                var validation = await validator.ValidateAsync(request, ct);
                if (!validation.IsValid)
                    return Results.ValidationProblem(validation.ToDictionary());

                return await handler.Handle(request, ct);
            })
            .WithName("CreatePayment")
            .Produces(201)
            .Produces(400)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403);

        // Get payment by id
        clientGroup.MapGet("/{id:guid}", async (Guid id, GetPaymentByIdHandler handler) =>
                await handler.Handle(new GetPaymentByIdRequest(id)))
            .WithName("GetPaymentById")
            .Produces(200)
            .Produces(404)
            .Produces(401)
            .Produces(403);

        // Get payment history for a user
        clientGroup.MapGet("/{userId:guid}/payments", async (Guid userId, GetPaymentHistoryHandler handler) =>
                await handler.Handle(new GetPaymentHistoryRequest(userId)))
            .WithName("GetUserPayments")
            .Produces(200)
            .Produces(401)
            .Produces(403);

        // Stripe: create checkout session
        clientGroup.MapPost("/stripe/checkout-session", async (
                CreateStripeCheckoutSessionRequest request,
                CreateStripeCheckoutSessionHandler handler,
                CancellationToken ct) =>
            {
                return await handler.Handle(request, ct);
            })
            .WithName("CreateStripeCheckoutSession")
            .Produces(200)
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(404);
        
        // Stripe webhook (public endpoint)
        group.MapPost("/stripe/webhook", (
                    HttpRequest request,
                    StripeWebhookHandler handler,
                    CancellationToken ct) =>
                handler.Handle(request, ct))
            .AllowAnonymous()
            .WithName("StripeWebhook");
    }

    private static void MapLoyaltyEndpoints(WebApplication app)
    {
        var clientGroup = app.MapGroup("/api/loyalty").WithTags("Loyalty")
            .RequireAuthorization(p => p.RequireRole("Client", "Admin"));
        
        // Get loyalty balance
        clientGroup.MapGet("/{userId:guid}/balance", async (Guid userId, GetLoyaltyBalanceHandler handler) =>
                await handler.Handle(new GetLoyaltyBalanceRequest(userId)))
            .WithName("GetLoyaltyBalance")
            .Produces(200)
            .Produces(401)
            .Produces(403);
        
        // Redeem points
        clientGroup.MapPost("/redeem", async (
                RedeemPointsRequest request,
                RedeemPointsHandler handler,
                IValidator<RedeemPointsRequest> validator,
                CancellationToken ct) =>
            {
                var validation = await validator.ValidateAsync(request, ct);
                if (!validation.IsValid)
                    return Results.ValidationProblem(validation.ToDictionary());

                return await handler.Handle(request, ct);
            })
            .WithName("RedeemPoints")
            .Produces(200)
            .Produces(400)
            .ProducesValidationProblem()
            .Produces(401)
            .Produces(403);

        // Loyalty transactions history
        clientGroup.MapGet("/{userId:guid}/transactions", async (Guid userId, CampusEatsContext db, CancellationToken ct) =>
            {
                var txs = await db.LoyaltyTransactions
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.CreatedAtUtc)
                    .ToListAsync(ct);

                return Results.Ok(txs);
            })
            .WithName("GetLoyaltyTransactions")
            .Produces(200)
            .Produces(401)
            .Produces(403);
    }

    private static void MapUserEndpoints(WebApplication app)
    {
        var adminGroup = app.MapGroup("/api/users").WithTags("Users")
                .RequireAuthorization(p => p.RequireRole("Admin"));
            
        // Get All Users (Admin only)
        adminGroup.MapGet("/", async (
                CampusEatsContext db,
                CancellationToken ct) =>
            {
                var users = await db.Users
                    .Select(u => new
                    {
                        u.Id,
                        u.Username,
                        u.Email,
                        Role = u.Role.ToString(),
                        u.CreatedAt
                    })
                    .ToListAsync(ct);

                return Results.Ok(users);
            })
            .WithName("GetAllUsers")
            .Produces(200)
            .Produces(401)
            .Produces(403);

        // Get User by ID (Admin only)
        adminGroup.MapGet("/{userId:guid}", async (
                Guid userId,
                CampusEatsContext db,
                CancellationToken ct) =>
            {
                var user = await db.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new
                    {
                        u.Id,
                        u.Username,
                        u.Email,
                        Role = u.Role.ToString(),
                        u.CreatedAt
                    })
                    .FirstOrDefaultAsync(ct);

                if (user == null)
                    return Results.NotFound("User not found");

                return Results.Ok(user);
            })
            .WithName("GetUserById")
            .Produces(200)
            .Produces(404)
            .Produces(401)
            .Produces(403);
    }
    
    private static void MapAuthEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");
        
        // Register User
        group.MapPost("/register", async (
            RegisterUserRequest request,
            CampusEatsContext db,
            IValidator<RegisterUserRequest> validator,
            CancellationToken ct) =>
        {
            // Validare
            var validationResult = await validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            // Verificare email existent
            if (await db.Users.AnyAsync(u => u.Email == request.Email, ct))
                return Results.BadRequest("Email already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = UserRole.Client, // Automat setat ca Client
                CreatedAt = DateTime.UtcNow
            };

            db.Users.Add(user);
            await db.SaveChangesAsync(ct);

            return Results.Created(
                $"/api/users/{user.Id}",
                new { user.Id, user.Username, user.Email, Role = user.Role.ToString() }
            );
        })
        .WithName("RegisterUser")
        .Produces(201)
        .Produces(400)
        .ProducesValidationProblem();

        // Login User
        group.MapPost("/login", async (
            LoginUserRequest request,
            CampusEatsContext db,
            JwtService jwtService,
            CancellationToken ct) =>
        {
            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Results.Unauthorized();

            var token = jwtService.GenerateToken(user);

            return Results.Ok(new
            {
                token,
                userId = user.Id,
                username = user.Username,
                role = user.Role.ToString()
            });
        })
        .WithName("LoginUser")
        .Produces(200)
        .Produces(401);
        
        // Logout User (Client-side token deletion, informative endpoint)
        group.MapPost("/logout", () =>
        {
            // JWT logout se face pe client-side prin ștergerea token-ului

            return Results.Ok(new 
            { 
                message = "Logged out successfully. Please delete the token on the client side." 
            });
        })
        .RequireAuthorization()
        .WithName("LogoutUser")
        .Produces(200)
        .Produces(401);


        // Change Password
        group.MapPost("/change-password", async (
            ChangePasswordRequest request,
            ChangePasswordHandler handler,
            IValidator<ChangePasswordRequest> validator,
            CancellationToken ct) =>
        {
            // Validare
            var validationResult = await validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            return await handler.Handle(request, ct);
        })
        .RequireAuthorization()
        .WithName("ChangePassword")
        .Produces(200)
        .Produces(400)
        .Produces(404)
        .ProducesValidationProblem()
        .Produces(401);


        // Update User Role (Admin only)
        app.MapPatch("/api/users/{userId:guid}/role", async (
            Guid userId,
            UpdateUserRoleRequest request,
            CampusEatsContext db,
            IValidator<UpdateUserRoleRequest> validator,
            CancellationToken ct) =>
        {
            var requestWithId = request with { UserId = userId };
            
            // Validare
            var validationResult = await validator.ValidateAsync(requestWithId, ct);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            // Găsire utilizator
            var user = await db.Users.FindAsync(new object[] { userId }, ct);
            if (user is null)
                return Results.NotFound("User not found");

            // Parse rol (validarea e deja făcută de validator)
            var newRole = Enum.Parse<UserRole>(requestWithId.NewRole, true);
            
            user.Role = newRole;
            await db.SaveChangesAsync(ct);

            return Results.Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                Role = user.Role.ToString(),
                Message = $"User role successfully updated to {newRole}"
            });
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"))
        .WithName("UpdateUserRole")
        .WithTags("Authentication")
        .Produces(200)
        .Produces(400)
        .Produces(404)
        .ProducesValidationProblem()
        .Produces(401)
        .Produces(403); 
    }
    
    private static void MapHealthEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/health").WithTags("System");
        
        group.MapGet("/", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
            .WithName("HealthCheck")
            .Produces(200);
    }
}