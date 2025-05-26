using System;
using System.Text.RegularExpressions;

namespace BusinessLayer.Validators
{
    public class PasswordResetValidator
    {
        public (bool isValid, string errorMessage) ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return (false, "Email address is required.");
            }

            var emailPattern = @"^[a-zA-Z0-9][a-zA-Z0-9._%+-]*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                return (false, "Invalid email format.");
            }

            return (true, string.Empty);
        }

        public (bool isValid, string errorMessage) ValidateResetCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return (false, "Reset code is required.");
            }

            if (!Regex.IsMatch(code, @"^\d{6}$"))
            {
                return (false, "Reset code must be a 6-digit number.");
            }

            return (true, string.Empty);
        }

        public (bool isValid, string errorMessage) ValidatePassword(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                return (false, "New password is required.");
            }

            if (newPassword.Length < 8)
            {
                return (false, "Password must be at least 8 characters long.");
            }

            if (!Regex.IsMatch(newPassword, @"[A-Z]"))
            {
                return (false, "Password must contain at least one uppercase letter.");
            }

            if (!Regex.IsMatch(newPassword, @"[a-z]"))
            {
                return (false, "Password must contain at least one lowercase letter.");
            }

            if (!Regex.IsMatch(newPassword, @"[0-9]"))
            {
                return (false, "Password must contain at least one digit.");
            }

            if (!Regex.IsMatch(newPassword, @"[!@#$%^&*(),.?""':{}|<>]"))
            {
                return (false, "Password must contain at least one special character.");
            }

            return (true, string.Empty);
        }
    }
}