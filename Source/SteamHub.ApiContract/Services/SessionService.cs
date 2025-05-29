using System;
using SteamHub.ApiContract.Models.Common;
using SteamHub.ApiContract.Models.Session;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ApiContract.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        }

        public async Task<Guid> CreateNewSessionAsync(User user)
        {
            var sessionDetails = await _sessionRepository.CreateSession(user.UserId);
            return sessionDetails.SessionId;
        }

        public async Task EndSessionAsync()
        {
            var currentSessionId = UserSession.Instance.CurrentSessionId;
            if (currentSessionId.HasValue)
            {
                await _sessionRepository.DeleteSession(currentSessionId.Value);
                UserSession.Instance.ClearSession();
            }
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            if (!UserSession.Instance.CurrentSessionId.HasValue)
                return null;

            var isValid = await ValidateSessionAsync(UserSession.Instance.CurrentSessionId.Value);
            if (!isValid)
            {
                UserSession.Instance.ClearSession();
                return null;
            }

            var userWithSession = await _sessionRepository.GetUserFromSession(UserSession.Instance.CurrentSessionId.Value);
            if (userWithSession == null)
            {
                UserSession.Instance.ClearSession();
                return null;
            }
            return new User
            {
                UserId = userWithSession.UserId,
                Username = userWithSession.Username,
                Email = userWithSession.Email,
                UserRole = userWithSession.Developer ? UserRole.Developer : UserRole.User
            };
        }

        public async Task<bool> IsUserLoggedInAsync()
        {
            if (!UserSession.Instance.CurrentSessionId.HasValue)
                return false;

            return await ValidateSessionAsync(UserSession.Instance.CurrentSessionId.Value);
        }

        public async Task RestoreSessionFromDatabaseAsync(Guid sessionId)
        {
            var sessionDetails = await _sessionRepository.GetSessionById(sessionId);
            if (sessionDetails == null)
                return;

            if (DateTime.UtcNow > sessionDetails.ExpiresAt)
            {
                await _sessionRepository.DeleteSession(sessionId);
                return;
            }

            UserSession.Instance.UpdateSession(
                sessionDetails.SessionId,
                sessionDetails.UserId,
                sessionDetails.CreatedAt,
                sessionDetails.ExpiresAt);
        }

        public async Task CleanupExpiredSessionsAsync()
        {
            var expiredSessionIds = await _sessionRepository.GetExpiredSessions();
            foreach (var expiredSessionId in expiredSessionIds)
            {
                await _sessionRepository.DeleteSession(expiredSessionId);
            }

            if (UserSession.Instance.CurrentSessionId.HasValue)
            {
                var isValid = await ValidateSessionAsync(UserSession.Instance.CurrentSessionId.Value);
                if (!isValid)
                {
                    UserSession.Instance.ClearSession();
                }
            }
        }

        public async Task<bool> ValidateSessionAsync(Guid sessionId)
        {
            var session = await _sessionRepository.GetSessionById(sessionId);
            return session != null && session.ExpiresAt > DateTime.UtcNow;
        }

        public async Task<SessionDetails?> GetCurrentSessionDetailsAsync()
        {
            if (!UserSession.Instance.CurrentSessionId.HasValue)
                return null;

            return await _sessionRepository.GetSessionById(UserSession.Instance.CurrentSessionId.Value);
        }
    }
}
