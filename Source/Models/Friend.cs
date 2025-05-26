namespace BusinessLayer.Models
{
    public class Friend
    {
        private string profilePhotoPath = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ProfilePhotoPath
        {
            get => string.IsNullOrEmpty(profilePhotoPath) ? "ms-appx:///Assets/default_avatar.png" : profilePhotoPath;
            set => profilePhotoPath = value;
        }
    }
}