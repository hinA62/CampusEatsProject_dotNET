using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CampusEatsFrontend.Models.User;

namespace CampusEatsFrontend.Services;

public class UserService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserService(HttpClient http)
    {
        _http = http;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public async Task<List<ClientProfileDto>?> GetAllUsersAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<ClientProfileDto>>("api/users", _jsonOptions);
        }
        catch
        {
            return new List<ClientProfileDto>();
        }
    }

    public async Task<ClientProfileDto?> GetUserByIdAsync(Guid userId)
    {
        try
        {
            return await _http.GetFromJsonAsync<ClientProfileDto>($"api/users/{userId}", _jsonOptions);
        }
        catch
        {
            return null;
        }
    }
}
