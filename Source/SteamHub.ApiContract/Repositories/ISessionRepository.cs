using SteamHub.ApiContract.Models.Session;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.ApiContract.Repositories
{
    public interface ISessionRepository
    {
        Task<SessionDetails> CreateSession(int userId);

        Task DeleteUserSessions(int userId);

        Task DeleteSession(Guid sessionId);

        Task<SessionDetails> GetSessionById(Guid sessionId);

        Task<UserWithSessionDetails?> GetUserFromSession(Guid sessionId);

        Task<List<Guid>> GetExpiredSessions();
    }
}