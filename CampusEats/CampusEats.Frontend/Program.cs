using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CampusEatsFrontend;
using CampusEatsFrontend.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped<AuthorizationMessageHandler>();


builder.Services.AddScoped(sp =>
{
    var authHandler = sp.GetRequiredService<AuthorizationMessageHandler>();
    authHandler.InnerHandler = new HttpClientHandler();
    
    var httpClient = new HttpClient(authHandler) 
    { 
        BaseAddress = new Uri("http://localhost:5298/") 
    };
    
    return httpClient;
});

builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNameCaseInsensitive = true;
    options.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<MenuItemService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<KitchenService>();
builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();
