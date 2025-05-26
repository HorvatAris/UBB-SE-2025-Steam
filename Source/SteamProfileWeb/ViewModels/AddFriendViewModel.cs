using System.Collections.Generic;

namespace SteamProfileWeb.ViewModels
{
    public class AddFriendViewModel
    {
        public List<AddFriendUserViewModel> Users { get; set; } = new();
        public int CurrentUserId { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class AddFriendUserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string ProfilePhotoPath { get; set; }
        public bool IsFriend { get; set; }
    }
} 