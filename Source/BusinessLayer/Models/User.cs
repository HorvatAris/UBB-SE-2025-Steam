using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class User
    {
        public byte[] ProfilePicture;

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        // This property is used for input/output but never stored in the database
        // The actual password is stored as a hash in the database
        public string Password { get; set; }
        public bool IsDeveloper { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<UserAchievement> UserAchievements { get; set; }
        public ICollection<SoldGame> SoldGames { get; set; } = new List<SoldGame>();
        public DateTime? LastLogin { get; set; }
        public string IpAddress;
        public string ProfilePicturePath;
        public FriendshipStatus FriendshipStatus;
        private static Dictionary<int, User> usersConst = new Dictionary<int, User>
        {
            { 1, new User { UserId = 1, Username = "JaneSmith", ProfilePicturePath = "ms-appx:///Assets/friend1_avatar.png" } },
            { 2, new User { UserId = 2, Username = "JohnDoe", ProfilePicturePath = "ms-appx:///Assets/default_avatar.png" } },
            { 3, new User { UserId = 3, Username = "AlexJohnson", ProfilePicturePath = "ms-appx:///Assets/friend2_avatar.png" } }
        };

        public string GetFriendButtonText(FriendshipStatus status)
        {
            switch (status)
            {
                case FriendshipStatus.Friends:
                    return "Friends";
                case FriendshipStatus.RequestSent:
                    return "Cancel Request";
                case FriendshipStatus.RequestReceived:
                    return "Accept Request";
                case FriendshipStatus.NotFriends:
                default:
                    return "Add Friend";
            }
        }
        public void UpdateFrom(User other)
        {
            Email = other.Email;
            Username = other.Username;
            Password = other.Password;
            IpAddress = other.IpAddress;
            FriendshipStatus = other.FriendshipStatus;
            IsDeveloper = other.IsDeveloper;
            CreatedAt = other.CreatedAt;
            LastLogin = other.LastLogin;
        }

        private async void LoadProfilePicture()
        {
#if DEBUG
            try
            {
                string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string imagePath = Path.Combine(exePath, "Assets", "default_avatar.png");
                ProfilePicture = File.ReadAllBytes(imagePath);
            }
            catch
            {
                ProfilePicture = new byte[0];
            }
#endif
        }
        public static User GetUserById(int userId)
        {
            if (usersConst.TryGetValue(userId, out User user))
            {
                return user;
            }

            return new User
            {
                UserId = userId,
                Username = $"User_{userId}",
                ProfilePicturePath = "ms-appx:///Assets/DefaultUser.png"
            };
        }

        public User(int id, string username, bool isDeveloper)
        {
            LoadProfilePicture();
            this.UserId = id;
            this.Username = username;
            this.IsDeveloper = isDeveloper;
        }

        public User(int id, string userName, string ipAddress)
        {
            this.UserId = id;
            this.Username = userName;
            this.IpAddress = ipAddress;
        }

        public User()
        {
            LoadProfilePicture();
            this.UserId = 0;
            this.Username = "test";
            this.IsDeveloper = false;
        }
    }

    public enum FriendshipStatus
    {
        NotFriends,
        Friends,
        RequestSent,
        RequestReceived
    }
}
