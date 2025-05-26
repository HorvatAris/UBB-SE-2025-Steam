namespace SteamHub.ApiContract.Services.Interfaces
{
    /// <summary>
    /// Service responsible for handling password reset operations for users.
    /// </summary>
    public interface IPasswordResetService
    {
        /// <summary>
        /// Sends a password reset code to the user's email.
        /// </summary>
        /// <param name="email">The email address associated with the user account.</param>
        /// <returns>A tuple indicating whether the operation was successful and a descriptive message.</returns>
        Task<(bool isValid, string message)> SendResetCode(string email);

        /// <summary>
        /// Verifies whether a provided reset code is valid for the specified email.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="code">The reset code to verify.</param>
        /// <returns>A tuple indicating whether the code is valid and a descriptive message.</returns>
        (bool isValid, string message) VerifyResetCode(string email, string code);

        /// <summary>
        /// Resets the user's password using a valid reset code.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="code">The reset code received by the user.</param>
        /// <param name="newPassword">The new password to set for the user.</param>
        /// <returns>A tuple indicating the success or failure of the operation and a message.</returns>
        (bool isValid, string message) ResetPassword(string email, string code, string newPassword);

        /// <summary>
        /// Removes expired password reset codes from storage.
        /// </summary>
        void CleanupExpiredCodes();
    }
}