using System.Globalization;
using System.Security.Claims;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models.Common;

namespace SteamHub.Web.Services;

public class WebUserDetails : IUserDetails
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private decimal _walletBalance;
    private int _pointsBalance;

    public WebUserDetails(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
        InitializeBalances();
    }

    private void InitializeBalances()
    {
        var walletStr = httpContextAccessor.HttpContext?.Session.GetString("WalletBalance");
        var pointsStr = httpContextAccessor.HttpContext?.Session.GetString("PointsBalance");

        if (decimal.TryParse(walletStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var wallet))
        {
            _walletBalance = wallet;
        }

        if (int.TryParse(pointsStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var points))
        {
            _pointsBalance = points;
        }
    }

    public int UserId => int.Parse(GetClaimValue(ClaimTypes.NameIdentifier)!);

    public int PointsBalance
    {
        get => _pointsBalance;
        set
        {
            _pointsBalance = value;
            httpContextAccessor.HttpContext?.Session.SetString("PointsBalance",
                value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public UserRole UserRole => Enum.Parse<UserRole>(GetClaimValue(ClaimTypes.Role)!);
    public string Username => GetClaimValue(ClaimTypes.Name)!;
    public string Password => GetClaimValue(ClaimTypes.Hash)!;
    public string Email => GetClaimValue(ClaimTypes.Email)!;
    public string ProfilePicture
    {
        get => GetClaimValue("ProfilePicture") ?? "ms-appx:///Assets/DefaultUser.png";
        set
        {
            httpContextAccessor.HttpContext?.User.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim("ProfilePicture", value)
            }));
        }
    }

    public decimal WalletBalance
    {
        get => _walletBalance;
        set
        {
            _walletBalance = value;
            httpContextAccessor.HttpContext?.Session.SetString("WalletBalance",
                value.ToString(CultureInfo.InvariantCulture));
        }
    }

    private string? GetClaimValue(string claimType)
    {
        return httpContextAccessor.HttpContext?.User.FindFirst(claimType)?.Value;
    }
}