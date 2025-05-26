using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories.Fakes
{
    public class FakeFriendshipsRepository : IFriendshipsRepository
    {
        private readonly List<Friendship> friendshipsList = new();

        public FakeFriendshipsRepository()
        {
            // Seed some dummy friendships for testing.
            friendshipsList.Add(new Friendship(friendshipId: 1, userId: 1, friendId: 2)
            {
                FriendUsername = "Alice",
                FriendProfilePicture = "alice.jpg"
            });
            friendshipsList.Add(new Friendship(friendshipId: 2, userId: 1, friendId: 3)
            {
                FriendUsername = "Bob",
                FriendProfilePicture = "bob.jpg"
            });
        }

        public List<Friendship> GetAllFriendships(int userId)
        {
            return friendshipsList.Where(friendship => friendship.UserId == userId).OrderBy(friendship => friendship.FriendUsername).ToList();
        }

        public void AddFriendship(int userId, int friendId)
        {
            if (friendshipsList.Any(friendship => friendship.UserId == userId && friendship.FriendId == friendId))
            {
                throw new Exception("Friendship already exists.");
            }

            int newFriendshipId = friendshipsList.Any() ? friendshipsList.Max(friendship => friendship.FriendshipId) + 1 : 1;
            friendshipsList.Add(new Friendship(newFriendshipId, userId, friendId)
            {
                FriendUsername = $"User{friendId}",
                FriendProfilePicture = $"user{friendId}.jpg"
            });
        }

        public Friendship GetFriendshipById(int friendshipId)
        {
            return friendshipsList.FirstOrDefault(friendship => friendship.FriendshipId == friendshipId);
        }

        public void RemoveFriendship(int friendshipId)
        {
            var friendship = friendshipsList.FirstOrDefault(friendship => friendship.FriendshipId == friendshipId);
            if (friendship != null)
            {
                friendshipsList.Remove(friendship);
            }
        }

        public int GetFriendshipCount(int userId)
        {
            return friendshipsList.Count(friendship => friendship.UserId == userId);
        }

        public int? GetFriendshipId(int userId, int friendId)
        {
            return friendshipsList.FirstOrDefault(friendship => friendship.UserId == userId && friendship.FriendId == friendId)?.FriendshipId;
        }
    }
}
