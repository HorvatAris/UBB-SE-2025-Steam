using System;
using NUnit.Framework;
using BusinessLayer.Validators;

namespace Tests.Validators
{
    [TestFixture]
    public class PasswordResetValidatorTests
    {
        private PasswordResetValidator passwordResetValidator;

        [SetUp]
        public void Setup()
        {
            passwordResetValidator = new PasswordResetValidator();
        }

        #region ValidateEmail Tests

        [Test]
        public void ValidateEmail_WithValidEmail_ReturnsTrue()
        {
            // Arrange
            string email = "test@example.com";

            // Act
            var result = passwordResetValidator.ValidateEmail(email);

            // Assert
            Assert.That(result.isValid, Is.True);
        }

        [Test]
        public void ValidateEmail_WithValidEmail_ReturnsEmptyMessage()
        {
            // Arrange
            string email = "test@example.com";

            // Act
            var result = passwordResetValidator.ValidateEmail(email);

            // Assert
            Assert.That(result.errorMessage, Is.Empty);
        }

        [Test]
        public void ValidateEmail_WithEmptyEmail_ReturnsFalse()
        {
            // Arrange
            string email = string.Empty;

            // Act
            var result = passwordResetValidator.ValidateEmail(email);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateEmail_WithEmptyEmail_ReturnsRequiredMessage()
        {
            // Arrange
            string email = string.Empty;

            // Act
            var result = passwordResetValidator.ValidateEmail(email);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Email address is required."));
        }

        [Test]
        public void ValidateEmail_WithNullEmail_ReturnsFalse()
        {
            // Arrange
            string email = null;

            // Act
            var result = passwordResetValidator.ValidateEmail(email);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateEmail_WithNullEmail_ReturnsRequiredMessage()
        {
            // Arrange
            string email = null;

            // Act
            var result = passwordResetValidator.ValidateEmail(email);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Email address is required."));
        }

        [Test]
        public void ValidateEmail_WithWhitespaceEmail_ReturnsFalse()
        {
            // Arrange
            string email = "   ";

            // Act
            var result = passwordResetValidator.ValidateEmail(email);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateEmail_WithWhitespaceEmail_ReturnsRequiredMessage()
        {
            // Arrange
            string email = "   ";

            // Act
            var result = passwordResetValidator.ValidateEmail(email);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Email address is required."));
        }

        [TestCase("test")]
        [TestCase("test@")]
        [TestCase("@example")]
        [TestCase("test@example")]
        [TestCase("test@@example.com")]
        [TestCase(".test@example.com")]
        public void ValidateEmail_WithInvalidEmail_ReturnsFalse(string email)
        {
            // Act
            var result = passwordResetValidator.ValidateEmail(email);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [TestCase("test")]
        [TestCase("test@")]
        [TestCase("@example")]
        [TestCase("test@example")]
        [TestCase("test@@example.com")]
        [TestCase(".test@example.com")]
        public void ValidateEmail_WithInvalidEmail_ReturnsInvalidFormatMessage(string email)
        {
            // Act
            var result = passwordResetValidator.ValidateEmail(email);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Invalid email format."));
        }

        [TestCase("test@example.com")]
        [TestCase("user.name@domain.co.uk")]
        [TestCase("firstname.lastname@example.org")]
        [TestCase("user123@example.io")]
        [TestCase("user+tag@example.com")]
        public void ValidateEmail_WithValidEmails_ReturnsTrue(string email)
        {
            // Act
            var result = passwordResetValidator.ValidateEmail(email);

            // Assert
            Assert.That(result.isValid, Is.True);
        }

        [TestCase("test@example.com")]
        [TestCase("user.name@domain.co.uk")]
        [TestCase("firstname.lastname@example.org")]
        [TestCase("user123@example.io")]
        [TestCase("user+tag@example.com")]
        public void ValidateEmail_WithValidEmails_ReturnsEmptyMessage(string email)
        {
            // Act
            var result = passwordResetValidator.ValidateEmail(email);

            // Assert
            Assert.That(result.errorMessage, Is.Empty);
        }

        #endregion

        #region ValidateResetCode Tests

        [Test]
        public void ValidateResetCode_WithValidCode_ReturnsTrue()
        {
            // Arrange
            string code = "123456";

            // Act
            var result = passwordResetValidator.ValidateResetCode(code);

            // Assert
            Assert.That(result.isValid, Is.True);
        }

        [Test]
        public void ValidateResetCode_WithValidCode_ReturnsEmptyMessage()
        {
            // Arrange
            string code = "123456";

            // Act
            var result = passwordResetValidator.ValidateResetCode(code);

            // Assert
            Assert.That(result.errorMessage, Is.Empty);
        }

        [Test]
        public void ValidateResetCode_WithEmptyCode_ReturnsFalse()
        {
            // Arrange
            string code = string.Empty;

            // Act
            var result = passwordResetValidator.ValidateResetCode(code);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateResetCode_WithEmptyCode_ReturnsRequiredMessage()
        {
            // Arrange
            string code = string.Empty;

            // Act
            var result = passwordResetValidator.ValidateResetCode(code);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Reset code is required."));
        }

        [Test]
        public void ValidateResetCode_WithNullCode_ReturnsFalse()
        {
            // Arrange
            string code = null;

            // Act
            var result = passwordResetValidator.ValidateResetCode(code);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateResetCode_WithNullCode_ReturnsRequiredMessage()
        {
            // Arrange
            string code = null;

            // Act
            var result = passwordResetValidator.ValidateResetCode(code);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Reset code is required."));
        }

        [Test]
        public void ValidateResetCode_WithWhitespaceCode_ReturnsFalse()
        {
            // Arrange
            string code = "   ";

            // Act
            var result = passwordResetValidator.ValidateResetCode(code);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidateResetCode_WithWhitespaceCode_ReturnsRequiredMessage()
        {
            // Arrange
            string code = "   ";

            // Act
            var result = passwordResetValidator.ValidateResetCode(code);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Reset code is required."));
        }

        [TestCase("12345")]
        [TestCase("1234567")]
        [TestCase("abcdef")]
        [TestCase("abc123")]
        [TestCase("12 345")]
        public void ValidateResetCode_WithInvalidCode_ReturnsFalse(string code)
        {
            // Act
            var result = passwordResetValidator.ValidateResetCode(code);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [TestCase("12345")]
        [TestCase("1234567")]
        [TestCase("abcdef")]
        [TestCase("abc123")]
        [TestCase("12 345")]
        public void ValidateResetCode_WithInvalidCode_Returns6DigitMessage(string code)
        {
            // Act
            var result = passwordResetValidator.ValidateResetCode(code);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Reset code must be a 6-digit number."));
        }

        [TestCase("123456")]
        [TestCase("654321")]
        [TestCase("000000")]
        [TestCase("999999")]
        public void ValidateResetCode_WithValidCodes_ReturnsTrue(string code)
        {
            // Act
            var result = passwordResetValidator.ValidateResetCode(code);

            // Assert
            Assert.That(result.isValid, Is.True);
        }

        [TestCase("123456")]
        [TestCase("654321")]
        [TestCase("000000")]
        [TestCase("999999")]
        public void ValidateResetCode_WithValidCodes_ReturnsEmptyMessage(string code)
        {
            // Act
            var result = passwordResetValidator.ValidateResetCode(code);

            // Assert
            Assert.That(result.errorMessage, Is.Empty);
        }

        #endregion

        #region ValidatePassword Tests

        [Test]
        public void ValidatePassword_WithValidPassword_ReturnsTrue()
        {
            // Arrange
            string password = "Password123!";

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.isValid, Is.True);
        }

        [Test]
        public void ValidatePassword_WithValidPassword_ReturnsEmptyMessage()
        {
            // Arrange
            string password = "Password123!";

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.errorMessage, Is.Empty);
        }

        [Test]
        public void ValidatePassword_WithEmptyPassword_ReturnsFalse()
        {
            // Arrange
            string password = string.Empty;

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidatePassword_WithEmptyPassword_ReturnsRequiredMessage()
        {
            // Arrange
            string password = string.Empty;

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("New password is required."));
        }

        [Test]
        public void ValidatePassword_WithNullPassword_ReturnsFalse()
        {
            // Arrange
            string password = null;

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidatePassword_WithNullPassword_ReturnsRequiredMessage()
        {
            // Arrange
            string password = null;

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("New password is required."));
        }

        [Test]
        public void ValidatePassword_WithWhitespacePassword_ReturnsFalse()
        {
            // Arrange
            string password = "   ";

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidatePassword_WithWhitespacePassword_ReturnsRequiredMessage()
        {
            // Arrange
            string password = "   ";

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("New password is required."));
        }

        [Test]
        public void ValidatePassword_WithShortPassword_ReturnsFalse()
        {
            // Arrange
            string password = "Pass1!"; // 6 characters

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidatePassword_WithShortPassword_ReturnsLengthMessage()
        {
            // Arrange
            string password = "Pass1!"; // 6 characters

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Password must be at least 8 characters long."));
        }

        [Test]
        public void ValidatePassword_WithNoUppercase_ReturnsFalse()
        {
            // Arrange
            string password = "password123!";

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidatePassword_WithNoUppercase_ReturnsUppercaseMessage()
        {
            // Arrange
            string password = "password123!";

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Password must contain at least one uppercase letter."));
        }

        [Test]
        public void ValidatePassword_WithNoLowercase_ReturnsFalse()
        {
            // Arrange
            string password = "PASSWORD123!";

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidatePassword_WithNoLowercase_ReturnsLowercaseMessage()
        {
            // Arrange
            string password = "PASSWORD123!";

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Password must contain at least one lowercase letter."));
        }

        [Test]
        public void ValidatePassword_WithNoDigit_ReturnsFalse()
        {
            // Arrange
            string password = "Password!";

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidatePassword_WithNoDigit_ReturnsDigitMessage()
        {
            // Arrange
            string password = "Password!";

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Password must contain at least one digit."));
        }

        [Test]
        public void ValidatePassword_WithNoSpecialChar_ReturnsFalse()
        {
            // Arrange
            string password = "Password123";

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ValidatePassword_WithNoSpecialChar_ReturnsSpecialCharMessage()
        {
            // Arrange
            string password = "Password123";

            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.errorMessage, Is.EqualTo("Password must contain at least one special character."));
        }

        [TestCase("Password123!")]
        [TestCase("Secure@Pass123")]
        [TestCase("Complex$Password456")]
        [TestCase("P@55w0rd")]
        [TestCase("MyVeryL0ng&C0mpl3xP@ssw0rd")]
        public void ValidatePassword_WithValidPasswords_ReturnsTrue(string password)
        {
            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.isValid, Is.True);
        }

        [TestCase("Password123!")]
        [TestCase("Secure@Pass123")]
        [TestCase("Complex$Password456")]
        [TestCase("P@55w0rd")]
        [TestCase("MyVeryL0ng&C0mpl3xP@ssw0rd")]
        public void ValidatePassword_WithValidPasswords_ReturnsEmptyMessage(string password)
        {
            // Act
            var result = passwordResetValidator.ValidatePassword(password);

            // Assert
            Assert.That(result.errorMessage, Is.Empty);
        }

        #endregion
    }
}