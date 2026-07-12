using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SocialCareManager.Web.Configuration;
using SocialCareManager.Web.Dtos.Calendar;
using SocialCareManager.Web.Services;

namespace SocialCareManager.Web.Services.Api;

public class CalendarEventService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly string _baseUrl;

    public CalendarEventService(
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

    public async Task<List<CalendarEventDto>> GetAllAsync(
        Guid serviceUserId,
        DateTime? from = null,
        DateTime? to = null)
    {
        SetAuthorization();

        var url =
            $"{_baseUrl}api/serviceusers/{serviceUserId}/calendar-events";

        var queryParts = new List<string>();

        if (from.HasValue)
        {
            queryParts.Add(
                $"from={Uri.EscapeDataString(from.Value.ToString("O"))}");
        }

        if (to.HasValue)
        {
            queryParts.Add(
                $"to={Uri.EscapeDataString(to.Value.ToString("O"))}");
        }

        if (queryParts.Count > 0)
        {
            url += "?" + string.Join("&", queryParts);
        }

        var result =
            await _httpClient.GetFromJsonAsync<List<CalendarEventDto>>(url);

        return result ?? new List<CalendarEventDto>();
    }

    public async Task<CalendarEventDto?> CreateAsync(
        Guid serviceUserId,
        CreateCalendarEventDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/calendar-events",
            dto);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content
            .ReadFromJsonAsync<CalendarEventDto>();
    }

    public async Task<bool> UpdateAsync(
        Guid serviceUserId,
        Guid eventId,
        EditCalendarEventDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PutAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/calendar-events/{eventId}",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CompleteAsync(
        Guid serviceUserId,
        Guid eventId)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/calendar-events/{eventId}/complete",
            new CompleteCalendarEventDto());

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CancelAsync(
        Guid serviceUserId,
        Guid eventId,
        CancelCalendarEventDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/calendar-events/{eventId}/cancel",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ReopenAsync(
        Guid serviceUserId,
        Guid eventId,
        ReopenCalendarEventDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/calendar-events/{eventId}/reopen",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(
        Guid serviceUserId,
        Guid eventId)
    {
        SetAuthorization();

        var response = await _httpClient.DeleteAsync(
            $"{_baseUrl}api/serviceusers/{serviceUserId}/calendar-events/{eventId}");

        return response.IsSuccessStatusCode;
    }
}