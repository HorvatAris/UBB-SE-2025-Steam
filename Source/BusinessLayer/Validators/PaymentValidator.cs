using System;
using System.Text.RegularExpressions;

namespace BusinessLayer.Validators
{
    public static class PaymentValidator
    {
        // Validation constraints
        private static class ValidationLimits
        {
            public const int DefaultMaximumMonetaryAmount = 500;
            public const int MinimumMonetaryAmount = 1;

            public const int CardNumberLength = 16;
            public const int CardVerificationValueLength = 3;

            public const int MinimumPasswordLength = 8;
        }

        // Regular expression patterns
        private static class RegexPatterns
        {
            public const string Email = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
            public const string InvalidEmailChars = @"(^\.)|(\.\.)|(\.$)";

            public const string Password = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

            public const string CardNumber = @"^\d{16}$";
            public const string CardVerificationValue = @"^\d{3,4}$"; // Allow 3 or 4 digits for CVV
            public const string ExpirationDate = @"^(0[1-9]|1[0-2])\/\d{2}$";

            public const string NumericOnly = @"^\d+$";
        }

        // Format specifiers
        private static class Formats
        {
            public const string DateSeparator = "/";
        }

        public static bool IsEmailValid(string emailAddress)
        {
            return !string.IsNullOrEmpty(emailAddress) &&
                   Regex.IsMatch(emailAddress, RegexPatterns.Email) &&
                   !Regex.IsMatch(emailAddress, RegexPatterns.InvalidEmailChars);
        }

        public static bool IsPasswordValid(string password)
        {
            return !string.IsNullOrEmpty(password) &&
                   Regex.IsMatch(password, RegexPatterns.Password);
        }

        public static bool IsCardNameValid(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            // A valid card name must contain at least two parts (first and last name)
            return name.Split(' ').Length > 1;
        }

        public static bool IsCardNumberValid(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber))
            {
                return false;
            }
            return Regex.IsMatch(cardNumber, RegexPatterns.CardNumber);
        }

        public static bool IsCardVerificationValueValid(string cardVerificationValue)
        {
            if (string.IsNullOrEmpty(cardVerificationValue))
            {
                return false;
            }
            return Regex.IsMatch(cardVerificationValue, RegexPatterns.CardVerificationValue);
        }

        public static bool IsExpirationDateValid(string expirationDate)
        {
            if (string.IsNullOrEmpty(expirationDate))
            {
                return false;
            }

            bool isValidDateFormat = Regex.IsMatch(expirationDate, RegexPatterns.ExpirationDate);

            if (!isValidDateFormat)
            {
                return false;
            }

            string[] dateParts = expirationDate.Split(Formats.DateSeparator);
            int month = int.Parse(dateParts[0]);
            int year = int.Parse(dateParts[1]);
            int currentMonth = DateTime.Today.Month;
            int currentYear = DateTime.Today.Year % 100;

            return (year > currentYear) || (year == currentYear && month >= currentMonth);
        }

        public static bool IsMonetaryAmountValid(string input, int maximumAmount = ValidationLimits.DefaultMaximumMonetaryAmount)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            if (!Regex.IsMatch(input, RegexPatterns.NumericOnly))
            {
                return false;
            }

            if (int.TryParse(input, out int amount))
            {
                if (amount > maximumAmount || amount < ValidationLimits.MinimumMonetaryAmount)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public static System.Collections.Generic.IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> ValidatePaymentSubmission(
            string selectedPaymentMethod,
            string? cardNumber,
            string? expiryDate,
            string? cvv,
            string? payPalEmail)
        {
            if (selectedPaymentMethod == "Credit Card")
            {
                if (string.IsNullOrWhiteSpace(cardNumber))
                {
                    yield return new System.ComponentModel.DataAnnotations.ValidationResult("Card Number is required for Credit Card payment.", new[] { "CardNumber" });
                }
                else if (!IsCardNumberValid(cardNumber))
                {
                    yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid Card Number format.", new[] { "CardNumber" });
                }

                if (string.IsNullOrWhiteSpace(expiryDate))
                {
                    yield return new System.ComponentModel.DataAnnotations.ValidationResult("Expiry Date is required for Credit Card payment.", new[] { "ExpiryDate" });
                }
                else if (!IsExpirationDateValid(expiryDate))
                {
                    yield return new System.ComponentModel.DataAnnotations.ValidationResult("Expiry Date must be in MM/YY format and not be in the past (e.g., 06/28).", new[] { "ExpiryDate" });
                }

                if (string.IsNullOrWhiteSpace(cvv))
                {
                    yield return new System.ComponentModel.DataAnnotations.ValidationResult("CVV is required for Credit Card payment.", new[] { "CVV" });
                }
                else if (!IsCardVerificationValueValid(cvv))
                {
                    yield return new System.ComponentModel.DataAnnotations.ValidationResult("CVV must be 3 or 4 digits.", new[] { "CVV" });
                }
            }
            else if (selectedPaymentMethod == "PayPal")
            {
                if (string.IsNullOrWhiteSpace(payPalEmail))
                {
                    yield return new System.ComponentModel.DataAnnotations.ValidationResult("PayPal Email is required for PayPal payment.", new[] { "PayPalEmail" });
                }
                else if (!IsEmailValid(payPalEmail))
                {
                    yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid PayPal Email format.", new[] { "PayPalEmail" });
                }
            }
        }
    }
}