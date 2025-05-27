namespace SteamHub.ApiContract.Exceptions;

/// <summary>
/// Represents an exception thrown when a username is already taken by another user.
/// </summary>
public class UsernameAlreadyTakenException : Exception
{
    private const string MessageTemplate = "The username '{0}' is already taken.";

    /// <summary>
    /// Initializes a new instance of the <see cref="UsernameAlreadyTakenException"/> class for a specific username.
    /// </summary>
    /// <param name="username">The username that is already taken.</param>
    public UsernameAlreadyTakenException(string username)
        : base(string.Format(MessageTemplate, username))
    {
    }
}