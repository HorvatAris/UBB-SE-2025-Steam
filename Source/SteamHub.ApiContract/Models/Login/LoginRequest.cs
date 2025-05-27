namespace SteamHub.ApiContract.Models.Login
{
    /// <summary>
    /// Represents the request payload for a login operation.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the user's email address or username.
        /// </summary>
        public string EmailOrUsername { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        public string Password { get; set; }
    }
}