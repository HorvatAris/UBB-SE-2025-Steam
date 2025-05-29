using SteamHub.ApiContract.Models.Common;

namespace SteamHub.ApiContract.Models.User
{
    public class CreateUserRequest
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public decimal WalletBalance { get; set; }

        public int PointsBalance { get; set; }

        public UserRole UserRole { get; set; }

        public string ProfilePicture { get; set; }
    }
}