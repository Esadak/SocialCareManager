using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SocialCareManager.Web.Configuration;
using SocialCareManager.Web.Dtos.Calendar;
using SocialCareManager.Web.Services;

namespace SocialCareManager.Web.Services.Api;

public class CalendarOverviewService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly string _baseUrl;

    public CalendarOverviewService(
        HttpClient httpClient,
        AuthService authService,
        IOptions<ApiSettings> apiSettings)
    {
        _httpClient = httpClient;
        _authService = authService;
        _baseUrl = apiSettings.Value.BaseUrl;
    }

    private void SetAuthorization()
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _authService.AccessToken);
    }

    public async Task<List<CalendarOverviewItemDto>> GetAllAsync(
        Guid serviceUserId,
        DateTime from,
        DateTime to)
    {
        SetAuthorization();

        var fromValue = Uri.EscapeDataString(
            from.ToString("O"));

        var toValue = Uri.EscapeDataString(
            to.ToString("O"));

        var url =
            $"{_baseUrl}api/serviceusers/{serviceUserId}/calendar-overview" +
            $"?from={fromValue}&to={toValue}";

        var result = await _httpClient.GetFromJsonAsync<
            List<CalendarOverviewItemDto>>(url);

        return result ?? new List<CalendarOverviewItemDto>();
    }
}