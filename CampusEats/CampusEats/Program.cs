using CampusEats.Features.Menu.Handlers;
using CampusEats.Features.Menu.Requests;
using CampusEats.Features.Order;
using CampusEats.Features.Order.Requests;
using CampusEats.Features.Order.Handlers;
using CampusEats.Features.Kitchen.Handlers;
using CampusEats.Features.Kitchen.Requests;
using CampusEats.Features.Inventory;
using CampusEats.Features.Auth;
using CampusEats.Features.Auth.Requests;
using CampusEats.Features.User;
using CampusEats.Validators.Auth;
using CampusEats.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text.Json.Serialization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON options to support string enums
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Postgresql Database with JSON support
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Create data_source with JSON support
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<CampusEatsContext>(options =>
    options.UseNpgsql(dataSource));

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Register Handlers
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
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<JwtService>();

// Add CORS (optional - useful for Blazor)
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


// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Apply migrations automatically on startup (Development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CampusEatsContext>();
    await db.Database.MigrateAsync();
    app.Logger.LogInformation("Database migrations applied successfully");
}



// ============================================
// MENU ENDPOINTS
// ============================================

// Create Menu (Admin only)
app.MapPost("/api/menu", async (CreateMenuRequest request, CreateMenuHandler handler) => 
        await handler.Handle(request))
.RequireAuthorization(policy => policy.RequireRole("Admin"))
.WithName("CreateMenu")
.WithTags("Menu")
.Produces(201)
.Produces(400)
.Produces(401)
.Produces(403);


// Update Menu (Admin only)
app.MapPut("/api/menu/{id:guid}", async (Guid id, UpdateMenuRequest request, UpdateMenuHandler handler) =>
{
    // Ensure ID from route matches request
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


// Delete Menu (Admin only)
app.MapDelete("/api/menu/{id:guid}", async (Guid id, DeleteMenuHandler handler) => 
        await handler.Handle(new DeleteMenuRequest(id)))
.RequireAuthorization(policy => policy.RequireRole("Admin"))
.WithName("DeleteMenu")
.WithTags("Menu")
.Produces(200)
.Produces(401)
.Produces(403)
.Produces(404);


// Get All Menus (Public - no auth required)
app.MapGet("/api/menu", async (CampusEatsContext db) =>
{
    var menus = await db.Menu.ToListAsync();
    return Results.Ok(menus);
})
.WithName("GetAllMenus")
.WithTags("Menu")
.Produces(200);


// Get Menu by ID (Public - no auth required)
app.MapGet("/api/menu/{id:guid}", async (Guid id, CampusEatsContext db) =>
{
    var menu = await db.Menu.FindAsync(id);
    return menu is not null ? Results.Ok(menu) : Results.NotFound();
})
.WithName("GetMenuById")
.WithTags("Menu")
.Produces(200)
.Produces(404);



// ============================================
// MENU ITEM ENDPOINTS
// ============================================

// Create Menu Item (Admin only)
app.MapPost("/api/menu-items", async (CreateItemRequest request, CreateItemHandler handler) =>
        await handler.Handle(request))
.RequireAuthorization(policy => policy.RequireRole("Admin"))
.WithName("CreateMenuItem")
.WithTags("MenuItems")
.Produces(201)
.Produces(400)
.Produces(401)
.Produces(403);


// Update Menu Item (Admin only)
app.MapPut("/api/menu-items/{id:guid}", async (Guid id, UpdateItemRequest request, UpdateItemHandler handler) =>
{
    // Ensure ID from route matches request
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


// Delete Menu Item (Admin only)
app.MapDelete("/api/menu-items/{id:guid}", async (Guid id, DeleteItemHandler handler) =>
        await handler.Handle(new DeleteItemRequest(id)))
.RequireAuthorization(policy => policy.RequireRole("Admin"))
.WithName("DeleteMenuItem")
.WithTags("MenuItems")
.Produces(204)
.Produces(401)
.Produces(403)
.Produces(404);


// Get All Menu Items (Public - no auth required)
app.MapGet("/api/menu-items", async (CampusEatsContext db) =>
{
    var items = await db.MenuItem.ToListAsync();
    return Results.Ok(items);
})
.WithName("GetAllMenuItems")
.WithTags("MenuItems")
.Produces(200);


// Get Menu Item by ID (Public - no auth required)
app.MapGet("/api/menu-items/{id:guid}", async (Guid id, CampusEatsContext db) =>
{
    var item = await db.MenuItem.FindAsync(id);
    return item is not null ? Results.Ok(item) : Results.NotFound();
})
.WithName("GetMenuItemById")
.WithTags("MenuItems")
.Produces(200)
.Produces(404);



// ============================================
// ORDER ENDPOINTS
// ============================================

// Place order (Client only - authenticated users)
app.MapPost("/api/orders", async (PlaceOrderRequest request, PlaceOrderHandler handler) => 
        await handler.Handle(request))
    .RequireAuthorization(policy => policy.RequireRole("Client", "Admin"))
    .WithName("PlaceOrder")
    .WithTags("Orders")
    .Produces(201)
    .Produces(400)
    .Produces(401)
    .Produces(403);


// Get order by id (Client can see own orders, Admin can see all)
app.MapGet("/api/orders/{id:guid}", async (Guid id, GetOrderByIdHandler handler) =>
        await handler.Handle(new GetOrderByIdRequest(id)))
    .RequireAuthorization(policy => policy.RequireRole("Client", "Admin", "Kitchen"))
    .WithName("GetOrderById")
    .WithTags("Orders")
    .Produces(200)
    .Produces(401)
    .Produces(403)
    .Produces(404);


// Get order history for a client (Client can see own, Admin can see all)
app.MapGet("/api/clients/{clientId:guid}/orders", async (Guid clientId, GetOrderHistoryHandler handler) => 
        await handler.Handle(new GetOrderHistoryRequest(clientId)))
    .RequireAuthorization(policy => policy.RequireRole("Client", "Admin"))
    .WithName("GetOrderHistory")
    .WithTags("Orders")
    .Produces(200)
    .Produces(401)
    .Produces(403);


// Cancel pending order (Client can cancel own orders, Admin can cancel any)
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



// ============================================
// KITCHEN ENDPOINTS
// ============================================

// Get pending/active orders for kitchen view (Kitchen staff only)
app.MapGet("/api/kitchen/orders", async (string? status, GetPendingOrdersHandler handler) => 
        await handler.Handle(new GetPendingOrdersRequest(status)))
    .RequireAuthorization(policy => policy.RequireRole("Kitchen", "Admin"))
    .WithName("GetKitchenOrders")
    .WithTags("Kitchen")
    .Produces(200)
    .Produces(400)
    .Produces(401)
    .Produces(403);


// Update order status (Kitchen staff only)
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



// ============================================
// INVENTORY ENDPOINTS (LEGACY)
// ============================================

// Rebuild inventory (Admin only)
app.MapPost("/api/inventory/{date}/rebuild", async (string date, InventoryService svc) =>
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

// Get inventory (Kitchen and Admin can view)
app.MapGet("/api/inventory/{date}", async (string date, InventoryService svc) =>
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
    .Produces(401)
    .Produces(403)
    .Produces(404)
    .Produces(400);

// ============================================
// AUTH ENDPOINTS
// ============================================

// Register User
app.MapPost("/api/auth/register", async (
    RegisterUserRequest request,
    CampusEatsContext db,
    IValidator<RegisterUserRequest> validator,  // ← Injectează validatorul
    CancellationToken ct) =>
{
    // Validare
    var validationResult = await validator.ValidateAsync(request, ct);
    if (!validationResult.IsValid)
        return Results.ValidationProblem(validationResult.ToDictionary());

    // Verificare email existent
    if (await db.Users.AnyAsync(u => u.Email == request.Email, ct))
        return Results.BadRequest("Email already exists");

    if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        return Results.BadRequest("Invalid role");

    var user = new CampusEats.Features.User.User
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
.ProducesValidationProblem();  // ← Adaugă response pentru validare


// Login User
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


// ============================================
// HEALTH CHECK
// ============================================

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
.WithName("HealthCheck")
.WithTags("System")
.Produces(200);

app.Logger.LogInformation("CampusEats API is starting...");
app.Logger.LogInformation("Swagger UI: http://localhost:5298/swagger");

app.Run();
