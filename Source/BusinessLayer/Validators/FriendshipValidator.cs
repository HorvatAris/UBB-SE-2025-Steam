using BusinessLayer.Models;

namespace BusinessLayer.Validators
{
    public static class FriendshipValidator
    {
        // Validation constants
        private const int MinimumUserId = 1;

        // Validation error message constants
        private const string ErrorInvalidUserId = "User ID must be greater than 0.";
        private const string ErrorInvalidFriendId = "Friend ID must be greater than 0.";
        private const string ErrorSelfFriendship = "User cannot be friends with themselves.";

        public static bool IsUserIdValid(int userId)
        {
            return userId >= MinimumUserId;
        }

        public static bool IsFriendIdValid(int friendUserId)
        {
            return friendUserId >= MinimumUserId;
        }

        public static bool IsNotSelfFriendship(int userId, int friendUserId)
        {
            return userId != friendUserId;
        }

        public static void ValidateFriendship(Friendship friendship)
        {
            if (!IsUserIdValid(friendship.UserId))
            {
                throw new InvalidOperationException(ErrorInvalidUserId);
            }

            if (!IsFriendIdValid(friendship.FriendId))
            {
                throw new InvalidOperationException(ErrorInvalidFriendId);
            }

            if (!IsNotSelfFriendship(friendship.UserId, friendship.FriendId))
            {
                throw new InvalidOperationException(ErrorSelfFriendship);
            }
        }
    }
}