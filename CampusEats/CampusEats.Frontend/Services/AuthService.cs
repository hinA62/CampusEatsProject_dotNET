using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CampusEatsFrontend.Models.Auth;
using Microsoft.JSInterop;

namespace CampusEatsFrontend.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;
    private readonly JsonSerializerOptions _jsonOptions;
    private UserDto? _currentUser;

    public AuthService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public UserDto? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser != null;

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", request, _jsonOptions);
            
            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions);
            if (result == null)
                return false;

            _currentUser = new UserDto
            {
                UserId = result.UserId,
                Username = result.Username,
                Role = result.Role,
                Token = result.Token
            };

            // Save token in localStorage
            await _js.InvokeVoidAsync("localStorage.setItem", "authToken", result.Token);
            await _js.InvokeVoidAsync("localStorage.setItem", "userId", result.UserId.ToString());
            await _js.InvokeVoidAsync("localStorage.setItem", "username", result.Username);
            await _js.InvokeVoidAsync("localStorage.setItem", "role", result.Role);

            // Set authorization header
            _http.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", request, _jsonOptions);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            // Call backend logout (optional, since JWT is stateless)
            if (IsAuthenticated)
            {
                await _http.PostAsync("api/auth/logout", null);
            }
        }
        catch { }
        finally
        {
            // Clear local storage
            await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
            await _js.InvokeVoidAsync("localStorage.removeItem", "userId");
            await _js.InvokeVoidAsync("localStorage.removeItem", "username");
            await _js.InvokeVoidAsync("localStorage.removeItem", "role");

            _currentUser = null;
            _http.DefaultRequestHeaders.Authorization = null;
        }
    }

    public async Task<(bool success, string? errorMessage)> ChangePasswordAsync(ChangePasswordRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/change-password", request, _jsonOptions);
            
            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }
            
            // Try to get error message from response
            var errorContent = await response.Content.ReadAsStringAsync();
            return (false, string.IsNullOrEmpty(errorContent) ? "Failed to change password" : errorContent);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task InitializeAsync()
    {
        try
        {
            var token = await _js.InvokeAsync<string?>("localStorage.getItem", "authToken");
            
            if (string.IsNullOrEmpty(token))
                return;

            var userIdStr = await _js.InvokeAsync<string?>("localStorage.getItem", "userId");
            var username = await _js.InvokeAsync<string?>("localStorage.getItem", "username");
            var role = await _js.InvokeAsync<string?>("localStorage.getItem", "role");

            if (!string.IsNullOrEmpty(userIdStr) && Guid.TryParse(userIdStr, out var userId))
            {
                _currentUser = new UserDto
                {
                    UserId = userId,
                    Username = username ?? "",
                    Role = role ?? "Client",
                    Token = token
                };

                _http.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch { }
    }

    private class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
