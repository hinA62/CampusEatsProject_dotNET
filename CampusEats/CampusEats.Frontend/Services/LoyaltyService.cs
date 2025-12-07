using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CampusEatsFrontend.Models.Loyalty;

namespace CampusEatsFrontend.Services;

public class LoyaltyService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions;

    public LoyaltyService(HttpClient http)
    {
        _http = http;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public async Task<LoyaltyBalanceDto?> GetLoyaltyBalanceAsync(Guid userId)
    {
        return await _http.GetFromJsonAsync<LoyaltyBalanceDto>($"api/loyalty/{userId}/balance", _jsonOptions);
    }

    public async Task<List<LoyaltyTransactionDto>?> GetLoyaltyTransactionsAsync(Guid userId)
    {
        return await _http.GetFromJsonAsync<List<LoyaltyTransactionDto>>($"api/loyalty/{userId}/transactions", _jsonOptions);
    }

    public async Task<HttpResponseMessage> RedeemPointsAsync(RedeemPointsRequest request)
    {
        return await _http.PostAsJsonAsync("api/loyalty/redeem", request, _jsonOptions);
    }
}
