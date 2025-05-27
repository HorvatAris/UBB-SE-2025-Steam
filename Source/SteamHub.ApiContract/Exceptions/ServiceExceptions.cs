namespace SteamHub.ApiContract.Exceptions;

/// <summary>
/// Represents a general exception that occurs during a service operation.
/// </summary>
public class ServiceException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceException"/> class with a specified error message.
    /// </summary>
    public ServiceException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance with a specified error message and inner exception.
    /// </summary>
    public ServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}