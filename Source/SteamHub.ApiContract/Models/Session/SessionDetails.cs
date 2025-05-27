namespace SteamHub.ApiContract.Models.Session;

public class SessionDetails
{
    public Guid SessionId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}