using SteamHub.ApiContract.Exceptions;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class FriendsServiceProxy : IFriendsService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        private IUserDetails User;

        public User GetUser()
        {
            return new User(this.User);
        }

        public FriendsServiceProxy(IHttpClientFactory httpClientFactory, IUserDetails user)
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
            this.User = user;
        }

        public async Task<List<Friendship>> GetAllFriendshipsAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Friends/{userId}");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<List<Friendship>>(_options) ?? new List<Friendship>();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error retrieving friendships", ex);
            }
        }

        public async Task RemoveFriendAsync(int friendshipIdentifier)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/Friends/{friendshipIdentifier}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error removing friend", ex);
            }
        }

        public async Task<int> GetFriendshipCountAsync(int userIdentifier)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Friends/{userIdentifier}/count");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<int>(_options);
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error retrieving friendship count", ex);
            }
        }

        public async Task<bool> AreUsersFriendsAsync(int userIdentifier1, int userIdentifier2)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Friends/check?user1={userIdentifier1}&user2={userIdentifier2}");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<bool>(_options);
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error checking friendship status", ex);
            }
        }

        public async Task<int?> GetFriendshipIdentifierAsync(int userIdentifier1, int userIdentifier2)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Friends/id?user1={userIdentifier1}&user2={userIdentifier2}");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<int?>(_options);
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error retrieving friendship ID", ex);
            }
        }

        public async Task AddFriendAsync(int userIdentifier, int friendIdentifier)
        {
            try
            {
                var request = new { UserId = userIdentifier, FriendId = friendIdentifier };
                var response = await _httpClient.PostAsJsonAsync("/api/Friends", request, _options);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error adding friend", ex);
            }
        }

        public async Task<bool> AddFriendByUsernameAsync(string user1Username, string user2Username, string friendEmail, string friendProfilePhotoPath)
        {
            if (string.IsNullOrEmpty(user1Username) || string.IsNullOrEmpty(user2Username))
            {
                throw new ArgumentException("Both usernames must be provided");
            }

            try
            {
                var request = new
                {
                    User1Username = user1Username,
                    User2Username = user2Username,
                    FriendEmail = friendEmail,
                    FriendProfilePhotoPath = friendProfilePhotoPath
                };

                var response = await _httpClient.PostAsJsonAsync("/api/Friends/add-by-username", request, _options);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<bool>(_options);
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Failed to add friend: {ex.Message}", ex);
            }
        }
    }
}
