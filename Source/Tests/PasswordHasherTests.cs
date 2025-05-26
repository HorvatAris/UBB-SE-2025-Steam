using NUnit.Framework;
using BusinessLayer.Utils;

namespace Tests
{
    [TestFixture]
    public class PasswordHasherTests
    {
        [Test]
        public void HashPassword_ReturnsHashedValue()
        {
            // Arrange
            string password = "SecureP@ss123";

            // Act
            string hashedPassword = PasswordHasher.HashPassword(password);

            // Assert
            Assert.That(hashedPassword, Is.Not.Null.And.Not.Empty, "Hashed password should not be null or empty.");
            Assert.That(hashedPassword, Does.Not.Contain(password), "Hashed password should not contain the original password.");
        }

        [Test]
        public void VerifyPassword_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            string password = "MySecret123";
            string hashedPassword = PasswordHasher.HashPassword(password);

            // Act
            bool result = PasswordHasher.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.That(result, Is.True, "VerifyPassword should return true for correct password.");
        }

        [Test]
        public void VerifyPassword_IncorrectPassword_ReturnsFalse()
        {
            // Arrange
            string password = "CorrectPassword";
            string wrongPassword = "WrongPassword";
            string hashedPassword = PasswordHasher.HashPassword(password);

            // Act
            bool result = PasswordHasher.VerifyPassword(wrongPassword, hashedPassword);

            // Assert
            Assert.That(result, Is.False, "VerifyPassword should return false for incorrect password.");
        }

        [Test]
        public void HashPassword_SamePasswordDifferentHashes_ReturnsDifferentHashes()
        {
            // Arrange
            string password = "RepeatMe";

            // Act
            string hash1 = PasswordHasher.HashPassword(password);
            string hash2 = PasswordHasher.HashPassword(password);

            // Assert
            Assert.That(hash1, Is.Not.EqualTo(hash2), "Hashing the same password twice should return different hashes.");
        }

        [Test]
        public void VerifyPassword_EmptyPasswordAndHash_ReturnsFalse()
        {
            // Arrange
            string emptyPassword = string.Empty;
            string emptyHash = string.Empty;

            // Act
            bool result = PasswordHasher.VerifyPassword(emptyPassword, emptyHash);

            // Assert
            Assert.That(result, Is.False, "Verifying an empty password against an empty hash should return false.");
        }

        [Test]
        public void VerifyPassword_EmptyPasswordAgainstValidHash_ReturnsFalse()
        {
            // Arrange
            string validPassword = "NotEmpty";
            string validHash = PasswordHasher.HashPassword(validPassword);
            string emptyPassword = string.Empty;

            // Act
            bool result = PasswordHasher.VerifyPassword(emptyPassword, validHash);

            // Assert
            Assert.That(result, Is.False, "Empty password should not verify against a valid hash.");
        }
    }
}
