using SteamHub.ApiContract.Models.User;

namespace SteamHub.ApiContract.Models.Login
{
    /// <summary>
    /// Represents the response payload for a successful login operation.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Gets or sets the authenticated user's basic details.
        /// </summary>
        public User.User User { get; set; }

        /// <summary>
        /// Gets or sets the JWT or bearer token for subsequent requests.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets additional session details including expiration.
        /// </summary>
        public UserWithSessionDetails UserWithSessionDetails { get; set; }
    }
}