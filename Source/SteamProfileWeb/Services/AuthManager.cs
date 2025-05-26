using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BusinessLayer.Models.Login;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Models;
using BusinessLayer.Services;

namespace SteamProfileWeb.Services;

/// <summary>
/// Manages authentication operations by calling the external Auth API and setting local cookie state.
/// </summary>
public class AuthManager : IAuthManager
{
    private readonly HttpClient httpClient;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ISessionService _sessionService;

    /// <summary>
    /// Constructs the AuthManager with HTTP client factory, HTTP context accessor, and session service.
    /// </summary>
    public AuthManager(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ISessionService sessionService)
    {
        this.httpClient = httpClientFactory.CreateClient("AuthApi");
        this.httpContextAccessor = httpContextAccessor;
        _sessionService = sessionService;
    }

    /// <inheritdoc />
    public async Task<bool> LoginAsync(string emailOrUsername, string password)
    {
        var loginModel = new { EmailOrUsername = emailOrUsername, Password = password };
        var response = await httpClient.PostAsJsonAsync("Auth/Login", loginModel);
        if (!response.IsSuccessStatusCode)
            return false;

        var content = await response.Content.ReadFromJsonAsync<LoginResponse>();
        if (content == null || content.User == null || string.IsNullOrEmpty(content.Token))
            return false;

        var userForSession = new BusinessLayer.Models.User { UserId = content.User.UserId };
        Guid sessionId = _sessionService.CreateNewSession(userForSession);

        var user = content.User;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("AccessToken", content.Token),
            new Claim("SessionId", sessionId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "SteamWebApi");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
            throw new InvalidOperationException("HttpContext is null. Ensure the IHttpContextAccessor is properly configured.");

        await httpContext.SignInAsync("SteamWebApi", principal);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> RegisterAsync(string username, string email, string password, bool isDeveloper)
    {
        var registerModel = new
        {
            Username = username,
            Email = email,
            Password = password,
            IsDeveloper = isDeveloper
        };
        var response = await httpClient.PostAsJsonAsync("Auth/Register", registerModel);
        return response.IsSuccessStatusCode;
    }

    /// <inheritdoc />
    public async Task LogoutAsync()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
            throw new InvalidOperationException("HttpContext is null. Ensure the IHttpContextAccessor is properly configured.");

        await httpContext.SignOutAsync("SteamWebApi");
    }
}

