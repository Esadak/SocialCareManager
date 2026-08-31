using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SocialCareManager.Web.Configuration;
using SocialCareManager.Web.Dtos;

namespace SocialCareManager.Web.Services.Api;

public class CarePlanService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly ApiSettings _apiSettings;

    public CarePlanService(
        HttpClient httpClient,
        AuthService authService,
        IOptions<ApiSettings> apiSettings)
    {
        _httpClient = httpClient;
        _authService = authService;
        _apiSettings = apiSettings.Value;
    }

    private void SetAuthorization()
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _authService.AccessToken);
    }

    public async Task<CarePlanDto?> GetActiveAsync(Guid serviceUserId)
    {
        SetAuthorization();

        try
        {
            return await _httpClient.GetFromJsonAsync<CarePlanDto>(
            $"{_apiSettings.BaseUrl}api/serviceusers/{serviceUserId}/careplans/active");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<List<CarePlanDto>> GetHistoryAsync(Guid serviceUserId)
    {
        SetAuthorization();

         try
 {
     var result = await _httpClient.GetFromJsonAsync<List<CarePlanDto>>(
         $"{_apiSettings.BaseUrl}api/serviceusers/{serviceUserId}/careplans/history");

     return result ?? new List<CarePlanDto>();
 }
 catch (HttpRequestException)
 {
     return new List<CarePlanDto>();
 }
    }

    public async Task<CarePlanDto?> CreateAsync(
        Guid serviceUserId,
        CreateCarePlanDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_apiSettings.BaseUrl}api/serviceusers/{serviceUserId}/careplans",
            dto);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<CarePlanDto>();
    }

    public async Task<bool> UpdateCurrentAsync(
        Guid serviceUserId,
        Guid carePlanId,
        UpdateCarePlanDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PutAsJsonAsync(
            $"{_apiSettings.BaseUrl}api/serviceusers/{serviceUserId}/careplans/{carePlanId}",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<CarePlanDto?> CreateNewVersionAsync(
        Guid serviceUserId,
        Guid carePlanId,
        CreateCarePlanDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_apiSettings.BaseUrl}api/serviceusers/{serviceUserId}/careplans/{carePlanId}/new-version",
            dto);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<CarePlanDto>();
    }
}