using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models.Common;
using SteamHub.ApiContract.Models.Session;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class SessionServiceProxy : ISessionService
    {
        private readonly HttpClient http_client;
        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public SessionServiceProxy(IHttpClientFactory httpClientFactory)
        {
            http_client = httpClientFactory.CreateClient("SteamHubApi");
        }

        /// <inheritdoc />
        public async Task<Guid> CreateNewSessionAsync(User user)
        {
            try
            {
                var response = await http_client.PostAsJsonAsync("/api/Session/Create", user, options);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Guid>(options);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating session: {exception.Message}");
                return Guid.Empty;
            }
        }

        /// <inheritdoc />
        public async Task EndSessionAsync()
        {
            try
            {
                var response = await http_client.PostAsync("/api/Session/Logout", null);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error ending session: {exception.Message}");
            }
        }

        /// <inheritdoc />
        public async Task<User?> GetCurrentUserAsync()
        {
            try
            {
                var response = await http_client.GetAsync("/api/Session/CurrentUser");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<User>(options);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting current user: {exception.Message}");
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<bool> IsUserLoggedInAsync()
        {
            try
            {
                var response = await http_client.GetAsync("/api/Session/IsLoggedIn");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<bool>(options);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking login status: {exception.Message}");
                return false;
            }
        }

        /// <inheritdoc />
        public async Task RestoreSessionFromDatabaseAsync(Guid sessionId)
        {
            try
            {
                var response = await http_client.GetAsync($"/api/Session/{sessionId}");
                response.EnsureSuccessStatusCode();
                var sessionDetails = await response.Content.ReadFromJsonAsync<UserWithSessionDetails>(options);
                if (sessionDetails != null)
                {
                    // Handle session restoration logic here
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error restoring session: {exception.Message}");
            }
        }

        /// <inheritdoc />
        public async Task CleanupExpiredSessionsAsync()
        {
            try
            {
                var response = await http_client.PostAsync("/api/Session/Cleanup", null);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error cleaning up sessions: {exception.Message}");
            }
        }

        /// <inheritdoc />
        public async Task<bool> ValidateSessionAsync(Guid sessionId)
        {
            try
            {
                var response = await http_client.GetAsync($"/api/Session/Validate/{sessionId}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<bool>(options);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating session: {exception.Message}");
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<SessionDetails?> GetCurrentSessionDetailsAsync()
        {
            try
            {
                var response = await http_client.GetAsync("/api/Session/Current");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<SessionDetails>(options);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting session details: {exception.Message}");
                return null;
            }
        }
    }
}
