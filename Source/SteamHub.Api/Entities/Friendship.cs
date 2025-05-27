namespace SteamHub.Api.Entities;

public class Friendship
{
    public int FriendshipId { get; set; }
    public int UserId { get; set; }
    public int FriendId { get; set; }

    // Navigation properties
    public User User { get; set; }
    public User Friend { get; set; }

    // Not mapped properties
    public string FriendUsername { get; set; }
    public string FriendProfilePicture { get; set; }
}