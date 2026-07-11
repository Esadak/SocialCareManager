using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SocialCareManager.Web.Configuration;
using SocialCareManager.Web.Dtos;
using SocialCareManager.Web.Services;

namespace SocialCareManager.Web.Services.Api;

public class IncidentService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly string _baseUrl;

    public IncidentService(
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

    public async Task<List<IncidentDto>> GetAllAsync(
        Guid serviceUserId)
    {
        SetAuthorization();

        var result = await _httpClient.GetFromJsonAsync<List<IncidentDto>>(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/incidents");

        return result ?? new List<IncidentDto>();
    }

    public async Task<IncidentDto?> CreateAsync(
        Guid serviceUserId,
        CreateIncidentDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/incidents",
            dto);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<IncidentDto>();
    }

    public async Task<bool> UpdateAsync(
        Guid serviceUserId,
        Guid incidentId,
        EditIncidentDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PutAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/incidents/{incidentId}",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ChangeStatusAsync(
        Guid serviceUserId,
        Guid incidentId,
        ChangeIncidentStatusDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/incidents/{incidentId}/status",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<IncidentFollowUpDto?> AddFollowUpAsync(
        Guid serviceUserId,
        Guid incidentId,
        CreateIncidentFollowUpDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/incidents/{incidentId}/follow-ups",
            dto);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content
            .ReadFromJsonAsync<IncidentFollowUpDto>();
    }

    public async Task<bool> CloseAsync(
        Guid serviceUserId,
        Guid incidentId,
        CloseIncidentDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/incidents/{incidentId}/close",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(
        Guid serviceUserId,
        Guid incidentId)
    {
        SetAuthorization();

        var response = await _httpClient.DeleteAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/incidents/{incidentId}");

        return response.IsSuccessStatusCode;
    }
}