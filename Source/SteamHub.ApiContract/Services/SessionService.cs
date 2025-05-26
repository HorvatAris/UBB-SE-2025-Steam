using System;
using SteamHub.ApiContract.Models.Session;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ApiContract.Services
{
    /// <summary>
    /// Manages user session lifecycle, including creation, validation, restoration, and cleanup.
    /// </summary>
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionService"/> class.
        /// </summary>
        /// <param name="sessionRepository">Repository for session persistence operations.</param>
        /// <param name="userRepository">Repository for retrieving user details.</param>
        public SessionService(ISessionRepository sessionRepository, IUserRepository userRepository)
        {
            _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <inheritdoc />
        public Guid CreateNewSession(User user)
        {
            _sessionRepository.DeleteUserSessions(user.UserId);
            var sessionDetails = _sessionRepository.CreateSession(user.UserId);

            UserSession.Instance.UpdateSession(
                sessionDetails.SessionId,
                user.UserId,
                sessionDetails.CreatedAt,
                sessionDetails.ExpiresAt);

            return sessionDetails.SessionId;
        }

        /// <inheritdoc />
        public void EndSession()
        {
            var currentSessionId = UserSession.Instance.CurrentSessionId;
            if (currentSessionId.HasValue)
            {
                _sessionRepository.DeleteSession(currentSessionId.Value);
                UserSession.Instance.ClearSession();
            }
        }

        /// <inheritdoc />
        public User GetCurrentUser()
        {
            if (!UserSession.Instance.IsSessionValid())
            {
                if (UserSession.Instance.CurrentSessionId.HasValue)
                {
                    _sessionRepository.DeleteSession(UserSession.Instance.CurrentSessionId.Value);
                    UserSession.Instance.ClearSession();
                }
                return null;
            }

            return _userRepository.GetUserById(UserSession.Instance.UserId);
        }

        /// <inheritdoc />
        public bool IsUserLoggedIn()
            => UserSession.Instance.IsSessionValid();

        /// <inheritdoc />
        public void RestoreSessionFromDatabase(Guid sessionId)
        {
            var sessionDetails = _sessionRepository.GetSessionById(sessionId);
            if (sessionDetails == null)
                return;

            if (DateTime.Now > sessionDetails.ExpiresAt)
            {
                _sessionRepository.DeleteSession(sessionId);
                return;
            }

            UserSession.Instance.UpdateSession(
                sessionDetails.SessionId,
                sessionDetails.UserId,
                sessionDetails.CreatedAt,
                sessionDetails.ExpiresAt);
        }

        /// <summary>
        /// Deletes all expired sessions and clears the current session if it is expired.
        /// </summary>
        public void CleanupExpiredSessions()
        {
            var expiredSessionIds = _sessionRepository.GetExpiredSessions();
            foreach (var expiredSessionId in expiredSessionIds)
            {
                _sessionRepository.DeleteSession(expiredSessionId);
            }

            if (UserSession.Instance.CurrentSessionId.HasValue && !UserSession.Instance.IsSessionValid())
            {
                UserSession.Instance.ClearSession();
            }
        }
    }
}
