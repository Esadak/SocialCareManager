using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SocialCareManager.Web.Configuration;
using SocialCareManager.Web.Dtos;
using SocialCareManager.Web.Services;

namespace SocialCareManager.Web.Services.Api;

public class MedicationService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly string _baseUrl;

    public MedicationService(
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

    public async Task<List<MedicationDto>> GetAllAsync(Guid serviceUserId)
    {
        SetAuthorization();

        var result = await _httpClient.GetFromJsonAsync<List<MedicationDto>>(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/medications");

        return result ?? new List<MedicationDto>();
    }

    public async Task<MedicationDto?> CreateAsync(Guid serviceUserId, CreateMedicationDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/medications",
            dto);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<MedicationDto>();
    }

    public async Task<bool> UpdateAsync(Guid serviceUserId, Guid medicationId, EditMedicationDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PutAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/medications/{medicationId}",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid serviceUserId, Guid medicationId)
    {
        SetAuthorization();

        var response = await _httpClient.DeleteAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/medications/{medicationId}");

        return response.IsSuccessStatusCode;
    }
}