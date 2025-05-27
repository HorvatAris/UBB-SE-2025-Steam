namespace SteamHub.ApiContract.Models.PasswordReset
{
    public class PasswordResetCode
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string ResetCode { get; set; }

        public DateTime ExpirationTime { get; set; }

        public bool Used { get; set; }

        public string? Email { get; set; }
    }
}