namespace SteamHub.Api.Entities;

public class SessionDetails
{
    public Guid SessionId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public User User { get; set; }
}