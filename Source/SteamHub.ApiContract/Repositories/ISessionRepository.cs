using SteamHub.ApiContract.Models.Session;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.ApiContract.Repositories
{
    /// <summary>
    /// Defines data access operations for session management.
    /// </summary>
    public interface ISessionRepository
    {
        /// <summary>
        /// Creates a new session record for a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The newly created session details.</returns>
        SessionDetails CreateSession(int userId);

        /// <summary>
        /// Deletes all sessions associated with a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        void DeleteUserSessions(int userId);

        /// <summary>
        /// Deletes a session by its identifier.
        /// </summary>
        /// <param name="sessionId">The session ID.</param>
        void DeleteSession(Guid sessionId);

        /// <summary>
        /// Retrieves session details by session identifier.
        /// </summary>
        /// <param name="sessionId">The session ID.</param>
        /// <returns>The session details or null.</returns>
        SessionDetails GetSessionById(Guid sessionId);

        /// <summary>
        /// Retrieves user and session details by session identifier.
        /// </summary>
        /// <param name="sessionId">The session ID.</param>
        /// <returns>The user with session details or null.</returns>
        UserWithSessionDetails? GetUserFromSession(Guid sessionId);

        /// <summary>
        /// Retrieves identifiers of all expired sessions.
        /// </summary>
        /// <returns>A list of expired session IDs.</returns>
        List<Guid> GetExpiredSessions();
    }
}