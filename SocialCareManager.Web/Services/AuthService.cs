using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SocialCareManager.Web.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public string? AccessToken { get; private set; }
    public string? Email { get; private set; }
    public string? Role { get; private set; }

    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(AccessToken);
    public bool IsAdmin => string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase);

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "http://localhost:5195/login",
            new LoginRequest(email, password));

        if (!response.IsSuccessStatusCode)
            return false;

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

        AccessToken = loginResponse?.AccessToken;

        if (string.IsNullOrWhiteSpace(AccessToken))
            return false;

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", AccessToken);

        var me = await _httpClient.GetFromJsonAsync<MeResponse>(
            "http://localhost:5195/api/account/me");

        Email = me?.Email ?? email;
        Role = me?.Roles?.FirstOrDefault();

        return true;
    }

    public void Logout()
    {
        AccessToken = null;
        Email = null;
        Role = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    private record LoginRequest(string Email, string Password);

    private class LoginResponse
    {
        public string TokenType { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }

    private class MeResponse
    {
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}