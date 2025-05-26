namespace SteamHub.ApiContract.Models.User;

public enum UserRole
{
    /// <summary>
    /// User is not a developer.
    /// </summary>
    User = 0,
    
    /// <summary>
    /// User is a developer.
    /// </summary>
    Developer = 1,
}