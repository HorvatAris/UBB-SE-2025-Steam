using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface IFriendsService
    {
        List<Friendship> GetAllFriendships();
        void RemoveFriend(int friendshipIdentifier);
        int GetFriendshipCount(int userIdentifier);
        bool AreUsersFriends(int userIdentifier1, int userIdentifier2);
        int? GetFriendshipIdentifier(int userIdentifier1, int userIdentifier2);
        void AddFriend(int userIdentifier, int friendIdentifier);
    }
}