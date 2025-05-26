using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BusinessLayer.Validators;

namespace SteamProfileWeb.ViewModels
{
    /// <summary>
    /// ViewModel for user registration, implements server-side validation by calling backend validators.
    /// </summary>
    public class RegisterViewModel : IValidatableObject
    {
        /// <summary>
        /// The desired username for the new account.
        /// </summary>
        [Required(ErrorMessage = "Username is required.")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        /// <summary>
        /// The email address for the new account.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// The password for the new account.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// Confirmation of the password to avoid typos.
        /// </summary>
        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Indicates if the new user should have developer privileges.
        /// </summary>
        [Display(Name = "Register as Developer?")]
        public bool IsDeveloper { get; set; }

        /// <summary>
        /// Validates additional rules that cannot be expressed with data annotations.
        /// Invokes backend <see cref="UserValidator"/> methods.
        /// </summary>
        /// <param name="validationContext">Context for validation.</param>
        /// <returns>Enumeration of validation failures, if any.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // Validate username using backend logic
            if (!UserValidator.IsValidUsername(Username))
            {
                results.Add(new ValidationResult(
                    "Invalid username.", new[] { nameof(Username) }));
            }

            // Validate email format using backend logic
            if (!UserValidator.IsEmailValid(Email))
            {
                results.Add(new ValidationResult(
                    "Invalid email format.", new[] { nameof(Email) }));
            }

            // Validate password strength using backend logic
            if (!UserValidator.IsPasswordValid(Password))
            {
                results.Add(new ValidationResult(
                    "Password must be at least 8 characters, include upper, lower, digit, and special char.",
                    new[] { nameof(Password) }));
            }

            // Double-check that password and confirmation match exactly
            if (!string.Equals(Password, ConfirmPassword, StringComparison.Ordinal))
            {
                results.Add(new ValidationResult(
                    "Passwords do not match.", new[] { nameof(ConfirmPassword) }));
            }

            return results;
        }
    }
}