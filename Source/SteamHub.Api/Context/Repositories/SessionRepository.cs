using SteamHub.ApiContract.Models.Session;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Repositories;

namespace SteamHub.Api.Context.Repositories
{
    /// <summary>
    /// Repository for managing user sessions in the data context.
    /// </summary>
    public class SessionRepository : ISessionRepository
    {
        private readonly DataContext context;

        public SessionRepository(DataContext newContext)
        {
            this.context = newContext ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Creates a new session for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user for whom the session is created.</param>
        /// <returns>The details of the newly created session.</returns>
        public SessionDetails CreateSession(int userId)
        {
            var old = context.UserSessions.Where(s => s.UserId == userId);
            context.UserSessions.RemoveRange(old);

            var session = new SessionDetails
            {
                UserId = userId,
                SessionId = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddHours(2),
            };

            context.UserSessions.Add(session);
            context.SaveChanges();
            return session;
        }

        /// <summary>
        /// Deletes all sessions of a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose sessions are to be deleted.</param>
        public void DeleteUserSessions(int userId)
        {
            var toDelete = context.UserSessions.Where(s => s.UserId == userId).ToList();
            context.UserSessions.RemoveRange(toDelete);
            context.SaveChanges();
        }

        /// <summary>
        /// Deletes a session by its session ID.
        /// </summary>
        /// <param name="sessionId">The ID of the session to delete.</param>
        public void DeleteSession(Guid sessionId)
        {
            var session = context.UserSessions.Find(sessionId);
            if (session != null)
            {
                context.UserSessions.Remove(session);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Retrieves a session by its session ID.
        /// </summary>
        /// <param name="sessionId">The ID of the session to retrieve.</param>
        /// <returns>The session details if found; otherwise, null.</returns>
        public SessionDetails GetSessionById(Guid sessionId)
        {
            return context.UserSessions.Find(sessionId);
        }

        /// <summary>
        /// Retrieves user details along with session information based on the session ID.
        /// </summary>
        /// <param name="sessionId">The ID of the session.</param>
        /// <returns>User details with session information if the session is valid; otherwise, null.</returns>
        public UserWithSessionDetails? GetUserFromSession(Guid sessionId)
        {
            var session = context.UserSessions.Find(sessionId);
            if (session == null || session.ExpiresAt <= DateTime.Now)
            {
                if (session != null)
                {
                    context.UserSessions.Remove(session);
                    context.SaveChanges();
                }
                return null;
            }

            var user = context.Users.Find(session.UserId);
            return user == null
                ? null
                : new UserWithSessionDetails
                {
                    SessionId = sessionId,
                    CreatedAt = session.CreatedAt,
                    ExpiresAt = session.ExpiresAt,
                    UserId = user.UserId,
                    Username = user.UserName,
                    Email = user.Email,
                    Developer = user.IsDeveloper,
                    UserCreatedAt = user.CreatedAt,
                    LastLogin = user.LastLogin
                };
        }

        /// <summary>
        /// Retrieves session IDs of expired sessions.
        /// </summary>
        /// <returns>List of session IDs that have expired.</returns>
        public List<Guid> GetExpiredSessions()
        {
            return context.UserSessions
                .Where(s => s.ExpiresAt < DateTime.Now)
                .Select(s => s.SessionId)
                .ToList();
        }

        /// <summary>
        /// Cleans up (deletes) expired sessions from the data context.
        /// </summary>
        public void CleanupExpiredSessions()
        {
            var expired = context.UserSessions
                    .Where(s => s.ExpiresAt < DateTime.Now)
                    .ToList();
            context.UserSessions.RemoveRange(expired);
            context.SaveChanges();
        }
    }
}
