using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository sessionRepository;
        private readonly IUsersRepository usersRepository;

        public SessionService(ISessionRepository sessionRepository, IUsersRepository usersRepository)
        {
            this.sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
            this.usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        public Guid CreateNewSession(User user)
        {
            // Business logic: Delete any existing sessions for this user
            sessionRepository.DeleteUserSessions(user.UserId);

            // Create a new session
            var sessionDetails = sessionRepository.CreateSession(user.UserId);

            // Update the singleton session instance
            UserSession.Instance.UpdateSession(
                sessionDetails.SessionId,
                user.UserId,
                sessionDetails.CreatedAt,
                sessionDetails.ExpiresAt);

            return sessionDetails.SessionId;
        }

        public void EndSession()
        {
            var sessionId = UserSession.Instance.CurrentSessionId;
            if (sessionId.HasValue)
            {
                sessionRepository.DeleteSession(sessionId.Value);
                UserSession.Instance.ClearSession();
            }
        }

        public User GetCurrentUser()
        {
            // Check if we have a valid session
            if (!UserSession.Instance.IsSessionValid())
            {
                if (UserSession.Instance.CurrentSessionId.HasValue)
                {
                    // If session exists but is invalid (likely expired), delete it
                    sessionRepository.DeleteSession(UserSession.Instance.CurrentSessionId.Value);
                    UserSession.Instance.ClearSession();
                }
                return null;
            }

            // Get user details from the database
            return usersRepository.GetUserById(UserSession.Instance.UserId);
        }

        public bool IsUserLoggedIn()
        {
            return UserSession.Instance.IsSessionValid();
        }

        public void RestoreSessionFromDatabase(Guid sessionId)
        {
            var sessionDetails = sessionRepository.GetSessionById(sessionId);

            if (sessionDetails != null)
            {
                // Check if session is expired
                if (DateTime.Now > sessionDetails.ExpiresAt)
                {
                    // Business logic: Delete expired session
                    sessionRepository.DeleteSession(sessionId);
                    return;
                }

                // Session is valid, update the singleton
                UserSession.Instance.UpdateSession(
                    sessionDetails.SessionId,
                    sessionDetails.UserId,
                    sessionDetails.CreatedAt,
                    sessionDetails.ExpiresAt);
            }
        }

        // Method to cleanup expired sessions (could be called by a background job)
        public void CleanupExpiredSessions()
        {
            var expiredSessions = sessionRepository.GetExpiredSessions();
            foreach (var expiredSessionId in expiredSessions)
            {
                sessionRepository.DeleteSession(expiredSessionId);
            }

            // Also check the current session
            if (UserSession.Instance.CurrentSessionId.HasValue && !UserSession.Instance.IsSessionValid())
            {
                UserSession.Instance.ClearSession();
            }
        }
    }
}