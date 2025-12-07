using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CampusEatsFrontend.Models.Payment;

namespace CampusEatsFrontend.Services;

public class PaymentService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions;

    public PaymentService(HttpClient http)
    {
        _http = http;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public async Task<HttpResponseMessage> CreatePaymentAsync(CreatePaymentRequest request)
    {
        return await _http.PostAsJsonAsync("api/payments", request, _jsonOptions);
    }

    public async Task<PaymentDto?> GetPaymentByIdAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<PaymentDto>($"api/payments/{id}", _jsonOptions);
    }

    public async Task<List<PaymentDto>?> GetPaymentHistoryAsync(Guid userId)
    {
        return await _http.GetFromJsonAsync<List<PaymentDto>>($"api/users/{userId}/payments", _jsonOptions);
    }
}
