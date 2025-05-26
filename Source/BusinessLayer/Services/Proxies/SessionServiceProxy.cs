using System;
using System.Threading.Tasks;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services.Proxies
{
    public class SessionServiceProxy : ServiceProxy, ISessionService
    {
        public SessionServiceProxy(string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
        }

        public Guid CreateNewSession(User user)
        {
            // This will be handled by the UserServiceProxy through the login process
            if (CurrentUser != null)
            {
                return CurrentUser.SessionId;
            }

            return Guid.Empty;
        }

        public void EndSession()
        {
            // Clear the local session info
            ClearCurrentUser();

            // Make an async call to the server to end the session
            _ = Task.Run(async () =>
            {
                try
                {
                    await PostAsync("Session/Logout", null);
                }
                catch
                {
                    // Ignore errors on logout - we've already cleared the local session
                }
            });
        }

        public User GetCurrentUser()
        {
            if (CurrentUser == null)
            {
                return null;
            }

            // Convert UserWithSessionDetails to User
            return new User
            {
                UserId = CurrentUser.UserId,
                Username = CurrentUser.Username,
                Email = CurrentUser.Email,
                IsDeveloper = CurrentUser.Developer,
                CreatedAt = CurrentUser.UserCreatedAt,
                LastLogin = CurrentUser.LastLogin
            };
        }

        public bool IsUserLoggedIn()
        {
            if (CurrentUser == null)
            {
                return false;
            }

            return CurrentUser.ExpiresAt > DateTime.Now;
        }

        public void RestoreSessionFromDatabase(Guid sessionId)
        {
            // Make an async call to validate and restore the session
            _ = Task.Run(async () =>
            {
                try
                {
                    var userWithSession = await GetAsync<UserWithSessionDetails>($"Session/{sessionId}");
                    if (userWithSession != null)
                    {
                        SetCurrentUser(userWithSession);
                    }
                }
                catch
                {
                    // Session invalid or expired - do nothing
                }
            });
        }

        public void CleanupExpiredSessions()
        {
            // This is handled on the server side
        }
    }
}