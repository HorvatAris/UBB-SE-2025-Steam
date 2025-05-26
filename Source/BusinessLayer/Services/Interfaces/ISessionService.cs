using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface ISessionService
    {
        Guid CreateNewSession(User user);
        void EndSession();
        User GetCurrentUser();
        bool IsUserLoggedIn();
        void RestoreSessionFromDatabase(Guid sessionIdentifier);
        void CleanupExpiredSessions();
    }
}
