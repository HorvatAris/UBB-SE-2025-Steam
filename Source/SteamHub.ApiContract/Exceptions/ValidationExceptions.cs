namespace SteamHub.ApiContract.Exceptions;

/// <summary>
/// Represents a general validation error that occurs when user input or state is invalid.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    public ValidationException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance with a specified error message.
    /// </summary>
    public ValidationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance with a specified error message and inner exception.
    /// </summary>
    public ValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}