using SteamHub.ApiContract.Models;

namespace SteamHub.Api.Entities
{
    /// <summary>
    /// Represents an application user with authentication, authorization, and profile details.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the display name chosen by the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user's email address for login and notifications.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Gets or sets the hashed password used for authentication.
        /// </summary>
        public string Password { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the user has developer privileges.
        /// </summary>
        public bool IsDeveloper { get; set; }
        
        /// <summary>
        /// Gets or sets the UTC timestamp when the user account was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Gets or sets the UTC timestamp of the user's last login, or null if never logged in.
        /// </summary>
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// Gets or sets the current wallet balance in the user's account (real currency).
        /// </summary>
        public float WalletBalance { get; set; }

        /// <summary>
        /// Gets or sets the current points balance for redeemable rewards.
        /// </summary>
        public float PointsBalance { get; set; }

        /// <summary>
        /// Gets or sets the foreign key enum for the user's role (e.g., Developer or User).
        /// </summary>
        public RoleEnum RoleId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the user's role details.
        /// </summary>
        public Role UserRole { get; set; }

        /// <summary>
        /// Gets or sets the collection of point shop items owned by the user.
        /// </summary>
        public IList<UserPointShopItemInventory> UserPointShopItemsInventory { get; set; }

        /// <summary>
        /// Gets or sets the collection of store transactions made by the user.
        /// </summary>
        public IList<StoreTransaction> StoreTransactions { get; set; }

        public ICollection<UserAchievement> UserAchievements { get; set; }


        /// <summary>
        /// Gets or sets the user's profile picture binary data.
        /// </summary>
        public byte[] ProfilePicture { get; set; }
    }
}