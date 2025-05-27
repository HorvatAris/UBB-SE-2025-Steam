namespace SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models.Common;

public interface IUserDetails
{
    int UserId { get; }
    float PointsBalance { get; set; }
    UserRole UserRole { get; }
    string Username { get; }
    public string Password { get; }
    string Email { get; }
    float WalletBalance { get; set; }
}