using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace CampusEatsFrontend.Services;

public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly IJSRuntime _jsRuntime;

    public AuthorizationMessageHandler(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            // Get token from localStorage
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken", cancellationToken);

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch
        {
            // If localStorage access fails, continue without token
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
