namespace SteamHub.Api.Entities
{
    public class PasswordResetCode
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string ResetCode { get; set; }

        public DateTime ExpirationTime { get; set; }

        public bool Used { get; set; }

        public string? Email { get; set; }

        public User User { get; set; }
    }
}