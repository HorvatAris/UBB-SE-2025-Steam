using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessLayer.Models
{
    public class UserProfile
    {
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public DateTime LastModified { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        private string profilePhotoPath = string.Empty;

        public string ProfilePhotoPath
        {
            get => string.IsNullOrEmpty(profilePhotoPath) ? "ms-appx:///Assets/default_avatar.png" : profilePhotoPath;
            set => profilePhotoPath = value;
        }

        [NotMapped]
        public ObservableCollection<Friend> Friends { get; set; } = new ObservableCollection<Friend>();

        [NotMapped]
        public ObservableCollection<FriendRequest> FriendRequests { get; set; } = new ObservableCollection<FriendRequest>();
    }
}