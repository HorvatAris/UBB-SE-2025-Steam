namespace SteamHub.ApiContract.Exceptions;

/// <summary>
/// Represents an exception that occurs within a repository operation.
/// </summary>
public class RepositoryException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryException"/> class with a specified error message.
    /// </summary>
    public RepositoryException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance with a specified error message and inner exception.
    /// </summary>
    public RepositoryException(string message, Exception innerException) : base(message, innerException)
    {
    }
}