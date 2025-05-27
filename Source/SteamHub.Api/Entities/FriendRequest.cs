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
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }
        public DateTime RequestDate { get; set; }
        public FriendRequestStatus Status { get; set; }

        // Navigation properties
        public User Sender { get; set; }
        public User Receiver { get; set; }

        // Not mapped property
        public string ProfilePhotoPath { get; set; } = "ms-appx:///Assets/default_avatar.png";
    }

    public enum FriendRequestStatus
    {
        Pending,
        Accepted,
        Rejected
    }
}
