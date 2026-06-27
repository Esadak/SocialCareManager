using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SocialCareManager.Web.Configuration;
using SocialCareManager.Web.Dtos;
using SocialCareManager.Web.Services;

namespace SocialCareManager.Web.Services.Api;

public class CarePlanService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly string _baseUrl;

    public CarePlanService(
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
            new AuthenticationHeaderValue("Bearer", _authService.AccessToken);
    }

    public async Task<CarePlanDto?> GetActiveAsync(Guid serviceUserId)
    {
        SetAuthorization();

        try
        {
            return await _httpClient.GetFromJsonAsync<CarePlanDto>(
                $"{_baseUrl}api/serviceusers/{serviceUserId}/careplans/active");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<bool> CreateAsync(Guid serviceUserId, CreateCarePlanDto dto)
{
    SetAuthorization();

    var response = await _httpClient.PostAsJsonAsync(
        $"{_baseUrl}api/serviceusers/{serviceUserId}/careplans",
        dto);

    return response.IsSuccessStatusCode;
}
}