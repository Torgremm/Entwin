using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

public class AuthService
{
    private readonly HttpClient _http;
    private string? _token;

    public string? Token => _token;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<bool> Login(string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/identity/login", new { email, password });

        if (!response.IsSuccessStatusCode)
            return false;

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        _token = result?.token;

        if (!string.IsNullOrEmpty(_token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            return true;
        }

        return false;
    }

    public async Task<bool> Register(string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/identity/register", new { email, password });
        return response.IsSuccessStatusCode;
    }

    public void Logout()
    {
        _token = null;
        _http.DefaultRequestHeaders.Authorization = null;
    }

    private class LoginResponse
    {
        public string token { get; set; } = "";
        public object user { get; set; } = default!;
    }
}
