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
        public string ProfilePicture;

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
        public string? Bio { get; set; }
        public DateTime LastModified { get; set; }
        public FriendshipStatus FriendshipStatus;
        private async void LoadProfilePicture()
        {
            #if DEBUG
            try
            {
                string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string imagePath = Path.Combine(exePath, "Assets", "default_avatar.png");
                ProfilePicture = imagePath;
            }
            catch
            {
                ProfilePicture = string.Empty;
            }
            #endif
        }
        public static User GetUserById(int userId)
        {
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
