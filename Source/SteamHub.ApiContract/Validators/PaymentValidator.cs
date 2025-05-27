using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.ApiContract.Validators
{
    /// <summary>
    /// Provides validation methods for credit card, PayPal, and payment submissions.
    /// </summary>
    public static class PaymentValidator
    {
        private static class ValidationLimits
        {
            public const int DefaultMaximumMonetaryAmount = 500;
            public const int MinimumMonetaryAmount = 1;
            public const int CardNumberLength = 16;
            public const int CardVerificationValueLength = 3;
            public const int MinimumPasswordLength = 8;
        }

        private static class RegexPatterns
        {
            public const string Email = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
            public const string InvalidEmailChars = @"(^\.)|(\.\.)|(\.$)";
            public const string Password = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            public const string CardNumber = @"^\d{16}$";
            public const string CardVerificationValue = @"^\d{3,4}$";
            public const string ExpirationDate = @"^(0[1-9]|1[0-2])\/\d{2}$";
            public const string NumericOnly = @"^\d+$";
        }

        private static class Formats
        {
            public const string DateSeparator = "/";
        }

        /// <summary>
        /// Determines if the given email address is in valid format and not containing invalid sequences.
        /// </summary>
        public static bool IsEmailValid(string emailAddress)
            => !string.IsNullOrEmpty(emailAddress)
                && Regex.IsMatch(emailAddress, RegexPatterns.Email)
                && !Regex.IsMatch(emailAddress, RegexPatterns.InvalidEmailChars);

        /// <summary>
        /// Determines if the given password meets complexity requirements.
        /// </summary>
        public static bool IsPasswordValid(string password)
            => !string.IsNullOrEmpty(password)
                && Regex.IsMatch(password, RegexPatterns.Password);

        /// <summary>
        /// Determines if the cardholder name contains at least a first and last name.
        /// </summary>
        public static bool IsCardNameValid(string name)
            => !string.IsNullOrEmpty(name) && name.Split(' ').Length > 1;

        /// <summary>
        /// Determines if the credit card number is exactly 16 digits.
        /// </summary>
        public static bool IsCardNumberValid(string cardNumber)
            => !string.IsNullOrEmpty(cardNumber) && Regex.IsMatch(cardNumber, RegexPatterns.CardNumber);

        /// <summary>
        /// Determines if the CVV is either 3 or 4 digits.
        /// </summary>
        public static bool IsCardVerificationValueValid(string cardVerificationValue)
            => !string.IsNullOrEmpty(cardVerificationValue)
                && Regex.IsMatch(cardVerificationValue, RegexPatterns.CardVerificationValue);

        /// <summary>
        /// Determines if the expiration date is in MM/YY format and not in the past.
        /// </summary>
        public static bool IsExpirationDateValid(string expirationDate)
        {
            if (string.IsNullOrEmpty(expirationDate) || !Regex.IsMatch(expirationDate, RegexPatterns.ExpirationDate))
                return false;

            var parts = expirationDate.Split(Formats.DateSeparator);
            int month = int.Parse(parts[0]);
            int year = int.Parse(parts[1]);
            int currentMonth = DateTime.Today.Month;
            int currentYear = DateTime.Today.Year % 100;

            return year > currentYear || (year == currentYear && month >= currentMonth);
        }

        /// <summary>
        /// Determines if the monetary amount string is numeric and within allowed bounds.
        /// </summary>
        public static bool IsMonetaryAmountValid(string input, int maximumAmount = ValidationLimits.DefaultMaximumMonetaryAmount)
            => !string.IsNullOrEmpty(input)
                && Regex.IsMatch(input, RegexPatterns.NumericOnly)
                && int.TryParse(input, out var amount)
                && amount >= ValidationLimits.MinimumMonetaryAmount
                && amount <= maximumAmount;

        /// <summary>
        /// Validates payment submission fields for the selected payment method and returns a list of validation results.
        /// </summary>
        public static IEnumerable<ValidationResult> ValidatePaymentSubmission(
            string selectedPaymentMethod,
            string? cardNumber,
            string? expiryDate,
            string? cvv,
            string? payPalEmail)
        {
            if (selectedPaymentMethod == "Credit Card")
            {
                if (string.IsNullOrWhiteSpace(cardNumber))
                    yield return new ValidationResult("Card Number is required for Credit Card payment.", new[] { nameof(cardNumber) });
                else if (!IsCardNumberValid(cardNumber))
                    yield return new ValidationResult("Invalid Card Number format.", new[] { nameof(cardNumber) });

                if (string.IsNullOrWhiteSpace(expiryDate))
                    yield return new ValidationResult("Expiry Date is required for Credit Card payment.", new[] { nameof(expiryDate) });
                else if (!IsExpirationDateValid(expiryDate))
                    yield return new ValidationResult("Expiry Date must be in MM/YY format and not be in the past.", new[] { nameof(expiryDate) });

                if (string.IsNullOrWhiteSpace(cvv))
                    yield return new ValidationResult("CVV is required for Credit Card payment.", new[] { nameof(cvv) });
                else if (!IsCardVerificationValueValid(cvv))
                    yield return new ValidationResult("CVV must be 3 or 4 digits.", new[] { nameof(cvv) });
            }
            else if (selectedPaymentMethod == "PayPal")
            {
                if (string.IsNullOrWhiteSpace(payPalEmail))
                    yield return new ValidationResult("PayPal Email is required for PayPal payment.", new[] { nameof(payPalEmail) });
                else if (!IsEmailValid(payPalEmail))
                    yield return new ValidationResult("Invalid PayPal Email format.", new[] { nameof(payPalEmail) });
            }
        }
    }
}