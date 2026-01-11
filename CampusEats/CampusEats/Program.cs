using CampusEats;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurarea serviciilor (Infrastructură + Handlers)
builder.AddInfrastructure();
builder.Services.AddApplicationServices();

// Configurare Stripe
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

var app = builder.Build();

// 2. Configurarea Pipeline-ului HTTP (Middleware)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Migrări automate (Development)
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CampusEats.Persistence.CampusEatsContext>();
    await db.Database.MigrateAsync();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// 3. Maparea Endpoint-urilor
app.MapAllEndpoints();

await app.RunAsync();