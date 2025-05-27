using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.Api.Entities
{
    public class FriendRequest
    {
        public int RequestId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ReceiverUsername { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; } = DateTime.Now;

        // Navigation properties
        public User Sender { get; set; }
        public User Receiver { get; set; }

        // Not mapped property
        public string ProfilePhotoPath { get; set; } = "ms-appx:///Assets/default_avatar.png";
    }
}
