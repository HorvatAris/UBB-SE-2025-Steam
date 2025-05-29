using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using SteamHub.ApiContract.Models.Common;
using SteamHub.ApiContract.Models.Login;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Validators;

namespace SteamHub.Web.Services
{

    /// <summary>
    /// Manages authentication operations by calling the external Auth API and setting local cookie state.
    /// </summary>
    public class AuthManager : IAuthManager
    {
        private readonly HttpClient httpClient;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISessionService _sessionService;
        private readonly IUserService _userService;

        /// <summary>
        /// Constructs the AuthManager with HTTP client factory, HTTP context accessor, and session service.
        /// </summary>
        public AuthManager(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ISessionService sessionService, IUserService userService)
        {
            this.httpClient = httpClientFactory.CreateClient("SteamHubApi");
            this.httpContextAccessor = httpContextAccessor;
            _sessionService = sessionService;
            _userService = userService;
        }

        /// <inheritdoc />
        public async Task<bool> LoginAsync(string emailOrUsername, string password)
        {
            var loginModel = new { EmailOrUsername = emailOrUsername, Password = password };
            var response = await httpClient.PostAsJsonAsync("api/Authentication/Login", loginModel);
            if (!response.IsSuccessStatusCode)
                return false;

            var content = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (content == null || content.User == null || string.IsNullOrEmpty(content.Token))
                return false;

            var userForSession = new User { UserId = content.User.UserId };
            Guid sessionId = await _sessionService.CreateNewSessionAsync(userForSession);
            // TEMPORARY: have to look if the other things are even needed
            await this._userService.LoginAsync(emailOrUsername, password);

            var user = content.User;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Hash, user.Password),
                new Claim(ClaimTypes.Role, user.UserRole.ToString()),
                new Claim("AccessToken", content.Token),
                new Claim("SessionId", sessionId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "SteamHubAuth");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("HttpContext is null. Ensure the IHttpContextAccessor is properly configured.");

            // Store wallet and points balance in session
            httpContext.Session.SetString("WalletBalance", user.WalletBalance.ToString(CultureInfo.InvariantCulture));
            httpContext.Session.SetString("PointsBalance", user.PointsBalance.ToString(CultureInfo.InvariantCulture));

            await httpContext.SignInAsync("SteamHubAuth", principal);
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> RegisterAsync(string username, string email, string password, string confirmPassword, bool isDeveloper)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                throw new ArgumentException("All fields are required.");
            }

            // Validate email format
            if (!UserValidator.IsEmailValid(email))
            {
                throw new ArgumentException("Invalid email format.");
            }

            await _userService.ValidateUserAndEmailAsync(email, username);

            // Validate the password
            if (password != confirmPassword)
            {
                throw new ArgumentException("Password must match!");
            }

            // Validate password
            if (!UserValidator.IsPasswordValid(password))
            {
                throw new ArgumentException("Password must be at least 8 characters long and contain at least one lowercase letter, one uppercase letter, one number, and one special character (@_.,/%^#$!%*?&).");
            }

            var registerModel = new User
            {
                Username = username,
                Email = email,
                Password = password,
                UserRole = isDeveloper ? UserRole.Developer : UserRole.User,
                ProfilePicture = string.Empty
            };

            var createdUser = await _userService.CreateUserAsync(registerModel);
            
            if (createdUser != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc />
        public async Task LogoutAsync()
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null)
                throw new InvalidOperationException("HttpContext is null. Ensure the IHttpContextAccessor is properly configured.");

            // Clear session data
            httpContext.Session.Remove("WalletBalance");
            httpContext.Session.Remove("PointsBalance");

            await httpContext.SignOutAsync("SteamHubAuth");
        }
    }
}