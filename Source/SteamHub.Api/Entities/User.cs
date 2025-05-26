using SteamHub.ApiContract.Models;

namespace SteamHub.Api.Entities
{
    /// <summary>
    /// Represents an application user with authentication, authorization, and profile details.
    /// </summary>
    public class User
    {
        public int UserId { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        
        public string Password { get; set; }
        
        public bool IsDeveloper { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? LastLogin { get; set; }

        public float WalletBalance { get; set; }

        public float PointsBalance { get; set; }
        public RoleEnum RoleId { get; set; }
        public Role UserRole { get; set; }

        public IList<UserPointShopItemInventory> UserPointShopItemsInventory { get; set; }
        public IList<StoreTransaction> StoreTransactions { get; set; }

        public ICollection<UserAchievement> UserAchievements { get; set; }
        public byte[] ProfilePicture { get; set; }
    }
}