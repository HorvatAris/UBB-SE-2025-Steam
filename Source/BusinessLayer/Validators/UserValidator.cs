using System;
using System.Linq;
using System.Text.RegularExpressions;
using BusinessLayer.Models;

namespace BusinessLayer.Validators
{
    public static class UserValidator
    {
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            return true;
        }

        public static bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                return false;
            }

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(ch => "@_.,/%^#$!%*?&".Contains(ch));

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }

        public static bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        public static void ValidateUser(User user)
        {
            if (!IsValidUsername(user.Username))
            {
                throw new InvalidOperationException("User Username is not valid.");
            }

            if (!IsPasswordValid(user.Password))
            {
                throw new InvalidOperationException("User Password is not valid.");
            }

            if (!IsEmailValid(user.Email))
            {
                throw new InvalidOperationException("User Email is not valid.");
            }
        }
    }
}