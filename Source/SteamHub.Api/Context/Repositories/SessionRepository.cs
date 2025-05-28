using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SteamHub.ApiContract.Models.Session;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models.Common;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Repositories;
using SessionDetails = SteamHub.ApiContract.Models.Session.SessionDetails;
using SessionDetailsDTO = SteamHub.Api.Entities.SessionDetails;

namespace SteamHub.Api.Context.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly DataContext context;
        private readonly UserSession user_session;

        public SessionRepository(DataContext newContext)
        {
            this.context = newContext ?? throw new ArgumentNullException(nameof(context));
            this.user_session = UserSession.Instance;
        }

        public async Task<SessionDetails> CreateSession(int userId)
        {
            // Remove any existing sessions for this user
            var old_session = await context.UserSessions.Where(session => session.UserId == userId).ToListAsync();
            context.UserSessions.RemoveRange(old_session);

            var session = new SessionDetailsDTO
            {
                UserId = userId,
                SessionId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(2),
            };

            await context.UserSessions.AddAsync(session);
            await context.SaveChangesAsync();

            // Update the current session
            await user_session.UpdateSessionAsync(
                session.SessionId,
                session.UserId,
                session.CreatedAt,
                session.ExpiresAt);

            return new SessionDetails
            {
                SessionId = session.SessionId,
                UserId = session.UserId,
                CreatedAt = session.CreatedAt,
                ExpiresAt = session.ExpiresAt
            };
        }

        public async Task DeleteUserSessions(int userId)
        {
            var session_to_delete = await context.UserSessions.Where(session => session.UserId == userId).ToListAsync();
            context.UserSessions.RemoveRange(session_to_delete);
            await context.SaveChangesAsync();

            // Clear the current session if it belongs to this user
            if (user_session.UserId == userId)
            {
                await user_session.ClearSessionAsync();
            }
        }

        public async Task DeleteSession(Guid sessionId)
        {
            var session = await context.UserSessions.FindAsync(sessionId);
            if (session != null)
            {
                context.UserSessions.Remove(session);
                await context.SaveChangesAsync();

                // Clear the current session if it matches
                if (user_session.CurrentSessionId == sessionId)
                {
                    await user_session.ClearSessionAsync();
                }
            }
        }

        public async Task<SessionDetails?> GetSessionById(Guid sessionId)
        {
            var session = await context.UserSessions.FindAsync(sessionId);
            if (session == null)
                return null;

            return new SessionDetails
            {
                SessionId = session.SessionId,
                UserId = session.UserId,
                CreatedAt = session.CreatedAt,
                ExpiresAt = session.ExpiresAt
            };
        }

        public async Task<UserWithSessionDetails?> GetUserFromSession(Guid sessionId)
        {
            var session = await context.UserSessions.FindAsync(sessionId);
            if (session == null || session.ExpiresAt <= DateTime.UtcNow)
            {
                if (session != null)
                {
                    context.UserSessions.Remove(session);
                    await context.SaveChangesAsync();
                }
                return null;
            }

            var user = await context.Users.FindAsync(session.UserId);
            if (user == null)
                return null;

            // Update the current session if it matches
            if (user_session.CurrentSessionId == sessionId)
            {
                await user_session.UpdateSessionAsync(
                    session.SessionId,
                    session.UserId,
                    session.CreatedAt,
                    session.ExpiresAt);
            }

            return new UserWithSessionDetails
            {
                SessionId = sessionId,
                CreatedAt = session.CreatedAt,
                ExpiresAt = session.ExpiresAt,
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Developer = user.UserRole == UserRole.Developer,
                UserCreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin
            };
        }

        public async Task<List<Guid>> GetExpiredSessions()
        {
            return await context.UserSessions
                .Where(session => session.ExpiresAt < DateTime.UtcNow)
                .Select(session => session.SessionId)
                .ToListAsync();
        }

        public async Task CleanupExpiredSessions()
        {
            var expired = await context.UserSessions
                    .Where(session => session.ExpiresAt < DateTime.UtcNow)
                    .ToListAsync();
            context.UserSessions.RemoveRange(expired);
            await context.SaveChangesAsync();

            // Clear the current session if it'session expired
            if (user_session.CurrentSessionId.HasValue)
            {
                var session = await GetSessionById(user_session.CurrentSessionId.Value);
                if (session == null || session.ExpiresAt <= DateTime.UtcNow)
                {
                    await user_session.ClearSessionAsync();
                }
            }
        }
    }
}
