using System.Net.Http.Headers;
using System.Net.Http.Json;
using SocialCareManager.Web.Dtos;

namespace SocialCareManager.Web.Services.Api;

public class ServiceUserService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public ServiceUserService(
        HttpClient httpClient,
        AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
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

        return await _httpClient.GetFromJsonAsync<List<ServiceUserDto>>(
            "http://localhost:5195/api/serviceusers");
    }

    public async Task<ServiceUserDto?> GetAsync(Guid id)
    {
        SetAuthorization();

        return await _httpClient.GetFromJsonAsync<ServiceUserDto>(
            $"http://localhost:5195/api/serviceusers/{id}");
    }

    public async Task<bool> CreateAsync(CreateServiceUserDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PostAsJsonAsync(
            "http://localhost:5195/api/serviceusers",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(Guid id, EditServiceUserDto dto)
    {
        SetAuthorization();

        var response = await _httpClient.PutAsJsonAsync(
            $"http://localhost:5195/api/serviceusers/{id}",
            dto);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        SetAuthorization();

        var response = await _httpClient.DeleteAsync(
            $"http://localhost:5195/api/serviceusers/{id}");

        return response.IsSuccessStatusCode;
    }
}