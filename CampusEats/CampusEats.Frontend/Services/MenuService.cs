using System.Net.Http.Json;
using CampusEatsFrontend.Models.Menu;

namespace CampusEatsFrontend.Services;

public class MenuService(HttpClient http)
{
    public async Task<List<MenuDto>?> GetAllMenusAsync()
    {
        return await http.GetFromJsonAsync<List<MenuDto>>("api/menu");
    }

    public async Task<MenuDto?> GetMenuByIdAsync(Guid id)
    {
        return await http.GetFromJsonAsync<MenuDto>($"api/menu/{id}");
    }

    public async Task<HttpResponseMessage> CreateMenuAsync(CreateMenuRequest req)
    {
        return await http.PostAsJsonAsync("api/menu", req);
    }

    public async Task<HttpResponseMessage> DeleteMenuAsync(Guid id)
    {
        return await http.DeleteAsync($"api/menu/{id}");
    }
}