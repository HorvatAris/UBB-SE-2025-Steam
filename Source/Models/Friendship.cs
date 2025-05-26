using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models
{
    public class Friendship
    {
        [Required]
        public int FriendshipId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int FriendId { get; set; }

        public string FriendUsername { get; set; }
        public string FriendProfilePicture { get; set; }

        public Friendship(int friendshipId, int userId, int friendId, string friendUsername = "", string friendProfilePicture = "")
        {
            FriendshipId = friendshipId;
            UserId = userId;
            FriendId = friendId;
            FriendUsername = friendUsername;
            FriendProfilePicture = friendProfilePicture;
        }
    }
}