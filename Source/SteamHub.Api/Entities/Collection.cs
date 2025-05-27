namespace SteamHub.Api.Entities;

public class Collection
{
    public int CollectionId { get; set; }
    public int UserId { get; set; }
    public string CollectionName { get; set; } = string.Empty;
    public string? CoverPicture { get; set; }
    public bool IsPublic { get; set; }
    public DateOnly CreatedAt { get; set; }

    // Navigation properties
    public User User { get; set; }
    public ICollection<CollectionGame> CollectionGames { get; set; }

    // Not mapped properties
    public bool IsAllOwnedGamesCollection { get; }
}