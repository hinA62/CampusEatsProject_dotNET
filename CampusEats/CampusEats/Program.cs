using CampusEats.Features.Menu.Handlers;
using CampusEats.Features.Menu.Requests;
using CampusEats.Features.Order;
using CampusEats.Features.Order.Requests;
using CampusEats.Features.Order.Handlers;
using CampusEats.Features.Kitchen.Handlers;
using CampusEats.Features.Kitchen.Requests;
using CampusEats.Features.Auth;
using CampusEats.Features.Auth.Requests;
using CampusEats.Features.Auth.Handlers;
using CampusEats.Features.User;
using CampusEats.Persistence;
using CampusEats.Features.Payment.Requests;
using CampusEats.Features.Payment.Handlers;
using CampusEats.Features.Loyalty.Requests;
using CampusEats.Features.Loyalty.Handlers;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text.Json.Serialization;
using System.Text;
using CampusEats.Features.Inventory.Handlers;

var builder = WebApplication.CreateBuilder(args);


builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<CampusEatsContext>(options =>
    options.UseNpgsql(dataSource));


builder.Services.AddValidatorsFromAssemblyContaining<Program>();


builder.Services.AddScoped<CreateMenuHandler>();
builder.Services.AddScoped<UpdateMenuHandler>();
builder.Services.AddScoped<DeleteMenuHandler>();
builder.Services.AddScoped<CreateItemHandler>();
builder.Services.AddScoped<UpdateItemHandler>();
builder.Services.AddScoped<DeleteItemHandler>();
builder.Services.AddScoped<PlaceOrderHandler>();
builder.Services.AddScoped<GetOrderHistoryHandler>();
builder.Services.AddScoped<GetOrderByIdHandler>();
builder.Services.AddScoped<CancelOrderHandler>();
builder.Services.AddScoped<GetPendingOrdersHandler>();
builder.Services.AddScoped<UpdateOrderStatusHandler>();
builder.Services.AddScoped<InventoryHandler>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<ChangePasswordHandler>();
builder.Services.AddScoped<CreatePaymentHandler>();
builder.Services.AddScoped<CreatePaymentHandler>();
builder.Services.AddScoped<GetPaymentByIdHandler>();
builder.Services.AddScoped<GetPaymentHistoryHandler>();
builder.Services.AddScoped<GetLoyaltyBalanceHandler>();
builder.Services.AddScoped<RedeemPointsHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();
var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CampusEatsContext>();
    await db.Database.MigrateAsync();
    app.Logger.LogInformation("Database migrations applied successfully");
}








app.MapPost("/api/menu", async (CreateMenuRequest request, CreateMenuHandler handler) => 
        await handler.Handle(request))
.RequireAuthorization(policy => policy.RequireRole("Admin"))
.WithName("CreateMenu")
.WithTags("Menu")
.Produces(201)
.Produces(400)
.Produces(401)
.Produces(403);



app.MapPut("/api/menu/{id:guid}", async (Guid id, UpdateMenuRequest request, UpdateMenuHandler handler) =>
{

    var requestWithId = request with { Id = id };
    return await handler.Handle(requestWithId);
})
.RequireAuthorization(policy => policy.RequireRole("Admin"))
.WithName("UpdateMenu")
.WithTags("Menu")
.Produces(200)
.Produces(400)
.Produces(401)
.Produces(403)
.Produces(404);



app.MapDelete("/api/menu/{id:guid}", async (Guid id, DeleteMenuHandler handler) => 
        await handler.Handle(new DeleteMenuRequest(id)))
.RequireAuthorization(policy => policy.RequireRole("Admin"))
.WithName("DeleteMenu")
.WithTags("Menu")
.Produces(200)
.Produces(401)
.Produces(403)
.Produces(404);



app.MapGet("/api/menu", async (CampusEatsContext db) =>
{
    var menus = await db.Menu.ToListAsync();
    return Results.Ok(menus);
})
.WithName("GetAllMenus")
.WithTags("Menu")
.Produces(200);



app.MapGet("/api/menu/{id:guid}", async (Guid id, CampusEatsContext db) =>
{
    var menu = await db.Menu.FindAsync(id);
    return menu is not null ? Results.Ok(menu) : Results.NotFound();
})
.WithName("GetMenuById")
.WithTags("Menu")
.Produces(200)
.Produces(404);








app.MapPost("/api/menu-items", async (CreateItemRequest request, CreateItemHandler handler) =>
        await handler.Handle(request))
.RequireAuthorization(policy => policy.RequireRole("Admin"))
.WithName("CreateMenuItem")
.WithTags("MenuItems")
.Produces(201)
.Produces(400)
.Produces(401)
.Produces(403);



app.MapPut("/api/menu-items/{id:guid}", async (Guid id, UpdateItemRequest request, UpdateItemHandler handler) =>
{

    var requestWithId = request with { Id = id };
    return await handler.Handle(requestWithId);
})
.RequireAuthorization(policy => policy.RequireRole("Admin"))
.WithName("UpdateMenuItem")
.WithTags("MenuItems")
.Produces(200)
.Produces(400)
.Produces(401)
.Produces(403)
.Produces(404);



app.MapDelete("/api/menu-items/{id:guid}", async (Guid id, DeleteItemHandler handler) =>
        await handler.Handle(new DeleteItemRequest(id)))
.RequireAuthorization(policy => policy.RequireRole("Admin"))
.WithName("DeleteMenuItem")
.WithTags("MenuItems")
.Produces(204)
.Produces(401)
.Produces(403)
.Produces(404);



app.MapGet("/api/menu-items", async (CampusEatsContext db) =>
{
    var items = await db.MenuItem.ToListAsync();
    return Results.Ok(items);
})
.WithName("GetAllMenuItems")
.WithTags("MenuItems")
.Produces(200);



app.MapGet("/api/menu-items/{id:guid}", async (Guid id, CampusEatsContext db) =>
{
    var item = await db.MenuItem.FindAsync(id);
    return item is not null ? Results.Ok(item) : Results.NotFound();
})
.WithName("GetMenuItemById")
.WithTags("MenuItems")
.Produces(200)
.Produces(404);








app.MapPost("/api/orders", async (PlaceOrderRequest request, PlaceOrderHandler handler) => 
        await handler.Handle(request))
    .RequireAuthorization(policy => policy.RequireRole("Client", "Admin"))
    .WithName("PlaceOrder")
    .WithTags("Orders")
    .Produces(201)
    .Produces(400)
    .Produces(401)
    .Produces(403);



app.MapGet("/api/orders", async (CampusEatsContext db) =>
{
    var orders = await db.Order
        .OrderByDescending(o => o.CreatedAt)
        .ToListAsync();
    return Results.Ok(orders);
})
    .RequireAuthorization(policy => policy.RequireRole("Admin"))
    .WithName("GetAllOrders")
    .WithTags("Orders")
    .Produces(200)
    .Produces(401)
    .Produces(403);


app.MapGet("/api/orders/{id:guid}", async (Guid id, GetOrderByIdHandler handler) =>
        await handler.Handle(new GetOrderByIdRequest(id)))
    .RequireAuthorization(policy => policy.RequireRole("Client", "Admin", "Kitchen"))
    .WithName("GetOrderById")
    .WithTags("Orders")
    .Produces(200)
    .Produces(401)
    .Produces(403)
    .Produces(404);



app.MapGet("/api/clients/{clientId:guid}/orders", async (Guid clientId, GetOrderHistoryHandler handler) => 
        await handler.Handle(new GetOrderHistoryRequest(clientId)))
    .RequireAuthorization(policy => policy.RequireRole("Client", "Admin"))
    .WithName("GetOrderHistory")
    .WithTags("Orders")
    .Produces(200)
    .Produces(401)
    .Produces(403);



app.MapPost("/api/orders/{id:guid}/cancel", async (Guid id, CancelOrderHandler handler) =>
        await handler.Handle(new CancelOrderRequest(id)))
    .RequireAuthorization(policy => policy.RequireRole("Client", "Admin"))
    .WithName("CancelOrder")
    .WithTags("Orders")
    .Produces(200)
    .Produces(401)
    .Produces(403)
    .Produces(404)
    .Produces(409);








app.MapGet("/api/kitchen/orders", async (string? status, GetPendingOrdersHandler handler) => 
        await handler.Handle(new GetPendingOrdersRequest(status)))
    .RequireAuthorization(policy => policy.RequireRole("Kitchen", "Admin"))
    .WithName("GetKitchenOrders")
    .WithTags("Kitchen")
    .Produces(200)
    .Produces(400)
    .Produces(401)
    .Produces(403);



app.MapPatch("/api/kitchen/orders/{id:guid}/status", async (Guid id, OrderStatus newStatus, UpdateOrderStatusHandler handler) =>
        await handler.Handle(new UpdateOrderStatusRequest(id, newStatus)))
    .RequireAuthorization(policy => policy.RequireRole("Kitchen", "Admin"))
    .WithName("UpdateOrderStatus")
    .WithTags("Kitchen")
    .Produces(200)
    .Produces(400)
    .Produces(401)
    .Produces(403)
    .Produces(404);








app.MapPost("/api/inventory/{date}/rebuild", async (string date, InventoryHandler svc) =>
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
    .WithTags("Inventory")
    .Produces(200)
    .Produces(400)
    .Produces(401)
    .Produces(403);


app.MapGet("/api/inventory/{date}", async (string date, InventoryHandler svc) =>
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
    .WithTags("Inventory")
    .Produces(200)
    .Produces(404)
    .Produces(400);








app.MapPost("/api/payments", async (
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
    .RequireAuthorization(policy => policy.RequireRole("Client", "Admin"))
    .WithName("CreatePayment")
    .WithTags("Payments")
    .Produces(201)
    .Produces(400)
    .ProducesValidationProblem()
    .Produces(401)
    .Produces(403);


app.MapGet("/api/payments/{id:guid}", async (Guid id, GetPaymentByIdHandler handler) =>
        await handler.Handle(new GetPaymentByIdRequest(id)))
    .RequireAuthorization(policy => policy.RequireRole("Client", "Admin"))
    .WithName("GetPaymentById")
    .WithTags("Payments")
    .Produces(200)
    .Produces(404)
    .Produces(401)
    .Produces(403);


app.MapGet("/api/users/{userId:guid}/payments", async (Guid userId, GetPaymentHistoryHandler handler) =>
        await handler.Handle(new GetPaymentHistoryRequest(userId)))
    .RequireAuthorization(policy => policy.RequireRole("Client", "Admin"))
    .WithName("GetUserPayments")
    .WithTags("Payments")
    .Produces(200)
    .Produces(401)
    .Produces(403);







app.MapGet("/api/loyalty/{userId:guid}/balance", async (Guid userId, GetLoyaltyBalanceHandler handler) =>
        await handler.Handle(new GetLoyaltyBalanceRequest(userId)))
    .RequireAuthorization(policy => policy.RequireRole("Client", "Admin"))
    .WithName("GetLoyaltyBalance")
    .WithTags("Loyalty")
    .Produces(200)
    .Produces(401)
    .Produces(403);



app.MapPost("/api/loyalty/redeem", async (
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
    .RequireAuthorization(policy => policy.RequireRole("Client", "Admin"))
    .WithName("RedeemPoints")
    .WithTags("Loyalty")
    .Produces(200)
    .Produces(400)
    .ProducesValidationProblem()
    .Produces(401)
    .Produces(403);


app.MapGet("/api/loyalty/{userId:guid}/transactions", async (Guid userId, CampusEatsContext db, CancellationToken ct) =>
    {
        var txs = await db.LoyaltyTransactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToListAsync(ct);

        return Results.Ok(txs);
    })
    .RequireAuthorization(policy => policy.RequireRole("Client", "Admin"))
    .WithName("GetLoyaltyTransactions")
    .WithTags("Loyalty")
    .Produces(200)
    .Produces(401)
    .Produces(403);







app.MapPost("/api/auth/register", async (
    RegisterUserRequest request,
    CampusEatsContext db,
    IValidator<RegisterUserRequest> validator,
    CancellationToken ct) =>
{

    var validationResult = await validator.ValidateAsync(request, ct);
    if (!validationResult.IsValid)
        return Results.ValidationProblem(validationResult.ToDictionary());


    if (await db.Users.AnyAsync(u => u.Email == request.Email, ct))
        return Results.BadRequest("Email already exists");

    if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        return Results.BadRequest("Invalid role");

    var user = new User
    {
        Id = Guid.NewGuid(),
        Username = request.Username,
        Email = request.Email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        Role = role,
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
.WithTags("Auth")
.Produces(201)
.Produces(400)
.ProducesValidationProblem();



app.MapPost("/api/auth/login", async (
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
.WithTags("Auth")
.Produces(200)
.Produces(401);



app.MapPost("/api/auth/logout", () =>
{


    return Results.Ok(new 
    { 
        message = "Logged out successfully. Please delete the token on the client side." 
    });
})
.RequireAuthorization()
.WithName("LogoutUser")
.WithTags("Auth")
.Produces(200)
.Produces(401);



app.MapPost("/api/auth/change-password", async (
    ChangePasswordRequest request,
    ChangePasswordHandler handler,
    IValidator<ChangePasswordRequest> validator,
    CancellationToken ct) =>
{

    var validationResult = await validator.ValidateAsync(request, ct);
    if (!validationResult.IsValid)
        return Results.ValidationProblem(validationResult.ToDictionary());

    return await handler.Handle(request, ct);
})
.RequireAuthorization()
.WithName("ChangePassword")
.WithTags("Auth")
.Produces(200)
.Produces(400)
.Produces(404)
.ProducesValidationProblem()
.Produces(401);






app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
.WithName("HealthCheck")
.WithTags("System")
.Produces(200);

app.Logger.LogInformation("CampusEats API is starting...");
app.Logger.LogInformation("Swagger UI: http://localhost:5298/swagger");

app.Run();
