using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ApiContract.ServiceProxies
{
    /// <summary>
    /// A proxy class that implements <see cref="IPasswordResetService"/> and communicates with the password reset API.
    /// </summary>
    public class PasswordResetServiceProxy : ServiceProxy, IPasswordResetService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordResetServiceProxy"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL of the API endpoint. Defaults to localhost if not provided.</param>
        public PasswordResetServiceProxy(string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
        }

        /// <summary>
        /// Sends a password reset code to the user's email by calling the remote API.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>A tuple indicating whether the operation was successful and an accompanying message.</returns>
        public async Task<(bool isValid, string message)> SendResetCode(string email)
        {
            try
            {
                var response = await PostAsync<ResetCodeResponse>("PasswordReset/send", new { Email = email });
                return (response.IsValid, response.Message);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to send reset code: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifies whether the provided reset code is valid for the specified email.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="code">The reset code received by the user.</param>
        /// <returns>A tuple indicating whether the code is valid and a descriptive message.</returns>
        public (bool isValid, string message) VerifyResetCode(string email, string code)
        {
            try
            {
                var response = PostAsync<ResetCodeResponse>("PasswordReset/verify", new
                {
                    Email = email,
                    Code = code
                }).GetAwaiter().GetResult();

                return (response.IsValid, response.Message);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to verify reset code: {ex.Message}");
            }
        }

        /// <summary>
        /// Resets the user's password using a valid reset code.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="code">The reset code received by the user.</param>
        /// <param name="newPassword">The new password to be set for the user.</param>
        /// <returns>A tuple indicating whether the operation was successful and a descriptive message.</returns>
        public (bool isValid, string message) ResetPassword(string email, string code, string newPassword)
        {
            try
            {
                var response = PostAsync<ResetCodeResponse>("PasswordReset/reset", new
                {
                    Email = email,
                    Code = code,
                    NewPassword = newPassword
                }).GetAwaiter().GetResult();

                return (response.IsValid, response.Message);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to reset password: {ex.Message}");
            }
        }

        /// <summary>
        /// Triggers cleanup of expired reset codes on the server.
        /// </summary>
        /// <remarks>
        /// This is a server-side maintenance operation; errors are ignored silently.
        /// </remarks>
        public void CleanupExpiredCodes()
        {
            try
            {
                PostAsync("PasswordReset/cleanup", null).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                // Ignore errors
            }
        }

        /// <summary>
        /// Represents the response returned from the reset code API endpoints.
        /// </summary>
        private class ResetCodeResponse
        {
            /// <summary>
            /// Indicates whether the operation was successful.
            /// </summary>
            public bool IsValid { get; set; }

            /// <summary>
            /// A message accompanying the result (e.g., success or error details).
            /// </summary>
            public string Message { get; set; }
        }
    }
}
