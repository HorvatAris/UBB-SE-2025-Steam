using System.Reflection;

namespace SteamHub.ApiContract.Models.User
{
    /// <summary>
    /// Data Transfer Object (DTO) representing user details.
    /// </summary>
    public class User : IUserDetails
    {
        public User(
            int userIdentifier,
            string name,
            string password,
            string email,
            float walletBalance,
            float pointsBalance,
            UserRole userRole)
        {
            UserId = userIdentifier;
            UserName = name;
            Password = password;
            Email = email;
            WalletBalance = walletBalance;
            PointsBalance = pointsBalance;
            UserRole = userRole;
        }


        public User(IUserDetails userDetails)
        {
            UserId = userDetails.UserId;
            UserName = userDetails.UserName;
            Email = userDetails.Email;
            WalletBalance = userDetails.WalletBalance;
            PointsBalance = userDetails.PointsBalance;
            UserRole = userDetails.UserRole;
        }

        public byte[] ProfilePicture;
        public string Username2 { get; set; }
        public bool IsDeveloper => UserRole == UserRole.Developer;
        public DateTime CreatedAt { get; set; }

        public ICollection<UserAchievement> UserAchievements { get; set; }
        public ICollection<SoldGame> SoldGames { get; set; } = new List<SoldGame>();
        public DateTime? LastLogin { get; set; }
        public string IpAddress;
        public string ProfilePicturePath;
        public FriendshipStatus FriendshipStatus;
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
            return new User
            {
                UserId = userId,
                Username2 = $"User_{userId}",
                ProfilePicturePath = "ms-appx:///Assets/DefaultUser.png"
            };
        }

        public User(int id, string username, bool isDeveloper)
        {
            LoadProfilePicture();
            this.UserId = id;
            this.Username2 = username;
            //this.IsDeveloper = isDeveloper;
        }

        public User(int id, string userName, string ipAddress)
        {
            this.UserId = id;
            this.Username2 = userName;
            this.IpAddress = ipAddress;
        }

        public User()
        {
            LoadProfilePicture();
            this.UserId = 0;
            this.Username2 = "test";
            //this.IsDeveloper = false;
        }

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the display name chosen by the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user's password (hashed or plain-text depending on context).
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the balance in the user's wallet (real currency).
        /// </summary>
        public float WalletBalance { get; set; }

        /// <summary>
        /// Gets or sets the user's points balance (redeemable rewards).
        /// </summary>
        public float PointsBalance { get; set; }

        /// <summary>
        /// Gets or sets the user's role (e.g., Developer or User).
        /// </summary>
        public UserRole UserRole { get; set; }
    }

    public enum FriendshipStatus
    {
        NotFriends,
        Friends,
        RequestSent,
        RequestReceived
    }
}