using SteamHub.ApiContract.Models.Session;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.ApiContract.Services.Interfaces
{
    public interface ISessionService
    {
        Task<Guid> CreateNewSessionAsync(User user);

        Task EndSessionAsync();

        Task<User?> GetCurrentUserAsync();

        Task<bool> IsUserLoggedInAsync();

        Task RestoreSessionFromDatabaseAsync(Guid sessionIdentifier);

        Task CleanupExpiredSessionsAsync();

        Task<bool> ValidateSessionAsync(Guid sessionId);

        Task<SessionDetails?> GetCurrentSessionDetailsAsync();
    }
}