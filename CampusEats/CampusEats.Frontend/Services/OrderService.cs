using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CampusEatsFrontend.Models.Order;

namespace CampusEatsFrontend.Services;

public class OrderService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions;

    public OrderService(HttpClient http)
    {
        _http = http;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public async Task<OrderDto?> GetOrderByIdAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<OrderDto>($"api/orders/{id}", _jsonOptions);
    }

    public async Task<List<OrderDto>?> GetOrderHistoryAsync(Guid clientId)
    {
        return await _http.GetFromJsonAsync<List<OrderDto>>($"api/clients/{clientId}/orders", _jsonOptions);
    }

    public async Task<HttpResponseMessage> PlaceOrderAsync(PlaceOrderRequest request)
    {
        return await _http.PostAsJsonAsync("api/orders", request, _jsonOptions);
    }

    public async Task<HttpResponseMessage> CancelOrderAsync(Guid orderId)
    {
        return await _http.PostAsync($"api/orders/{orderId}/cancel", null);
    }

    // Admin: Get all orders (we'll need to add this endpoint on backend if not exists)
    public async Task<List<OrderDto>?> GetAllOrdersAsync()
    {
        try
        {
            // This might not exist on backend yet, but we'll create the method for future use
            return await _http.GetFromJsonAsync<List<OrderDto>>("api/orders", _jsonOptions);
        }
        catch
        {
            return new List<OrderDto>();
        }
    }

    // Get order details with client info and items
    public async Task<OrderDetailsDto?> GetOrderDetailsAsync(Guid orderId)
    {
        try
        {
            return await _http.GetFromJsonAsync<OrderDetailsDto>($"api/orders/{orderId}/details", _jsonOptions);
        }
        catch
        {
            return null;
        }
    }
}
