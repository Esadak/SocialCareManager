using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SocialCareManager.Web.Configuration;
using SocialCareManager.Web.Dtos;

namespace SocialCareManager.Web.Services.Api;

public class MedicationService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly ApiSettings _apiSettings;

    public MedicationService(
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
            new AuthenticationHeaderValue("Bearer", _authService.AccessToken);
    }

    public async Task<List<MedicationDto>> GetAllAsync(Guid serviceUserId)
    {
        SetAuthorization();

        var result = await _httpClient.GetFromJsonAsync<List<MedicationDto>>(
            $"{_apiSettings.BaseUrl}/api/serviceusers/{serviceUserId}/medications");

        return result ?? new List<MedicationDto>();
    }

    public async Task<MedicationDto?> CreateAsync(
        Guid serviceUserId,
        CreateMedicationDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_apiSettings.BaseUrl}/api/serviceusers/{serviceUserId}/medications",
            dto);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<MedicationDto>();
    }

    public async Task<bool> UpdateAsync(
        Guid serviceUserId,
        Guid medicationId,
        EditMedicationDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PutAsJsonAsync(
            $"{_apiSettings.BaseUrl}/api/serviceusers/{serviceUserId}/medications/{medicationId}",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(
        Guid serviceUserId,
        Guid medicationId)
    {
        SetAuthorization();

        var response = await _httpClient.DeleteAsync(
            $"{_apiSettings.BaseUrl}/api/serviceusers/{serviceUserId}/medications/{medicationId}");

        return response.IsSuccessStatusCode;
    }
}