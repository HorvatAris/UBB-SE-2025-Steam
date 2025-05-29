using SteamHub.ApiContract.Exceptions;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class FriendsServiceProxy : ServiceProxy, IFriendsService
    {
        public FriendsServiceProxy(IHttpClientFactory httpClientFactory, string baseUrl = "https://localhost:7241/api/")
            : base(baseUrl)
        {

        }

        public async Task<List<Friendship>> GetAllFriendshipsAsync(int userId)
        {
            try
            {
                return await GetAsync<List<Friendship>>($"Friends/{userId}") ?? new List<Friendship>();
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
                await DeleteAsync<object>($"Friends/{friendshipIdentifier}");
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
                return await GetAsync<int>($"Friends/{userIdentifier}/count");
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
                return await GetAsync<bool>($"Friends/check?user1={userIdentifier1}&user2={userIdentifier2}");
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
                return await GetAsync<int?>($"Friends/id?user1={userIdentifier1}&user2={userIdentifier2}");
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
                await PostAsync("Friends", request);
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

                return await PostAsync<bool>("Friends/add-by-username", request);
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Failed to add friend: {ex.Message}", ex);
            }
        }
    }
}
