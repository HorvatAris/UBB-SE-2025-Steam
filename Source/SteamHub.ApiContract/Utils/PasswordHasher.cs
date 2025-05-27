namespace SteamHub.ApiContract.Utils
{
    /// <summary>
    /// Provides methods for hashing and verifying user passwords using BCrypt.
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Generates a salted BCrypt hash for the given plain-text password.
        /// </summary>
        /// <param name="password">The plain-text password to hash.</param>
        /// <returns>A BCrypt hashed representation of the password.</returns>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifies that a plain-text password matches a previously hashed password.
        /// </summary>
        /// <param name="password">The plain-text password to verify.</param>
        /// <param name="hashedPassword">The BCrypt hashed password to compare against.</param>
        /// <returns><c>true</c> if the password matches the hash; otherwise, <c>false</c>.</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
            {
                return false;
            }
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}