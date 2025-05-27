namespace SteamHub.ApiContract.Models.User
{
    public class CreateUserRequest
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public float WalletBalance { get; set; }

        public float PointsBalance { get; set; }

        public RoleEnum Role { get; set; }

        public string ProfilePicture { get; set; }
    }
}