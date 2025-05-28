using SteamHub.ApiContract.Models.Common;

namespace SteamHub.ApiContract.Models.User;

public class UpdateUserRequest
{
    public string UserName { get; set; }

    public string Email { get; set; }

    public decimal WalletBalance { get; set; }

    public int PointsBalance { get; set; }

    public UserRole UserRole { get; set; }
}
