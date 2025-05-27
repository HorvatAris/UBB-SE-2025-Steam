using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Models.Common;

namespace SteamHub.Api.Entities
{
    /// <summary>
    /// Represents an application user with authentication, authorization, and profile details.
    /// </summary>
    public class User
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }
        
        public string Password { get; set; }
        
        public UserRole UserRole { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? LastLogin { get; set; }

        public float WalletBalance { get; set; }

        public float PointsBalance { get; set; }

        public string ProfilePicture { get; set; }
        public string ProfilePicturePath;
        public string? Bio { get; set; }
        public DateTime LastModified { get; set; }

        public IList<UserPointShopItemInventory> UserPointShopItemsInventory { get; set; }

        public IList<StoreTransaction> StoreTransactions { get; set; }

        public ICollection<UserAchievement> UserAchievements { get; set; }

        public ICollection<SoldGame> SoldGames { get; set; }

        public ICollection<Review> Reviews { get; set; }

        // News-related navigation properties
        public ICollection<Post> NewsPosts { get; set; }
        public ICollection<Comment> NewsComments { get; set; }
        public ICollection<PostRatingType> PostRatings { get; set; }

        // Friend request navigation properties
        public ICollection<FriendRequest> SentFriendRequests { get; set; }
        public ICollection<FriendRequest> ReceivedFriendRequests { get; set; }

        // Friendship navigation properties
        public ICollection<Friendship> Friendships { get; set; }
        public ICollection<Friendship> FriendOf { get; set; }

        // Chat-related navigation properties
        public ICollection<ChatConversation> ConversationsAsUser1 { get; set; }
        public ICollection<ChatConversation> ConversationsAsUser2 { get; set; }
        public ICollection<ChatMessage> SentMessages { get; set; }

        // Forum-related navigation properties
        public ICollection<ForumComment> ForumComments { get; set; }
        public ICollection<UserLikedComment> LikedComments { get; set; }
        public ICollection<UserDislikedComment> DislikedComments { get; set; }
    }
}