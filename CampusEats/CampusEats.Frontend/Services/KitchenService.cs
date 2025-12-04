using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CampusEatsFrontend.Models.Order;

namespace CampusEatsFrontend.Services;

public class KitchenService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions;

    public KitchenService(HttpClient http)
    {
        _http = http;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public async Task<List<OrderDto>?> GetKitchenOrdersAsync(string? status = null)
    {
        var url = string.IsNullOrEmpty(status) 
            ? "api/kitchen/orders" 
            : $"api/kitchen/orders?status={status}";
        
        return await _http.GetFromJsonAsync<List<OrderDto>>(url, _jsonOptions);
    }

    public async Task<HttpResponseMessage> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        return await _http.PatchAsJsonAsync($"api/kitchen/orders/{orderId}/status?newStatus={newStatus}", new { }, _jsonOptions);
    }
}
