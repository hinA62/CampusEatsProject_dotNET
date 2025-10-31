using CampusEats.Features.Menu.Handlers;
using CampusEats.Features.Menu.Requests;
using CampusEats.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON options to support string enums
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure PostgreSQL Database with JSON support
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Create data source with JSON support
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

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

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

// Create Menu
app.MapPost("/api/menu", async (CreateMenuRequest request, CreateMenuHandler handler) =>
{
    return await handler.Handle(request);
})
.WithName("CreateMenu")
.WithTags("Menu")
.Produces(201)
.Produces(400);

// Update Menu
app.MapPut("/api/menu/{id:guid}", async (Guid id, UpdateMenuRequest request, UpdateMenuHandler handler) =>
{
    // Ensure ID from route matches request
    var requestWithId = request with { Id = id };
    return await handler.Handle(requestWithId);
})
.WithName("UpdateMenu")
.WithTags("Menu")
.Produces(200)
.Produces(400)
.Produces(404);

// Delete Menu
app.MapDelete("/api/menu/{id:guid}", async (Guid id, DeleteMenuHandler handler) =>
{
    return await handler.Handle(new DeleteMenuRequest(id));
})
.WithName("DeleteMenu")
.WithTags("Menu")
.Produces(200)
.Produces(404);

// Get All Menus
app.MapGet("/api/menu", async (CampusEatsContext db) =>
{
    var menus = await db.Menu.ToListAsync();
    return Results.Ok(menus);
})
.WithName("GetAllMenus")
.WithTags("Menu")
.Produces(200);

// Get Menu by ID
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

// Create Menu Item
app.MapPost("/api/menu-items", async (CreateItemRequest request, CreateItemHandler handler) =>
{
    return await handler.Handle(request);
})
.WithName("CreateMenuItem")
.WithTags("MenuItems")
.Produces(201)
.Produces(400);

// Update Menu Item
app.MapPut("/api/menu-items/{id:guid}", async (Guid id, UpdateItemRequest request, UpdateItemHandler handler) =>
{
    // Ensure ID from route matches request
    var requestWithId = request with { Id = id };
    return await handler.Handle(requestWithId);
})
.WithName("UpdateMenuItem")
.WithTags("MenuItems")
.Produces(200)
.Produces(400)
.Produces(404);

// Delete Menu Item
app.MapDelete("/api/menu-items/{id:guid}", async (Guid id, DeleteItemHandler handler) =>
{
    return await handler.Handle(new DeleteItemRequest(id));
})
.WithName("DeleteMenuItem")
.WithTags("MenuItems")
.Produces(204)
.Produces(404);

// Get All Menu Items
app.MapGet("/api/menu-items", async (CampusEatsContext db) =>
{
    var items = await db.MenuItem.ToListAsync();
    return Results.Ok(items);
})
.WithName("GetAllMenuItems")
.WithTags("MenuItems")
.Produces(200);

// Get Menu Item by ID
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
// HEALTH CHECK
// ============================================

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
.WithName("HealthCheck")
.WithTags("System")
.Produces(200);

app.Logger.LogInformation("🚀 CampusEats API is starting...");
app.Logger.LogInformation("📍 Swagger UI: http://localhost:5298/swagger");

app.Run();
