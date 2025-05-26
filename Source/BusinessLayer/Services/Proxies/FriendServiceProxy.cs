using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Services.Proxies
{
    public class FriendServiceProxy : ServiceProxy, IFriendService
    {
        public FriendServiceProxy(string baseUrl = "https://localhost:7262/api/")
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
    }
}