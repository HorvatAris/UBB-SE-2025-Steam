namespace SteamHub.ApiContract.Exceptions;

/// <summary>
/// Represents a validation exception that is specific to user-related operations.
/// </summary>
public class UserValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserValidationException"/> class with a specified error message.
    /// </summary>
    public UserValidationException(string message)
        : base(message)
    {
    }
}