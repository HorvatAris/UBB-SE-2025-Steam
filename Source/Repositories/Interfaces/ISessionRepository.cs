using BusinessLayer.Models;

namespace BusinessLayer.Repositories.Interfaces
{
    public interface ISessionRepository
    {
        SessionDetails CreateSession(int userId);
        void DeleteUserSessions(int userId);
        void DeleteSession(Guid sessionId);
        SessionDetails GetSessionById(Guid sessionId);
        UserWithSessionDetails GetUserFromSession(Guid sessionId);
        List<Guid> GetExpiredSessions();
    }
}
