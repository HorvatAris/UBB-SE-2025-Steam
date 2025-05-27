namespace SteamHub.ApiContract.Repositories
{
    /// <summary>
    /// Repository interface for persisting and managing password reset codes.
    /// </summary>
    public interface IPasswordResetRepository
    {
        /// <summary>
        /// Stores a reset code for a user along with its expiration time.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="code">The reset code.</param>
        /// <param name="expiryTime">The expiration time for the code.</param>
        void StoreResetCode(int userId, string code, DateTime expiryTime);

        /// <summary>
        /// Verifies whether the provided reset code matches the stored one for the given email.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="code">The reset code to verify.</param>
        /// <returns>True if the code is valid and matches; otherwise, false.</returns>
        bool VerifyResetCode(string email, string code);

        /// <summary>
        /// Validates whether the provided reset code is still valid (e.g., not expired).
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="code">The reset code to validate.</param>
        /// <returns>True if the code is valid and not expired; otherwise, false.</returns>
        bool ValidateResetCode(string email, string code);

        /// <summary>
        /// Resets the password for the user if the reset code is valid.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="code">The reset code associated with the request.</param>
        /// <param name="hashedPassword">The new hashed password to be set.</param>
        /// <returns>True if the password was successfully reset; otherwise, false.</returns>
        bool ResetPassword(string email, string code, string hashedPassword);

        /// <summary>
        /// Deletes any expired reset codes from storage.
        /// </summary>
        void CleanupExpiredCodes();
    }
}
