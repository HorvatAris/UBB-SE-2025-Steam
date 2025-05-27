namespace SteamHub.ApiContract.Proxies
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using Models.User;
    using Repositories;

    public class UserRepositoryProxy : IUserRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public UserRepositoryProxy(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
        }

        public List<User>? GetAllUsers()
        {
            return null;
        }

        public User? GetUserById(int id)
        {
            return null;
        }

        public User UpdateUser(User user)
        {
            return null;
        }

        public User CreateUser(User user)
        {
            return null;
        }

        public void DeleteUser(int userId)
        {
            // No-op
        }

        public User? VerifyCredentials(string emailOrUsername)
        {
            return null;
        }

        public User? GetUserByEmail(string email)
        {
            return null;
        }

        public User? GetUserByUsername(string username)
        {
            return null;
        }

        public string CheckUserExists(string email, string username)
        {
            return null;
        }

        public void ChangeEmail(int userId, string newEmail)
        {
            // No-op
        }

        public void ChangePassword(int userId, string newPassword)
        {
            // No-op
        }

        public void ChangeUsername(int userId, string newUsername)
        {
            // No-op
        }

        public void UpdateLastLogin(int userId)
        {
            // No-op
        }

        public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Users", request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<CreateUserResponse>(_jsonOptions);
            return result ?? throw new InvalidOperationException("Invalid response from CreateUserAsync");
        }

        public async Task UpdateUserAsync(int id, UpdateUserRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/Users/{id}", request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<GetUsersResponse?> GetUsersAsync()
        {
            var response = await _httpClient.GetAsync("/api/Users");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<GetUsersResponse>(_jsonOptions);
            return result;
        }

        public async Task<UserResponse?> GetUserByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Users/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<UserResponse>(_jsonOptions);
            return result;
        }

        public void UpdateProfileBioAsync(int userId, string bio)
        {
            // No-op
        }

        public Task UpdateProfilePictureAsync(int userId, string localImagePath)
        {
            return null;
        }
    }
}
