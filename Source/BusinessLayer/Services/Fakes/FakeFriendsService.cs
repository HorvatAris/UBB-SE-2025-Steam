using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services.Fakes
{
    public class FakeFriendsService : IFriendsService
    {
        private List<Friendship> friendshipList;

        public FakeFriendsService()
        {
            friendshipList = new List<Friendship>();

            // Seed with some dummy friendships for testing.
            friendshipList.Add(new Friendship(1, 1, 2)
            {
                FriendUsername = "Alice",
                FriendProfilePicture = "alice.jpg"
            });
            friendshipList.Add(new Friendship(2, 1, 3)
            {
                FriendUsername = "Bob",
                FriendProfilePicture = "bob.jpg"
            });
        }

        private int CompareByUsername(Friendship firstFriendship, Friendship secondFriendship)
        {
            return string.Compare(firstFriendship.FriendUsername, secondFriendship.FriendUsername, StringComparison.Ordinal);
        }

        public List<Friendship> GetAllFriendships()
        {
            List<Friendship> filteredFriendships = new List<Friendship>();

            foreach (Friendship friendship in friendshipList)
            {
                // Assume current user is 1
                if (friendship.UserId == 1)
                {
                    filteredFriendships.Add(friendship);
                }
            }

            filteredFriendships.Sort(CompareByUsername);
            return filteredFriendships;
        }

        public void RemoveFriend(int friendshipId)
        {
            Friendship friendshipToRemove = null;

            foreach (Friendship friendship in friendshipList)
            {
                if (friendship.FriendshipId == friendshipId)
                {
                    friendshipToRemove = friendship;
                    break;
                }
            }

            if (friendshipToRemove != null)
            {
                friendshipList.Remove(friendshipToRemove);
            }
        }

        public int GetFriendshipCount(int userId)
        {
            int friendshipCount = 0;

            foreach (Friendship friendship in friendshipList)
            {
                if (friendship.UserId == userId)
                {
                    friendshipCount++;
                }
            }

            return friendshipCount;
        }

        public bool AreUsersFriends(int firstUserId, int secondUserId)
        {
            foreach (Friendship friendship in friendshipList)
            {
                if (friendship.UserId == firstUserId && friendship.FriendId == secondUserId)
                {
                    return true;
                }
            }

            return false;
        }

        public int? GetFriendshipIdentifier(int firstUserId, int secondUserId)
        {
            foreach (Friendship friendship in friendshipList)
            {
                if (friendship.UserId == firstUserId && friendship.FriendId == secondUserId)
                {
                    return friendship.FriendshipId;
                }
            }

            return null;
        }

        public void AddFriend(int userId, int friendId)
        {
            foreach (Friendship friendship in friendshipList)
            {
                if (friendship.UserId == userId && friendship.FriendId == friendId)
                {
                    throw new Exception("Friendship already exists.");
                }
            }

            int newFriendshipId = friendshipList.Count > 0 ? friendshipList[friendshipList.Count - 1].FriendshipId + 1 : 1;

            Friendship newFriendship = new Friendship(newFriendshipId, userId, friendId)
            {
                FriendUsername = "User" + friendId.ToString(),
                FriendProfilePicture = "user" + friendId.ToString() + ".jpg"
            };

            friendshipList.Add(newFriendship);
        }
    }
}
