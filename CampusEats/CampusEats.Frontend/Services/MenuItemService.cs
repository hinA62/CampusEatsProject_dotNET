using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CampusEatsFrontend.Models.MenuItem;

namespace CampusEatsFrontend.Services;

public class MenuItemService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions;

    public MenuItemService(HttpClient http)
    {
        _http = http;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public async Task<List<MenuItemDto>?> GetAllMenuItemsAsync()
    {
        return await _http.GetFromJsonAsync<List<MenuItemDto>>("api/menu-items", _jsonOptions);
    }

    public async Task<MenuItemDto?> GetMenuItemByIdAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<MenuItemDto>($"api/menu-items/{id}", _jsonOptions);
    }

    public async Task<HttpResponseMessage> CreateMenuItemAsync(CreateMenuItemRequest request)
    {
        return await _http.PostAsJsonAsync("api/menu-items", request, _jsonOptions);
    }

    public async Task<HttpResponseMessage> UpdateMenuItemAsync(UpdateMenuItemRequest request)
    {
        return await _http.PutAsJsonAsync($"api/menu-items/{request.Id}", request, _jsonOptions);
    }

    public async Task<HttpResponseMessage> DeleteMenuItemAsync(Guid id)
    {
        return await _http.DeleteAsync($"api/menu-items/{id}");
    }
}
