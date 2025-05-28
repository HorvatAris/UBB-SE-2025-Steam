namespace SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models.Common;

public interface IUserDetails
{
    int UserId { get; }
    int PointsBalance { get; set; }
    UserRole UserRole { get; }
    string Username { get; }
    public string Password { get; }
    string Email { get; }
    decimal WalletBalance { get; set; }
}