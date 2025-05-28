using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Models.User;
using System;
using System.Collections.Generic;

namespace SteamHub.ApiContract.Services.Interfaces
{
    public interface IFriendsService
    {
        public Task<List<Friendship>> GetAllFriendshipsAsync(int userId);
        public Task RemoveFriendAsync(int friendshipIdentifier);
        public Task<int> GetFriendshipCountAsync(int userIdentifier);
        public Task<bool> AreUsersFriendsAsync(int userIdentifier1, int userIdentifier2);
        public Task<int?> GetFriendshipIdentifierAsync(int userIdentifier1, int userIdentifier2);
        public Task AddFriendAsync(int userIdentifier, int friendIdentifier);
        public Task<bool> AddFriendByUsernameAsync(string user1Username, string user2Username, string friendEmail, string friendProfilePhotoPath);
        User GetUser();
    }
}
