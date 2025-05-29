using SteamHub.ApiContract.Exceptions;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ApiContract.Services
{
    public class FriendsService : IFriendsService
    {
        // Error message constants
        private const string Error_RetrieveFriendships = "Error retrieving friendships for user.";
        private const string Error_RemoveFriend = "Error removing friend.";
        private const string Error_RetrieveFriendshipCount = "Error retrieving friendship count.";
        private const string Error_CheckFriendshipStatus = "Error checking friendship status.";
        private const string Error_RetrieveFriendshipId = "Error retrieving friendship ID.";
        private const string Error_AddFriend = "Error adding friend.";

        private readonly IFriendshipsRepository friendshipsRepository;

        public FriendsService(IFriendshipsRepository friendshipsRepository)
        {
            this.friendshipsRepository = friendshipsRepository ??
                throw new ArgumentNullException(nameof(friendshipsRepository));
        }

        public async Task<List<Friendship>> GetAllFriendshipsAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("User ID must be positive", nameof(userId));
            }

            try
            {
                return await friendshipsRepository.GetAllFriendshipsAsync(userId);
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Error_RetrieveFriendships, repositoryException);
            }
        }

        public async Task RemoveFriendAsync(int friendshipId)
        {
            try
            {
                await friendshipsRepository.RemoveFriendshipAsync(friendshipId);
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Error_RemoveFriend, repositoryException);
            }
        }

        public async Task<int> GetFriendshipCountAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("User ID must be positive", nameof(userId));
            }

            try
            {
                return await friendshipsRepository.GetFriendshipCountAsync(userId);
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Error_RetrieveFriendshipCount, repositoryException);
            }
        }

        public async Task<bool> AreUsersFriendsAsync(int userId1, int userId2)
        {
            if (userId1 <= 0 || userId2 <= 0)
            {
                throw new ArgumentException("User IDs must be positive");
            }

            try
            {
                var user1Friendships = await friendshipsRepository.GetAllFriendshipsAsync(userId1);
                return user1Friendships.Any(friendship => friendship.FriendId == userId2);
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Error_CheckFriendshipStatus, repositoryException);
            }
        }

        public async Task<int?> GetFriendshipIdentifierAsync(int userId1, int userId2)
        {
            if (userId1 <= 0 || userId2 <= 0)
            {
                throw new ArgumentException("User IDs must be positive");
            }

            try
            {
                var user1Friendships = await friendshipsRepository.GetAllFriendshipsAsync(userId1);
                return user1Friendships.FirstOrDefault(friendship => friendship.FriendId == userId2)?.FriendshipId;
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Error_RetrieveFriendshipId, repositoryException);
            }
        }

        public async Task AddFriendAsync(int userId, int friendId)
        {
            if (userId <= 0 || friendId <= 0)
            {
                throw new ArgumentException("User IDs must be positive");
            }

            try
            {
                await friendshipsRepository.AddFriendshipAsync(userId, friendId);
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Error_AddFriend, repositoryException);
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
                return await friendshipsRepository.AddFriendAsync(
                    user1Username,
                    user2Username,
                    friendEmail,
                    friendProfilePhotoPath);
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException("Failed to add friend by username", repositoryException);
            }
        }
        public User GetUser()
        {
            throw new NotImplementedException();
        }
    }
}
