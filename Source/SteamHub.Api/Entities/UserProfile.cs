using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SteamHub.Api.Entities
{
    public class UserProfile
    {
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public DateTime LastModified { get; set; }

        // Navigation properties
        public User User { get; set; }
        public ICollection<Friendship> Friendships { get; set; }
        public ICollection<FriendRequest> FriendRequests { get; set; }

        // Display properties (not mapped to database)
        [NotMapped]
        public string Username { get; set; } = string.Empty;
        [NotMapped]
        public string Email { get; set; } = string.Empty;
        [NotMapped]
        public string profilePhotoPath = string.Empty;
    }
}