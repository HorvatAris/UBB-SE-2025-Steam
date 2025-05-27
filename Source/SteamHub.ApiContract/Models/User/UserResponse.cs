namespace SteamHub.ApiContract.Models.User;

public class UserResponse
{
    public int UserId { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public float WalletBalance { get; set; }

    public float PointsBalance { get; set; }

    public RoleEnum Role { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public string ProfilePicture { get; set; }

    public bool IsDeveloper => Role == RoleEnum.Developer;
}
