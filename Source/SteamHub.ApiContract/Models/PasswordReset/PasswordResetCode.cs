namespace SteamHub.ApiContract.Models.PasswordReset
{
    /// <summary>
    /// Represents a password reset code entry associated with a user.
    /// </summary>
    public class PasswordResetCode
    {
        /// <summary>
        /// The unique identifier for the password reset code entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the user this reset code is associated with.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The generated reset code sent to the user's email.
        /// </summary>
        public string ResetCode { get; set; }

        /// <summary>
        /// The time when the reset code expires and becomes invalid.
        /// </summary>
        public DateTime ExpirationTime { get; set; }

        /// <summary>
        /// Indicates whether the reset code has already been used.
        /// </summary>
        public bool Used { get; set; }

        /// <summary>
        /// The email address associated with the reset code (optional).
        /// </summary>
        public string? Email { get; set; }
    }
}