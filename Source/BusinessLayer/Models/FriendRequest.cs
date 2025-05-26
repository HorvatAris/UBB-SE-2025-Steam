using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class FriendRequest
    {
        private string profilePhotoPath = string.Empty;

        public int RequestId { get; set; }

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
