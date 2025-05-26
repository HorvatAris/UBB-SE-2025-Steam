using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;
using BusinessLayer.DataContext;

namespace BusinessLayer.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ApplicationDbContext context;

        public SessionRepository(ApplicationDbContext newContext)
        {
            this.context = newContext ?? throw new ArgumentNullException(nameof(context));
        }

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

        public void DeleteUserSessions(int userId)
        {
            var toDelete = context.UserSessions.Where(s => s.UserId == userId).ToList();
            context.UserSessions.RemoveRange(toDelete);
            context.SaveChanges();
        }

        public void DeleteSession(Guid sessionId)
        {
            var session = context.UserSessions.Find(sessionId);
            if (session != null)
            {
                context.UserSessions.Remove(session);
                context.SaveChanges();
            }
        }

        public SessionDetails GetSessionById(Guid sessionId)
        {
            return context.UserSessions.Find(sessionId);
        }

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
                    Username = user.Username,
                    Email = user.Email,
                    Developer = user.IsDeveloper,
                    UserCreatedAt = user.CreatedAt,
                    LastLogin = user.LastLogin
                };
        }

        public List<Guid> GetExpiredSessions()
        {
            return context.UserSessions
                .Where(s => s.ExpiresAt < DateTime.Now)
                .Select(s => s.SessionId)
                .ToList();
        }

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