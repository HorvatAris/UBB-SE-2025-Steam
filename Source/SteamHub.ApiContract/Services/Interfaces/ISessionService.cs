using SteamHub.ApiContract.Models.User;

namespace SteamHub.ApiContract.Services.Interfaces
{
    /// <summary>
    /// Defines operations for managing user sessions, including creation, termination, and validation.
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// Creates a new session for the specified user, invalidating any existing sessions.
        /// </summary>
        /// <param name="user">The user for whom to create the session.</param>
        /// <returns>The new session identifier.</returns>
        Guid CreateNewSession(User user);

        /// <summary>
        /// Ends the current session, if one exists, and cleans up associated data.
        /// </summary>
        void EndSession();

        /// <summary>
        /// Retrieves the user associated with the current session.
        /// </summary>
        /// <returns>The current <see cref="User"/>, or <c>null</c> if no valid session exists.</returns>
        User GetCurrentUser();

        /// <summary>
        /// Checks whether a user session is currently active and valid.
        /// </summary>
        /// <returns><c>true</c> if a valid session exists; otherwise <c>false</c>.</returns>
        bool IsUserLoggedIn();

        /// <summary>
        /// Restores a session from persistent storage by session identifier.
        /// </summary>
        /// <param name="sessionIdentifier">The identifier of the session to restore.</param>
        void RestoreSessionFromDatabase(Guid sessionIdentifier);

        /// <summary>
        /// Deletes all expired sessions from persistent storage and clears the current session if expired.
        /// </summary>
        void CleanupExpiredSessions();
    }
}