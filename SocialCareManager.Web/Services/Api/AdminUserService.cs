using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SocialCareManager.Web.Configuration;
using SocialCareManager.Web.Dtos;

namespace SocialCareManager.Web.Services.Api;

public class AdminUserService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly ApiSettings _apiSettings;

    public AdminUserService(
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

    public async Task<List<StaffUserDto>> GetAllAsync()
    {
        SetAuthorization();

        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<StaffUserDto>>(
                $"{_apiSettings.BaseUrl}api/admin/users");

            return result ?? new List<StaffUserDto>();
        }
        catch (HttpRequestException)
        {
            return new List<StaffUserDto>();
        }
    }

    public async Task<(bool Success, string? Error)> CreateAsync(CreateStaffUserDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_apiSettings.BaseUrl}api/admin/users",
            dto);

        if (response.IsSuccessStatusCode)
            return (true, null);

        var error = await response.Content.ReadAsStringAsync();
        return (false, string.IsNullOrWhiteSpace(error) ? "Could not create user." : error);
    }
}