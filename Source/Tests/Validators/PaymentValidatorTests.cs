using System;
using NUnit.Framework;
using BusinessLayer.Validators;

namespace Tests.Validators
{
    [TestFixture]
    public class PaymentValidatorTests
    {
        private const int MAXIMUM_MONEY_AMOUNT = 500;

        #region IsMonetaryAmountValid Tests

        [Test]
        public void IsMonetaryAmountValid_ValidAmount_ReturnsTrue()
        {
            // Arrange
            string amountString = "250";

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMonetaryAmountValid_MaximumAmount_ReturnsTrue()
        {
            // Arrange
            string amountString = MAXIMUM_MONEY_AMOUNT.ToString();

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsMonetaryAmountValid_ExceedsMaximum_ReturnsFalse()
        {
            // Arrange
            string amountString = (MAXIMUM_MONEY_AMOUNT + 1).ToString();

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_ZeroAmount_ReturnsFalse()
        {
            // Arrange
            string amountString = "0";

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_NegativeAmount_ReturnsFalse()
        {
            // Arrange
            string amountString = "-10";

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_NonNumericString_ReturnsFalse()
        {
            // Arrange
            string amountString = "abc";

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_EmptyString_ReturnsFalse()
        {
            // Arrange
            string amountString = string.Empty;

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_NullString_ReturnsFalse()
        {
            // Arrange
            string amountString = null;

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsMonetaryAmountValid_ValidDigitsButCannotParse_ReturnsFalse()
        {
            // Arrange
            // A number that's too large to be parsed as an int but contains only digits
            string amountString = "9999999999999999999";

            // Act
            bool result = PaymentValidator.IsMonetaryAmountValid(amountString, MAXIMUM_MONEY_AMOUNT);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region IsCardNameValid Tests

        [Test]
        public void IsCardNameValid_ValidName_ReturnsTrue()
        {
            // Arrange
            string cardholderName = "John Smith";

            // Act
            bool result = PaymentValidator.IsCardNameValid(cardholderName);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCardNameValid_EmptyName_ReturnsFalse()
        {
            // Arrange
            string cardholderName = string.Empty;

            // Act
            bool result = PaymentValidator.IsCardNameValid(cardholderName);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNameValid_NullName_ReturnsFalse()
        {
            // Arrange
            string cardholderName = null;

            // Act
            bool result = PaymentValidator.IsCardNameValid(cardholderName);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNameValid_SingleName_ReturnsFalse()
        {
            // Arrange
            string cardholderName = "John";

            // Act
            bool result = PaymentValidator.IsCardNameValid(cardholderName);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region IsCardNumberValid Tests

        [Test]
        public void IsCardNumberValid_VisaNumber_ReturnsTrue()
        {
            // Arrange
            string cardNumber = "4111111111111111";

            // Act
            bool result = PaymentValidator.IsCardNumberValid(cardNumber);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCardNumberValid_MastercardNumber_ReturnsTrue()
        {
            // Arrange
            string cardNumber = "5555555555554444";

            // Act
            bool result = PaymentValidator.IsCardNumberValid(cardNumber);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCardNumberValid_AmexNumber_ReturnsTrue()
        {
            // Arrange
            string americanExpressCardNumber = "3782822463100051";

            // Act
            bool result = PaymentValidator.IsCardNumberValid(americanExpressCardNumber);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCardNumberValid_TooLongNumber_ReturnsFalse()
        {
            // Arrange
            string tooLongCardNumber = "12345678901234562";

            // Act
            bool result = PaymentValidator.IsCardNumberValid(tooLongCardNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNumberValid_TooShortNumber_ReturnsFalse()
        {
            // Arrange
            string tooShortCardNumber = "1234";

            // Act
            bool result = PaymentValidator.IsCardNumberValid(tooShortCardNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNumberValid_EmptyNumber_ReturnsFalse()
        {
            // Arrange
            string emptyCardNumber = string.Empty;

            // Act
            bool result = PaymentValidator.IsCardNumberValid(emptyCardNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNumberValid_NullNumber_ReturnsFalse()
        {
            // Arrange
            string nullCardNumber = null;

            // Act
            bool result = PaymentValidator.IsCardNumberValid(nullCardNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardNumberValid_NonNumericInput_ReturnsFalse()
        {
            // Arrange
            string nonNumericCardNumber = "123abc456def";

            // Act
            bool result = PaymentValidator.IsCardNumberValid(nonNumericCardNumber);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region IsCardVerificationValueValid Tests

        [Test]
        public void IsCardVerificationValueValid_ThreeDigitCVV_ReturnsTrue()
        {
            // Arrange
            string cardVerificationValueString = "123";

            // Act
            bool result = PaymentValidator.IsCardVerificationValueValid(cardVerificationValueString);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCardVerificationValueValid_AllNinesCVV_ReturnsTrue()
        {
            // Arrange
            string cardVerificationValueString = "999";

            // Act
            bool result = PaymentValidator.IsCardVerificationValueValid(cardVerificationValueString);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCardVerificationValueValid_AllZerosCVV_ReturnsTrue()
        {
            // Arrange
            string cardVerificationValueString = "000";

            // Act
            bool result = PaymentValidator.IsCardVerificationValueValid(cardVerificationValueString);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCardVerificationValueValid_TooShortCVV_ReturnsFalse()
        {
            // Arrange
            string tooShortCardVerificationValue = "12";

            // Act
            bool result = PaymentValidator.IsCardVerificationValueValid(tooShortCardVerificationValue);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardVerificationValueValid_TooLongCVV_ReturnsFalse()
        {
            // Arrange
            string tooLongCardVerificationValue = "12345";

            // Act
            bool result = PaymentValidator.IsCardVerificationValueValid(tooLongCardVerificationValue);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardVerificationValueValid_EmptyCVV_ReturnsFalse()
        {
            // Arrange
            string emptyCardVerificationValue = string.Empty;

            // Act
            bool result = PaymentValidator.IsCardVerificationValueValid(emptyCardVerificationValue);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardVerificationValueValid_NullCVV_ReturnsFalse()
        {
            // Arrange
            string nullCardVerificationValue = null;

            // Act
            bool result = PaymentValidator.IsCardVerificationValueValid(nullCardVerificationValue);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCardVerificationValueValid_NonNumericCVV_ReturnsFalse()
        {
            // Arrange
            string nonNumericCardVerificationValue = "12A";

            // Act
            bool result = PaymentValidator.IsCardVerificationValueValid(nonNumericCardVerificationValue);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region IsExpirationDateValid Tests

        [Test]
        public void IsExpirationDateValid_FutureDate_ReturnsTrue()
        {
            // Arrange
            string futureExpirationDateString = "12/28";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(futureExpirationDateString);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsExpirationDateValid_CurrentMonthAndYear_ReturnsTrue()
        {
            // Arrange
            int currentMonth = DateTime.Now.Month;
            int currentYearTwoDigits = DateTime.Now.Year % 100;
            string currentExpirationDateString = $"{currentMonth:D2}/{currentYearTwoDigits:D2}";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(currentExpirationDateString);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsExpirationDateValid_PastDate_ReturnsFalse()
        {
            // Arrange
            int lastYearTwoDigits = (DateTime.Now.Year - 1) % 100;
            string pastExpirationDateString = $"12/{lastYearTwoDigits:D2}";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(pastExpirationDateString);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_InvalidMonth_ReturnsFalse()
        {
            // Arrange
            string expirationDateWithInvalidMonth = "13/30";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(expirationDateWithInvalidMonth);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_SingleDigitMonthFormat_ReturnsFalse()
        {
            // Arrange
            string expirationDateWithSingleDigitMonth = "1/23";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(expirationDateWithSingleDigitMonth);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_NoSlashFormat_ReturnsFalse()
        {
            // Arrange
            string expirationDateWithoutSlash = "1234";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(expirationDateWithoutSlash);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_DashSeparatorFormat_ReturnsFalse()
        {
            // Arrange
            string expirationDateWithDashSeparator = "12-23";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(expirationDateWithDashSeparator);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_DotSeparatorFormat_ReturnsFalse()
        {
            // Arrange
            string expirationDateWithDotSeparator = "12.23";

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(expirationDateWithDotSeparator);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_EmptyDate_ReturnsFalse()
        {
            // Arrange
            string emptyExpirationDateString = string.Empty;

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(emptyExpirationDateString);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsExpirationDateValid_NullDate_ReturnsFalse()
        {
            // Arrange
            string nullExpirationDateString = null;

            // Act
            bool result = PaymentValidator.IsExpirationDateValid(nullExpirationDateString);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region IsEmailValid Tests

        [Test]
        public void IsEmailValid_SimpleValidEmail_ReturnsTrue()
        {
            // Arrange
            string email = "user@example.com";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsEmailValid_ComplexValidEmail_ReturnsTrue()
        {
            // Arrange
            string email = "name.surname@domain.co.uk";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsEmailValid_ValidEmailWithNumbers_ReturnsTrue()
        {
            // Arrange
            string email = "user123@gmail.com";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsEmailValid_ValidEmailWithPlusTag_ReturnsTrue()
        {
            // Arrange
            string email = "user+tag@domain.org";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsEmailValid_MissingAtSymbol_ReturnsFalse()
        {
            // Arrange
            string email = "plainaddress";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_MissingUsername_ReturnsFalse()
        {
            // Arrange
            string email = "@missingusername.com";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_MissingDomainName_ReturnsFalse()
        {
            // Arrange
            string email = "user@.com";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_DoubleDotInDomain_ReturnsFalse()
        {
            // Arrange
            string email = "user@domain..com";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_DoubleAtSymbol_ReturnsFalse()
        {
            // Arrange
            string email = "user@domain@domain.com";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_DotAtEnd_ReturnsFalse()
        {
            // Arrange
            string email = "user@domain.com.";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_DotAtStart_ReturnsFalse()
        {
            // Arrange
            string email = ".user@domain.com";

            // Act
            bool result = PaymentValidator.IsEmailValid(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_EmptyEmail_ReturnsFalse()
        {
            // Arrange
            string emptyEmail = string.Empty;

            // Act
            bool result = PaymentValidator.IsEmailValid(emptyEmail);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_NullEmail_ReturnsFalse()
        {
            // Arrange
            string nullEmail = null;

            // Act
            bool result = PaymentValidator.IsEmailValid(nullEmail);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region IsPasswordValid Tests

        [Test]
        public void IsPasswordValid_ValidPassword_ReturnsTrue()
        {
            // Arrange
            string validPassword = "Pass123!";

            // Act
            bool result = PaymentValidator.IsPasswordValid(validPassword);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsPasswordValid_EmptyPassword_ReturnsFalse()
        {
            // Arrange
            string emptyPassword = string.Empty;

            // Act
            bool result = PaymentValidator.IsPasswordValid(emptyPassword);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_NullPassword_ReturnsFalse()
        {
            // Arrange
            string nullPassword = null;

            // Act
            bool result = PaymentValidator.IsPasswordValid(nullPassword);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_NoUppercase_ReturnsFalse()
        {
            // Arrange
            string password = "pass123!";

            // Act
            bool result = PaymentValidator.IsPasswordValid(password);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_NoLowercase_ReturnsFalse()
        {
            // Arrange
            string password = "PASS123!";

            // Act
            bool result = PaymentValidator.IsPasswordValid(password);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_NoDigit_ReturnsFalse()
        {
            // Arrange
            string password = "Password!";

            // Act
            bool result = PaymentValidator.IsPasswordValid(password);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_NoSpecialCharacter_ReturnsFalse()
        {
            // Arrange
            string password = "Password123";

            // Act
            bool result = PaymentValidator.IsPasswordValid(password);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_TooShort_ReturnsFalse()
        {
            // Arrange
            string password = "Pa1!";

            // Act
            bool result = PaymentValidator.IsPasswordValid(password);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion
    }
}