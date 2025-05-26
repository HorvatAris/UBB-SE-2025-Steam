namespace BusinessLayer.Models
{
    public class FriendRequest
    {
        private string profilePhotoPath = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ProfilePhotoPath
        {
            get => string.IsNullOrEmpty(profilePhotoPath) ? "ms-appx:///Assets/default_avatar.png" : profilePhotoPath;
            set => profilePhotoPath = value;
        }
        public string ReceiverUsername { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; } = DateTime.Now;
    }
}