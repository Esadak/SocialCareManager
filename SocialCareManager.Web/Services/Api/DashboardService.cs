using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SocialCareManager.Web.Configuration;
using SocialCareManager.Web.Dtos.Dashboard;
using SocialCareManager.Web.Services;

namespace SocialCareManager.Web.Services.Api;

public class DashboardService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly string _baseUrl;

    public DashboardService(
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

    public async Task<DashboardInsightsDto?> GetInsightsAsync()
    {
        SetAuthorization();

        try
        {
            return await _httpClient
                .GetFromJsonAsync<DashboardInsightsDto>(
                    $"{_baseUrl}api/dashboard/insights");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }
}