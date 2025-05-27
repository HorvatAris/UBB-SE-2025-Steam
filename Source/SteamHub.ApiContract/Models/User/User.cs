using System.Reflection;

namespace SteamHub.ApiContract.Models.User
{
    /// <summary>
    /// Data Transfer Object (DTO) representing user details.
    /// </summary>
    public class User : IUserDetails
    {
        public User()
        {

        }

        public User(IUserDetails userDetails)
        {
            UserId = userDetails.UserId;
            Username = userDetails.Username;
            Email = userDetails.Email;
            WalletBalance = userDetails.WalletBalance;
            PointsBalance = userDetails.PointsBalance;
            UserRole = userDetails.UserRole;
        }
        
        public static User GetUserById(int userId)
        {
            return new User
            {
                UserId = userId,
                Username = $"User_{userId}",
                ProfilePicture = "ms-appx:///Assets/DefaultUser.png"
            };
        }

        public DateTime? LastLogin { get; set; }
        public string IpAddress;
        public FriendshipStatus FriendshipStatus;

        public int UserId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public float WalletBalance { get; set; }

        public float PointsBalance { get; set; }

        public UserRole UserRole { get; set; }

        public string ProfilePicture { get; set; }

        public bool IsDeveloper => UserRole == UserRole.Developer;

        public DateTime CreatedAt { get; set; }

        public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();

        public ICollection<SoldGame> SoldGames { get; set; } = new List<SoldGame>();
    }

    public enum FriendshipStatus
    {
        NotFriends,
        Friends,
        RequestSent,
        RequestReceived
    }
}