namespace SteamHub.ApiContract.Models.Register;

/// <summary>
/// Represents the request payload for a user registration operation.
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Gets or sets the desired username for the new account.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the email address for the new account.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the plain-text password for the new account.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this user is registering as a developer.
    /// </summary>
    public bool IsDeveloper { get; set; }
}
