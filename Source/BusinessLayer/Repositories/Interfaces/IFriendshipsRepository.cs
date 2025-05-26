using BusinessLayer.Models;

namespace BusinessLayer.Repositories.Interfaces
{
    public interface IFriendshipsRepository
    {
        List<Friendship> GetAllFriendships(int userId);
        void AddFriendship(int userId, int friendId);
        Friendship GetFriendshipById(int friendshipId);
        void RemoveFriendship(int friendshipId);
        int GetFriendshipCount(int userId);
        int? GetFriendshipId(int userId, int friendId);
    }
}