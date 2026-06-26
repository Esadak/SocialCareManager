using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SocialCareManager.Web.Configuration;
using SocialCareManager.Web.Dtos;
using SocialCareManager.Web.Services;

namespace SocialCareManager.Web.Services.Api;

public class NextOfKinService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly string _baseUrl;

    public NextOfKinService(
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

    public async Task<List<NextOfKinDto>?> GetAllAsync(Guid serviceUserId)
    {
        SetAuthorization();

        return await _httpClient.GetFromJsonAsync<List<NextOfKinDto>>(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/nextofkin");
    }

    public async Task<int> GetCountAsync(Guid serviceUserId)
    {
        var contacts = await GetAllAsync(serviceUserId);

        return contacts?.Count ?? 0;
    }

    public async Task<bool> CreateAsync(Guid serviceUserId, CreateNextOfKinDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/nextofkin",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(Guid serviceUserId, Guid contactId, EditNextOfKinDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PutAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/nextofkin/{contactId}",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid serviceUserId, Guid contactId)
    {
        SetAuthorization();

        var response = await _httpClient.DeleteAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/nextofkin/{contactId}");

        return response.IsSuccessStatusCode;
    }
}