﻿namespace SteamHub.Web.Services
{
    /// <summary>
    /// Defines methods for user authentication and registration via external API.
    /// </summary>
    public interface IAuthManager
    {
        /// <summary>
        /// Attempts to sign in a user with the provided credentials.
        /// </summary>
        /// <param name="emailOrUsername">The user's email or username.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>True if sign-in succeeded; otherwise false.</returns>
        Task<bool> LoginAsync(string emailOrUsername, string password);

        /// <summary>
        /// Attempts to register a new user account.
        /// </summary>
        /// <param name="username">Desired username.</param>
        /// <param name="email">User's email address.</param>
        /// <param name="password">Desired password.</param>
        /// <param name="isDeveloper">Whether the user registers as a developer.</param>
        /// <returns>True if registration succeeded; otherwise false.</returns>
        Task<bool> RegisterAsync(string username, string email, string password, string confirmPassword, bool isDeveloper);

        /// <summary>
        /// Signs out the current user.
        /// </summary>
        Task LogoutAsync();
    }
}