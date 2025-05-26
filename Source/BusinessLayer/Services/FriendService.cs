using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services
{
    public class FriendService : IFriendService
    {
        private readonly IFriendRepository friendRepository;

        public FriendService(IFriendRepository friendRepository)
        {
            this.friendRepository = friendRepository ?? throw new ArgumentNullException(nameof(friendRepository));
        }

        public async Task<IEnumerable<Friend>> GetFriendsAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            }

            return await friendRepository.GetFriendsAsync(username);
        }

        public async Task<bool> AddFriendAsync(string user1Username, string user2Username, string friendEmail, string friendProfilePhotoPath)
        {
            if (string.IsNullOrEmpty(user1Username) || string.IsNullOrEmpty(user2Username))
            {
                throw new ArgumentException("Both usernames must be provided");
            }

            return await friendRepository.AddFriendAsync(user1Username, user2Username, friendEmail, friendProfilePhotoPath);
        }

        public async Task<bool> RemoveFriendAsync(string user1Username, string user2Username)
        {
            if (string.IsNullOrEmpty(user1Username) || string.IsNullOrEmpty(user2Username))
            {
                throw new ArgumentException("Both usernames must be provided");
            }

            return await friendRepository.RemoveFriendAsync(user1Username, user2Username);
        }
    }
}