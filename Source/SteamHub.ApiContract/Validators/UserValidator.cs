using System.Text.RegularExpressions;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.ApiContract.Validators
{
    /// <summary>
    /// Provides validation methods for User DTO instances.
    /// </summary>
    public static class UserValidator
    {
        /// <summary>
        /// Determines if a username is non-empty.
        /// </summary>
        public static bool IsValidUsername(string username) => !string.IsNullOrEmpty(username);

        /// <summary>
        /// Determines if a password meets complexity requirements.
        /// </summary>
        public static bool IsPasswordValid(string password)
            => !string.IsNullOrWhiteSpace(password)
               && password.Length >= 8
               && password.Any(char.IsUpper)
               && password.Any(char.IsLower)
               && password.Any(char.IsDigit)
               && password.Any(ch => "@_.,/%^#$!%*?&".Contains(ch));

        /// <summary>
        /// Determines if an email address is in valid format.
        /// </summary>
        public static bool IsEmailValid(string email)
            => !string.IsNullOrWhiteSpace(email)
               && Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        /// <summary>
        /// Validates all properties of a <see cref="User"/> and throws an <see cref="InvalidOperationException"/> on failure.
        /// </summary>
        public static void ValidateUser(User user)
        {
            if (!IsValidUsername(user.Username))
                throw new InvalidOperationException("User UserName is not valid.");

            if (!IsPasswordValid(user.Password))
                throw new InvalidOperationException("User Password is not valid.");

            if (!IsEmailValid(user.Email))
                throw new InvalidOperationException("User Email is not valid.");
        }
    }
}