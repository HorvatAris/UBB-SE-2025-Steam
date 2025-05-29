namespace SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models.Common;

public class UserResponse
{
    public int UserId { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public decimal WalletBalance { get; set; }

    public int PointsBalance { get; set; }

    public UserRole UserRole { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public string ProfilePicture { get; set; }
    public string Bio { get; set; }
    public DateTime LastChanged { get; set; }

    public bool IsDeveloper => UserRole == UserRole.Developer;
}
