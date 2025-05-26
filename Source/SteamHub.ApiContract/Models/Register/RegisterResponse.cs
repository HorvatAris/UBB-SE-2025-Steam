using SteamHub.ApiContract.Models.User;

namespace SteamHub.ApiContract.Models.Register
{
    /// <summary>
    /// Represents the response payload for a successful registration operation.
    /// </summary>
    public class RegisterResponse
    {
        /// <summary>
        /// Gets or sets the newly created user's basic details.
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