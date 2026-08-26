using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SocialCareManager.Web.Configuration;
using SocialCareManager.Web.Dtos.DailyNotes;

namespace SocialCareManager.Web.Services.Api;

public class DailyNoteService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly ApiSettings _apiSettings;

    public DailyNoteService(
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

    public async Task<List<DailyNoteDto>?> GetAllAsync(Guid serviceUserId)
    {
        SetAuthorization();

        var notes = await _httpClient.GetFromJsonAsync<List<DailyNoteDto>>(
            $"{_apiSettings.BaseUrl}api/serviceusers/{serviceUserId}/dailynotes");

        return notes?
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }

    public async Task<bool> CreateAsync(Guid serviceUserId, CreateDailyNoteDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_apiSettings.BaseUrl}api/serviceusers/{serviceUserId}/dailynotes",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(Guid serviceUserId, Guid noteId, EditDailyNoteDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PutAsJsonAsync(
            $"{_apiSettings.BaseUrl}api/serviceusers/{serviceUserId}/dailynotes/{noteId}",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid serviceUserId, Guid noteId)
    {
        SetAuthorization();

        var response = await _httpClient.DeleteAsync(
            $"{_apiSettings.BaseUrl}api/serviceusers/{serviceUserId}/dailynotes/{noteId}");

        return response.IsSuccessStatusCode;
    }

    public async Task<int> GetCountAsync(Guid serviceUserId)
    {
        Console.WriteLine($"API BASE URL: {_apiSettings.BaseUrl}");
Console.WriteLine($"DAILY NOTES URL: {_apiSettings.BaseUrl}api/serviceusers/{serviceUserId}/dailynotes");
        var notes = await GetAllAsync(serviceUserId);
        return notes?.Count ?? 0;
    }
}