namespace SteamHub.ApiContract.Models.User
{
    public class UserWithSessionDetails
    {
        public Guid SessionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool Developer { get; set; }
        public DateTime UserCreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    } 
}