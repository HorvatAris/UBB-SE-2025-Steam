using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models.Login;
using SteamHub.ApiContract.Models.Session;
using SteamHub.ApiContract.Services.Interfaces;
using System.Diagnostics;

namespace SteamHub.ApiContract.ServiceProxies
{
    /// <summary>
    /// Proxy implementation of <see cref="IUserService"/>, communicating with the SteamHub API over HTTP.
    /// </summary>
    public class UserServiceProxy : ServiceProxy, IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public UserServiceProxy(IHttpClientFactory httpClientFactory)
            : base()
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
        }

        /// <inheritdoc/>
        public async Task<List<User>> GetAllUsersAsync()
        {
            var response = await _httpClient.GetAsync("/api/User/All");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to fetch users: {error}");
            }
            return await response.Content.ReadFromJsonAsync<List<User>>(_jsonOptions)!;
        }

        /// <inheritdoc/>
        public List<User> GetAllUsers()
        {
            try
            {
                return GetAsync<List<User>>("User").GetAwaiter().GetResult();
            }
            catch
            {
                return new List<User>();
            }
        }

        /// <inheritdoc/>
        public User GetUserByIdentifier(int userId)
        {
            try
            {
                return GetAsync<User>($"User/{userId}").GetAwaiter().GetResult();
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public User GetUserByEmail(string email)
        {
            try
            {
                return GetAsync<User>($"User/email/{email}").GetAwaiter().GetResult();
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public User GetUserByUsername(string username)
        {
            try
            {
                return GetAsync<User>($"User/username/{username}").GetAwaiter().GetResult();
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public void ValidateUserAndEmail(string email, string username)
        {
            try
            {
                PostSync("User/validate", new { Email = email, Username = username });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <inheritdoc/>
        public User CreateUser(User user)
        {
            try
            {
                return PostSync<User>("User", user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create user: {ex.Message}");
            }
        }

        /// <inheritdoc/>
        public User UpdateUser(User user)
        {
            try
            {
                return PutAsync<User>($"User/{user.UserId}", user).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update user: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public void DeleteUser(int userId)
        {
            try
            {
                DeleteAsync<object>($"User/{userId}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete user: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public bool AcceptChanges(int userId, string givenPassword)
        {
            try
            {
                return PostAsync<bool>($"User/{userId}/verify", new { Password = givenPassword })
                    .GetAwaiter().GetResult();
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public void UpdateUserEmail(int userId, string newEmail)
        {
            try
            {
                PutAsync<User>($"User/{userId}/email", new { Email = newEmail }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update email: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public void UpdateUserPassword(int userId, string newPassword)
        {
            try
            {
                PutAsync<User>($"User/{userId}/password", new { Password = newPassword }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update password: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public void UpdateUserUsername(int userId, string newUsername)
        {
            try
            {
                PutAsync<User>($"User/{userId}/username", new { Username = newUsername }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update username: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<User?> LoginAsync(string emailOrUsername, string password)
        {
            try
            {
                Debug.WriteLine($"Attempting login for user: {emailOrUsername}");
                var loginRequest = new { EmailOrUsername = emailOrUsername, Password = password };
                
                Debug.WriteLine("Sending login request...");
                var response = await PostAsync<LoginResponse>("Authentication/Login", loginRequest);
                Debug.WriteLine($"Response received: {response != null}");
                
                if (response == null)
                {
                    Debug.WriteLine("Login response is null");
                    return null;
                }

                Debug.WriteLine($"Response details - Token: {!string.IsNullOrEmpty(response.Token)}, User: {response.User != null}, SessionDetails: {response.UserWithSessionDetails != null}");
                
                if (response.User != null)
                {
                    Debug.WriteLine($"Login successful for user: {response.User.Username}");
                    Debug.WriteLine("Setting auth token...");
                    SetAuthToken(response.Token);
                    Debug.WriteLine("Setting current user...");
                    SetCurrentUser(response.UserWithSessionDetails);
                    Debug.WriteLine("Login process completed successfully");
                    return response.User;
                }

                Debug.WriteLine("Login response User is null");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Login failed with error: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Debug.WriteLine($"Inner exception stack trace: {ex.InnerException.StackTrace}");
                }
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task LogoutAsync()
        {
            try
            {
                await _httpClient.PostAsync("/api/Session/Logout", null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Logout failed: {ex.Message}");
            }
        }

        /// <inheritdoc/>
        public User? GetCurrentUser()
        {
            try
            {
                var response = _httpClient.GetAsync("/api/Session/CurrentUser").GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                    return null;

                return response.Content.ReadFromJsonAsync<User>(_jsonOptions).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to get current user: {ex.Message}");
                return null;
            }
        }

        /// <inheritdoc/>
        public bool IsUserLoggedIn()
        {
            try
            {
                var response = _httpClient.GetAsync("/api/Session/IsLoggedIn").GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                    return false;

                return response.Content.ReadFromJsonAsync<bool>(_jsonOptions).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to check login status: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public bool UpdateUserUsername(string username, string currentPassword)
        {
            try
            {
                PostAsync("User/updateUsername", new { Username = username, CurrentPassword = currentPassword }).GetAwaiter().GetResult();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool UpdateUserPassword(string password, string currentPassword)
        {
            try
            {
                PostAsync("User/updatePassword", new { Password = password, CurrentPassword = currentPassword }).GetAwaiter().GetResult();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool UpdateUserEmail(string email, string currentPassword)
        {
            try
            {
                PostAsync("User/updateEmail", new { Email = email, CurrentPassword = currentPassword }).GetAwaiter().GetResult();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool VerifyUserPassword(string password)
        {
            try
            {
                return PostAsync<bool>("User/verifyPassword", new { Password = password }).GetAwaiter().GetResult();
            }
            catch
            {
                return false;
            }
        }
    }
}