using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SocialCareManager.Web.Configuration;
using SocialCareManager.Web.Dtos;

namespace SocialCareManager.Web.Services.Api;

public class MedicationAdministrationService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly string _baseUrl;

    public MedicationAdministrationService(
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

    public async Task<List<MedicationAdministrationDto>> GetAllAsync(
        Guid serviceUserId)
    {
        SetAuthorization();

        var result = await _httpClient.GetFromJsonAsync<
            List<MedicationAdministrationDto>>(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/medication-administrations");

        return result ?? new List<MedicationAdministrationDto>();
    }

    public async Task<MedicationAdministrationDto?> CreateAsync(
        Guid serviceUserId,
        CreateMedicationAdministrationDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/medication-administrations",
            dto);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content
            .ReadFromJsonAsync<MedicationAdministrationDto>();
    }

    public async Task<bool> UpdateAsync(
        Guid serviceUserId,
        Guid administrationId,
        EditMedicationAdministrationDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PutAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/medication-administrations/{administrationId}",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RecordAsync(
        Guid serviceUserId,
        Guid administrationId,
        RecordMedicationAdministrationDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/medication-administrations/{administrationId}/record",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(
        Guid serviceUserId,
        Guid administrationId)
    {
        SetAuthorization();

        var response = await _httpClient.DeleteAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/medication-administrations/{administrationId}");

        return response.IsSuccessStatusCode;
    }
}