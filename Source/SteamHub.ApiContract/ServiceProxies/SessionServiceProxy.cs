using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ApiContract.ServiceProxies
{
    /// <summary>
    /// Proxy implementation of <see cref="ISessionService"/>, managing session state via HTTP and local cache.
    /// </summary>
    public class SessionServiceProxy : ServiceProxy, ISessionService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionServiceProxy"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL for API endpoints.</param>
        public SessionServiceProxy(string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
        }

        /// <inheritdoc />
        public Guid CreateNewSession(User user)
        {
            // Session is managed by login in UserServiceProxy; return current session ID if exists
            return CurrentUser?.SessionId ?? Guid.Empty;
        }

        /// <inheritdoc />
        public void EndSession()
        {
            // Clear local session info
            ClearCurrentUser();

            // Notify server to end session asynchronously
            _ = Task.Run(async () =>
            {
                try
                {
                    await PostAsync("Session/Logout", null).ConfigureAwait(false);
                }
                catch
                {
                    // Ignore errors after local clear
                }
            });
        }

        /// <inheritdoc />
        public User GetCurrentUser()
        {
            if (CurrentUser == null)
            {
                return null;
            }

            // Map session details to User DTO
            return new User
            {
                UserId = CurrentUser.UserId,
                UserName = CurrentUser.Username,
                Email = CurrentUser.Email,
                UserRole = CurrentUser.Developer ? UserRole.Developer : UserRole.User
            };
        }

        /// <inheritdoc />
        public bool IsUserLoggedIn()
        {
            // Check local cache for session validity
            return CurrentUser != null && CurrentUser.ExpiresAt > DateTime.Now;
        }

        /// <inheritdoc />
        public void RestoreSessionFromDatabase(Guid sessionId)
        {
            // Attempt to restore session asynchronously
            _ = Task.Run(async () =>
            {
                try
                {
                    var sessionDetails = await GetAsync<UserWithSessionDetails>($"Session/{sessionId}").ConfigureAwait(false);
                    if (sessionDetails != null)
                    {
                        SetCurrentUser(sessionDetails);
                    }
                }
                catch
                {
                    // Do nothing if session invalid or expired
                }
            });
        }

        /// <inheritdoc />
        public void CleanupExpiredSessions()
        {
            // No-op: cleanup is handled server-side
        }
    }
}
