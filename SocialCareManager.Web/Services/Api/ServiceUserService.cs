using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SocialCareManager.Web.Configuration;
using SocialCareManager.Web.Dtos;
using SocialCareManager.Web.Services;

namespace SocialCareManager.Web.Services.Api;

public class ServiceUserService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly string _baseUrl;

    public ServiceUserService(
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

    public async Task<List<ServiceUserDto>?> GetAllAsync()
    {
        SetAuthorization();

        Console.WriteLine($"SERVICE USER BASE URL: '{_baseUrl}'");
    Console.WriteLine($"SERVICE USER REQUEST URL: '{_baseUrl}api/serviceusers'");

        return await _httpClient.GetFromJsonAsync<List<ServiceUserDto>>(
            $"{_baseUrl}api/serviceusers");
    }

    public async Task<ServiceUserDto?> GetAsync(Guid id)
    {
        SetAuthorization();

        

        return await _httpClient.GetFromJsonAsync<ServiceUserDto>(
            $"{_baseUrl}api/serviceusers/{id}");
    }

    public async Task<bool> CreateAsync(CreateServiceUserDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}api/serviceusers",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(Guid id, EditServiceUserDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PutAsJsonAsync(
            $"{_baseUrl}api/serviceusers/{id}",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        SetAuthorization();

        var response = await _httpClient.DeleteAsync(
            $"{_baseUrl}api/serviceusers/{id}");

        return response.IsSuccessStatusCode;
    }
}