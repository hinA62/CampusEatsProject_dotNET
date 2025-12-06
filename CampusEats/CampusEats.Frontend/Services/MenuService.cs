using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CampusEatsFrontend.Models.Menu;

namespace CampusEatsFrontend.Services;

public class MenuService(HttpClient http)
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<List<MenuDto>?> GetAllMenusAsync()
    {
        return await http.GetFromJsonAsync<List<MenuDto>>("api/menu", _jsonOptions);
    }

    public async Task<MenuDto?> GetMenuByIdAsync(Guid id)
    {
        return await http.GetFromJsonAsync<MenuDto>($"api/menu/{id}", _jsonOptions);
    }

    public async Task<HttpResponseMessage> CreateMenuAsync(CreateMenuRequest req)
    {
        return await http.PostAsJsonAsync("api/menu", req, _jsonOptions);
    }

    public async Task<HttpResponseMessage> UpdateMenuAsync(UpdateMenuRequest req)
    {
        return await http.PutAsJsonAsync($"api/menu/{req.Id}", req, _jsonOptions);
    }

    public async Task<HttpResponseMessage> DeleteMenuAsync(Guid id)
    {
        return await http.DeleteAsync($"api/menu/{id}");
    }
}