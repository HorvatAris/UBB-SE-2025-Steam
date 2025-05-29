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
        public UserServiceProxy(string baseUrl = "https://localhost:7241/api/")
            : base(baseUrl)
        {
        }

        private void EnsureAuthorized()
        {
            if (CurrentUser == null)
            {
                Debug.WriteLine("Authorization required but no user session found");
                throw new UnauthorizedAccessException("User must be logged in to perform this operation.");
            }
        }

        /// <inheritdoc/>
        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await GetAsync<List<User>>("User");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to fetch users: {ex.Message}");
                return new List<User>();
            }
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
                EnsureAuthorized();
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
                if (CurrentUser == null)
                {
                    Debug.WriteLine("No user session found");
                    return null;
                }
                return await GetAsync<User>("Session/CurrentUser");
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
                return await GetAsync<bool>("Session/IsLoggedIn");
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

        public async Task UpdateProfilePictureAsync(int userId, string profilePicturePath)
        {
            try
            {
                EnsureAuthorized();
                await PutAsync<object>($"User/{userId}/profilePicture", new { ProfilePicture = profilePicturePath });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update profile picture: {ex.Message}", ex);
            }
        }

        public async Task UpdateProfileBioAsync(int userId, string profileBio)
        {
            try
            {
                EnsureAuthorized();
                await PutAsync<object>($"User/{userId}/bio", new { Bio = profileBio });
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
                Debug.WriteLine($"LoginAsync - Starting login for user: {emailOrUsername}");
                var loginRequest = new { EmailOrUsername = emailOrUsername, Password = password };
                
                Debug.WriteLine("LoginAsync - Sending login request...");
                var response = await PostAsync<LoginResponse>("Authentication/Login", loginRequest);
                
                if (response == null)
                {
                    Debug.WriteLine("LoginAsync - Login response is null");
                    return null;
                }

                Debug.WriteLine($"LoginAsync - Response received:");
                Debug.WriteLine($"  Token exists: {!string.IsNullOrEmpty(response.Token)}");
                Debug.WriteLine($"  User exists: {response.User != null}");
                Debug.WriteLine($"  SessionDetails exists: {response.UserWithSessionDetails != null}");
                
                if (response.User != null && !string.IsNullOrEmpty(response.Token))
                {
                    Debug.WriteLine($"LoginAsync - Login successful for user: {response.User.Username}");
                    Debug.WriteLine($"LoginAsync - Setting auth token: {response.Token.Substring(0, Math.Min(20, response.Token.Length))}...");
                    base.SetAuthToken(response.Token);
                    
                    Debug.WriteLine("LoginAsync - Setting current user");
                    base.SetCurrentUser(response.UserWithSessionDetails);
                    
                    Debug.WriteLine("LoginAsync - Login process completed successfully");
                    return response.User;
                }

                Debug.WriteLine("LoginAsync - Login failed - response.User or response.Token is null");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoginAsync - Login failed with error: {ex.Message}");
                Debug.WriteLine($"LoginAsync - Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"LoginAsync - Inner exception: {ex.InnerException.Message}");
                    Debug.WriteLine($"LoginAsync - Inner exception stack trace: {ex.InnerException.StackTrace}");
                }
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task LogoutAsync()
        {
            try
            {
                Debug.WriteLine("LogoutAsync - Starting logout process");
                await PostAsync("Session/Logout", null);
                Debug.WriteLine("LogoutAsync - Clearing auth token");
                base.SetAuthToken(null);
                Debug.WriteLine("LogoutAsync - Clearing current user");
                base.ClearCurrentUser();
                Debug.WriteLine("LogoutAsync - Logout completed successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LogoutAsync - Logout failed: {ex.Message}");
            }
        }
    }
}