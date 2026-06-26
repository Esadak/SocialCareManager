using System.Net.Http.Headers;
using System.Net.Http.Json;
using SocialCareManager.Web.Dtos.DailyNotes;

namespace SocialCareManager.Web.Services.Api;

public class DailyNoteService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public DailyNoteService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
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
            $"http://localhost:5195/api/serviceusers/{serviceUserId}/dailynotes");

        return notes?
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }

    public async Task<bool> CreateAsync(Guid serviceUserId, CreateDailyNoteDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"http://localhost:5195/api/serviceusers/{serviceUserId}/dailynotes",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(Guid serviceUserId, Guid noteId, EditDailyNoteDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PutAsJsonAsync(
            $"http://localhost:5195/api/serviceusers/{serviceUserId}/dailynotes/{noteId}",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid serviceUserId, Guid noteId)
    {
        SetAuthorization();

        var response = await _httpClient.DeleteAsync(
            $"http://localhost:5195/api/serviceusers/{serviceUserId}/dailynotes/{noteId}");

        return response.IsSuccessStatusCode;
    }
}