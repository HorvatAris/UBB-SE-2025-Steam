using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Services
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
        private readonly IUserService userService;

        public FriendsService(IFriendshipsRepository friendshipsRepository, IUserService userService)
        {
            this.friendshipsRepository = friendshipsRepository ?? throw new ArgumentNullException(nameof(friendshipsRepository));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public List<Friendship> GetAllFriendships()
        {
            try
            {
                int currentUserId = userService.GetCurrentUser().UserId;
                var userFriendships = friendshipsRepository.GetAllFriendships(currentUserId);
                return userFriendships;
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Error_RetrieveFriendships, repositoryException);
            }
        }

        public void RemoveFriend(int friendshipId)
        {
            try
            {
                friendshipsRepository.RemoveFriendship(friendshipId);
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Error_RemoveFriend, repositoryException);
            }
        }

        public int GetFriendshipCount(int userId)
        {
            try
            {
                int friendshipCount = friendshipsRepository.GetFriendshipCount(userId);
                return friendshipCount;
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Error_RetrieveFriendshipCount, repositoryException);
            }
        }

        public bool AreUsersFriends(int userId1, int userId2)
        {
            try
            {
                var user1Friendships = friendshipsRepository.GetAllFriendships(userId1);
                foreach (var friendship in user1Friendships)
                {
                    if (friendship.FriendId == userId2)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Error_CheckFriendshipStatus, repositoryException);
            }
        }

        public int? GetFriendshipIdentifier(int userId1, int userId2)
        {
            try
            {
                var user1Friendships = friendshipsRepository.GetAllFriendships(userId1);
                foreach (var friendship in user1Friendships)
                {
                    if (friendship.FriendId == userId2)
                    {
                        return friendship.FriendshipId;
                    }
                }
                return null;
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Error_RetrieveFriendshipId, repositoryException);
            }
        }

        public void AddFriend(int userId, int friendId)
        {
            try
            {
                friendshipsRepository.AddFriendship(userId, friendId);
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Error_AddFriend, repositoryException);
            }
        }
    }
}
