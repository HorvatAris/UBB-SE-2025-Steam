using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.Models;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Exceptions;
using SteamHub.ApiContract.Models;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class FriendServiceProxy : ServiceProxy, IFriendsService
    {
        public FriendServiceProxy(string baseUrl = "http://172.30.245.56:8000/api/")
            : base(baseUrl)
        {
        }

        public async Task<IEnumerable<Friend>> GetFriendsAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            }

            try
            {
                return await GetAsync<List<Friend>>($"Friend?username={Uri.EscapeDataString(username)}");
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Failed to get friends: {ex.Message}", ex);
            }
        }

        public async Task<bool> AddFriendAsync(string user1Username, string user2Username, string friendEmail, string friendProfilePhotoPath)
        {
            if (string.IsNullOrEmpty(user1Username) || string.IsNullOrEmpty(user2Username))
            {
                throw new ArgumentException("Both usernames must be provided");
            }

            try
            {
                return await PostAsync<bool>("Friend", new
                {
                    User1Username = user1Username,
                    User2Username = user2Username,
                    FriendEmail = friendEmail,
                    FriendProfilePhotoPath = friendProfilePhotoPath
                });
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Failed to add friend: {ex.Message}", ex);
            }
        }

        public async Task<bool> RemoveFriendAsync(string user1Username, string user2Username)
        {
            if (string.IsNullOrEmpty(user1Username) || string.IsNullOrEmpty(user2Username))
            {
                throw new ArgumentException("Both usernames must be provided");
            }

            try
            {
                return await PostAsync<bool>("Friend/remove", new
                {
                    User1Username = user1Username,
                    User2Username = user2Username
                });
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Failed to remove friend: {ex.Message}", ex);
            }
        }

        public List<Friendship> GetAllFriendships()
        {
            throw new NotImplementedException();
        }

        public void RemoveFriend(int friendshipIdentifier)
        {
            throw new NotImplementedException();
        }

        public int GetFriendshipCount(int userIdentifier)
        {
            throw new NotImplementedException();
        }

        public bool AreUsersFriends(int userIdentifier1, int userIdentifier2)
        {
            return false;
        }

        public int? GetFriendshipIdentifier(int userIdentifier1, int userIdentifier2)
        {
            throw new NotImplementedException();
        }

        public void AddFriend(int userIdentifier, int friendIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<List<Friendship>> GetAllFriendshipsAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFriendAsync(int friendshipIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetFriendshipCountAsync(int userIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AreUsersFriendsAsync(int userIdentifier1, int userIdentifier2)
        {
            throw new NotImplementedException();
        }

        public Task<int?> GetFriendshipIdentifierAsync(int userIdentifier1, int userIdentifier2)
        {
            throw new NotImplementedException();
        }

        public Task AddFriendAsync(int userIdentifier, int friendIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddFriendByUsernameAsync(string user1Username, string user2Username, string friendEmail, string friendProfilePhotoPath)
        {
            throw new NotImplementedException();
        }

        public Models.User.User GetUser()
        {
            throw new NotImplementedException();
        }
    }
}