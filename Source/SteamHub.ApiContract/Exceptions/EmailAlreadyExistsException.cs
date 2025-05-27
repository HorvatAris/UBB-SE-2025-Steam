namespace SteamHub.ApiContract.Exceptions;

/// <summary>
/// Represents an exception thrown when an email is already associated with an existing account.
/// </summary>
public class EmailAlreadyExistsException : Exception
{
    private const string MessageTemplate = "An account with the email '{0}' already exists.";

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailAlreadyExistsException"/> class for a specific email.
    /// </summary>
    /// <param name="email">The email that already exists in the system.</param>
    public EmailAlreadyExistsException(string email)
        : base(string.Format(MessageTemplate, email))
    {
    }
}