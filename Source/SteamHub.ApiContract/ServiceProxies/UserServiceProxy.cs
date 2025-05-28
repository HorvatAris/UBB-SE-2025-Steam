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
        private string _authToken;

        public UserServiceProxy(IHttpClientFactory httpClientFactory)
            : base()
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
        }

        private string GetAuthToken()
        {
            return _authToken;
        }

        private void SetAuthToken(string token)
        {
            _authToken = token;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        private void EnsureAuthorized()
        {
            var authToken = GetAuthToken();
            if (string.IsNullOrEmpty(authToken))
            {
                Debug.WriteLine("Authorization required but no auth token found");
                throw new UnauthorizedAccessException("User must be logged in to perform this operation.");
            }
        }

        /// <inheritdoc/>
        public async Task<List<User>> GetAllUsersAsync()
        {
            EnsureAuthorized();
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
                EnsureAuthorized();
                return GetAsync<List<User>>("User").GetAwaiter().GetResult();
            }
            catch
            {
                return new List<User>();
            }
        }

        /// <inheritdoc/>
        public async Task<User> GetUserByIdentifierAsync(int userId)
        {
            try
            {
                // EnsureAuthorized();
                return await GetAsync<User>($"User/{userId}");
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                EnsureAuthorized();
                return await GetAsync<User>($"User/email/{email}");
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            try
            {
                EnsureAuthorized();
                return await GetAsync<User>($"User/username/{username}");
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task ValidateUserAndEmailAsync(string email, string username)
        {
            try
            {
                EnsureAuthorized();
                await PostAsync("User/validate", new { Email = email, Username = username });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <inheritdoc/>
        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                EnsureAuthorized();
                return await PostAsync<User>("User", user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create user: {ex.Message}");
            }
        }

        /// <inheritdoc/>
        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                EnsureAuthorized();
                return await PutAsync<User>($"User/{user.UserId}", user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update user: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task DeleteUserAsync(int userId)
        {
            try
            {
                EnsureAuthorized();
                await DeleteAsync<object>($"User/{userId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete user: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> AcceptChangesAsync(int userId, string givenPassword)
        {
            try
            {
                EnsureAuthorized();
                return await PostAsync<bool>($"User/{userId}/verify", new { Password = givenPassword });
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task UpdateUserEmailAsync(int userId, string newEmail)
        {
            try
            {
                EnsureAuthorized();
                await PutAsync<User>($"User/{userId}/email", new { Email = newEmail });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update email: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task UpdateUserPasswordAsync(int userId, string newPassword)
        {
            try
            {
                EnsureAuthorized();
                await PutAsync<User>($"User/{userId}/password", new { Password = newPassword });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update password: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task UpdateUserUsernameAsync(int userId, string newUsername)
        {
            try
            {
                EnsureAuthorized();
                await PutAsync<User>($"User/{userId}/username", new { Username = newUsername });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update username: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<User?> GetCurrentUserAsync()
        {
            try
            {
                Debug.WriteLine("Attempting to get current user...");
                
                // Check if we have an auth token
                var authToken = GetAuthToken();
                if (string.IsNullOrEmpty(authToken))
                {
                    Debug.WriteLine("No auth token found");
                    return null;
                }

                Debug.WriteLine($"Auth token found: {authToken.Substring(0, Math.Min(20, authToken.Length))}...");

                // Ensure the token is set in the headers
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

                // Make the request to the correct endpoint
                var response = await _httpClient.GetAsync("/api/Session/CurrentUser");
                
                Debug.WriteLine($"Response status code: {response.StatusCode}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Failed to get current user. Status: {response.StatusCode}, Error: {errorContent}");
                    
                    // If unauthorized, clear the token
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        Debug.WriteLine("Unauthorized response - clearing auth token");
                        SetAuthToken(null);
                    }
                    
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Response content: {content}");

                if (string.IsNullOrEmpty(content))
                {
                    Debug.WriteLine("Empty response content");
                    return null;
                }

                var user = JsonSerializer.Deserialize<User>(content, _jsonOptions);
                Debug.WriteLine($"Successfully retrieved user: {user?.Username ?? "null"}");
                return user;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in GetCurrentUser: {ex.Message}");
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> IsUserLoggedInAsync()
        {
            try
            {
                EnsureAuthorized();
                var response = await _httpClient.GetAsync("/api/Session/IsLoggedIn");
                if (!response.IsSuccessStatusCode)
                    return false;

                return await response.Content.ReadFromJsonAsync<bool>(_jsonOptions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to check login status: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateUserUsernameAsync(string username, string currentPassword)
        {
            try
            {
                EnsureAuthorized();
                await PostAsync("User/updateUsername", new { Username = username, CurrentPassword = currentPassword });
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateUserPasswordAsync(string password, string currentPassword)
        {
            try
            {
                EnsureAuthorized();
                await PostAsync("User/updatePassword", new { Password = password, CurrentPassword = currentPassword });
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateUserEmailAsync(string email, string currentPassword)
        {
            try
            {
                EnsureAuthorized();
                await PostAsync("User/updateEmail", new { Email = email, CurrentPassword = currentPassword });
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> VerifyUserPasswordAsync(string password)
        {
            try
            {
                EnsureAuthorized();
                return await PostAsync<bool>("User/verifyPassword", new { Password = password });
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateProfilePictureAsync(string profilePicturePath)
        {
            try
            {
                EnsureAuthorized();
                await PostAsync("User/updatePFP", new { ProfilePicture = profilePicturePath });
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update profile picture: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateProfileBioAsync(string profileBio)
        {
            try
            {
                EnsureAuthorized();
                await PostAsync("User/updateBio", new { Bio = profileBio });
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update profile bio: {ex.Message}", ex);
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
                SetAuthToken(null); // Clear the auth token on logout
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Logout failed: {ex.Message}");
            }
        }
    }
}