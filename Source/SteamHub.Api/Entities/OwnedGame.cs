namespace SteamHub.Api.Entities;

public class OwnedGame
{
    public int GameId { get; set; }
    public int UserId { get; set; }
    public string GameTitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CoverPicture { get; set; }

    // Navigation properties
    public User User { get; set; }
}