using System.Reflection;
using System;
using System.IO;
using System.Threading.Tasks;
using BusinessLayer.Repositories;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Models;
using BusinessLayer.Data;
using Moq;
using NUnit.Framework;

namespace Tests.ServiceTests
{
    /// <summary>
    /// Tests for the PasswordResetService class.
    /// </summary>
    [TestFixture]
    public class PasswordResetServiceTests
    {
        private PasswordResetService passwordResetService;
        private Mock<PasswordResetRepository> mockPasswordResetRepository;
        private Mock<IUserService> mockUserService;
        private string testResetCodesPath;

        [SetUp]
        public void Setup()
        {
            this.mockPasswordResetRepository = new Mock<PasswordResetRepository>(new Mock<IDataLink>().Object);
            this.mockUserService = new Mock<IUserService>();
            this.passwordResetService = new PasswordResetService(this.mockPasswordResetRepository.Object, this.mockUserService.Object);

            // Set up test directory for reset codes
            this.testResetCodesPath = Path.Combine(Path.GetTempPath(), "TestResetCodes");
            if (Directory.Exists(this.testResetCodesPath))
            {
                Directory.Delete(this.testResetCodesPath, true);
            }
            Directory.CreateDirectory(this.testResetCodesPath);

            // Use reflection to set the private resetCodesPath field
            typeof(PasswordResetService)
                .GetField("resetCodesPath", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(this.passwordResetService, this.testResetCodesPath);
        }

        [TearDown]
        public void Cleanup()
        {
            try
            {
                if (Directory.Exists(this.testResetCodesPath))
                {
                    Directory.Delete(this.testResetCodesPath, true);
                }
            }
            catch (IOException)
            {
                // Ignore directory deletion errors during test cleanup
            }
        }

        [Test]
        public async Task SendResetCode_WithValidEmail_ReturnsValidResult()
        {
            // Arrange
            string email = "test@example.com";
            var user = new User { UserId = 1, Email = email, Username = "testuser" };
            this.mockUserService.Setup(mockService => mockService.GetUserByEmail(email)).Returns(user);

            // Act
            var (isValid, message) = await this.passwordResetService.SendResetCode(email);

            // Assert
            Assert.That(isValid, Is.True);
        }

        [Test]
        public async Task SendResetCode_WithValidEmail_ReturnsSuccessMessage()
        {
            // Arrange
            string email = "test@example.com";
            var user = new User { UserId = 1, Email = email, Username = "testuser" };
            this.mockUserService.Setup(mockService => mockService.GetUserByEmail(email)).Returns(user);

            // Act
            var (isValid, message) = await this.passwordResetService.SendResetCode(email);

            // Assert
            Assert.That(message, Is.EqualTo("Reset code sent successfully."));
        }

        [Test]
        public async Task SendResetCode_WithValidEmail_CreatesResetCodeFile()
        {
            // Arrange
            string email = "test@example.com";
            var user = new User { UserId = 1, Email = email, Username = "testuser" };
            this.mockUserService.Setup(mockService => mockService.GetUserByEmail(email)).Returns(user);

            // Act
            await this.passwordResetService.SendResetCode(email);

            // Assert
            string filePath = Path.Combine(this.testResetCodesPath, $"{email.ToLower()}_reset_code.txt");
            Assert.That(File.Exists(filePath), Is.True);
        }

        [Test]
        public async Task SendResetCode_WithValidEmail_CreatesValidResetCode()
        {
            // Arrange
            string email = "test@example.com";
            var user = new User { UserId = 1, Email = email, Username = "testuser" };
            this.mockUserService.Setup(mockService => mockService.GetUserByEmail(email)).Returns(user);

            // Act
            await this.passwordResetService.SendResetCode(email);

            // Assert
            string filePath = Path.Combine(this.testResetCodesPath, $"{email.ToLower()}_reset_code.txt");
            string fileContent = File.ReadAllText(filePath);
            string code = fileContent.Split('|')[0];
            Assert.That(System.Text.RegularExpressions.Regex.IsMatch(code, @"^\d{6}$"), Is.True);
        }

        [Test]
        public async Task SendResetCode_UnregisteredEmail_ReturnsInvalidResult()
        {
            // Arrange
            string email = "nonexistent@example.com";
            this.mockUserService.Setup(mockService => mockService.GetUserByEmail(email)).Returns((User)null);

            // Act
            var result = await this.passwordResetService.SendResetCode(email);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public async Task SendResetCode_UnregisteredEmail_ReturnsNotRegisteredMessage()
        {
            // Arrange
            string email = "nonexistent@example.com";
            this.mockUserService.Setup(mockService => mockService.GetUserByEmail(email)).Returns((User)null);

            // Act
            var result = await this.passwordResetService.SendResetCode(email);

            // Assert
            Assert.That(result.message, Is.EqualTo("Email is not registered."));
        }

        [Test]
        public void VerifyResetCode_WithValidCode_ReturnsTrue()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string filePath = Path.Combine(this.testResetCodesPath, $"{email.ToLower()}_reset_code.txt");
            File.WriteAllText(filePath, $"{code}|{DateTime.UtcNow.AddMinutes(10):O}");

            // Act
            bool result = this.passwordResetService.VerifyResetCode(email, code).Item1;

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void VerifyResetCode_WithNonExistentCode_ReturnsFalse()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";

            // Act
            bool result = this.passwordResetService.VerifyResetCode(email, code).Item1;

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ResetPassword_WithValidCode_ResetsPassword()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string newPassword = "NewPassword123!";
            string filePath = Path.Combine(this.testResetCodesPath, $"{email.ToLower()}_reset_code.txt");
            File.WriteAllText(filePath, $"{code}|{DateTime.UtcNow.AddMinutes(10):O}");
            var user = new User { UserId = 1, Email = email, Username = "testuser" };
            this.mockUserService.Setup(mockService => mockService.GetUserByEmail(email)).Returns(user);
            this.mockUserService.Setup(mockService => mockService.UpdateUserPassword(user.UserId, newPassword));

            // Act
            var (isValid, message) = this.passwordResetService.ResetPassword(email, code, newPassword);

            // Assert
            Assert.That(isValid, Is.True);
        }

        [Test]
        public void ResetPassword_WithValidCode_ReturnsSuccessMessage()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string newPassword = "NewPassword123!";
            string filePath = Path.Combine(this.testResetCodesPath, $"{email.ToLower()}_reset_code.txt");
            File.WriteAllText(filePath, $"{code}|{DateTime.UtcNow.AddMinutes(10):O}");
            var user = new User { UserId = 1, Email = email, Username = "testuser" };
            this.mockUserService.Setup(mockService => mockService.GetUserByEmail(email)).Returns(user);
            this.mockUserService.Setup(mockService => mockService.UpdateUserPassword(user.UserId, newPassword));

            // Act
            var (isValid, message) = this.passwordResetService.ResetPassword(email, code, newPassword);

            // Assert
            Assert.That(message, Is.EqualTo("Password reset successfully."));
        }

        [Test]
        public void ResetPassword_WithInvalidCode_ReturnsInvalidResult()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string newPassword = "NewPassword123!";
            string filePath = Path.Combine(this.testResetCodesPath, $"{email.ToLower()}_reset_code.txt");
            File.WriteAllText(filePath, $"{code}|{DateTime.UtcNow.AddMinutes(-10):O}"); // Expired code

            // Act
            var result = this.passwordResetService.ResetPassword(email, code, newPassword);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ResetPassword_WithInvalidPassword_ReturnsInvalidResult()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string invalidPassword = "weak";
            string filePath = Path.Combine(this.testResetCodesPath, $"{email.ToLower()}_reset_code.txt");
            File.WriteAllText(filePath, $"{code}|{DateTime.UtcNow.AddMinutes(10):O}");
            var user = new User { UserId = 1, Email = email, Username = "testuser" };
            this.mockUserService.Setup(mockService => mockService.GetUserByEmail(email)).Returns(user);

            // Act
            var result = this.passwordResetService.ResetPassword(email, code, invalidPassword);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ResetPassword_WithNonExistentUser_ReturnsInvalidResult()
        {
            // Arrange
            string email = "nonexistent@example.com";
            string code = "123456";
            string newPassword = "NewPassword123!";
            string filePath = Path.Combine(this.testResetCodesPath, $"{email.ToLower()}_reset_code.txt");
            File.WriteAllText(filePath, $"{code}|{DateTime.UtcNow.AddMinutes(10):O}");
            this.mockUserService.Setup(mockService => mockService.GetUserByEmail(email)).Returns((User)null);

            // Act
            var result = this.passwordResetService.ResetPassword(email, code, newPassword);

            // Assert
            Assert.That(result.isValid, Is.False);
        }

        [Test]
        public void ResetPassword_WithNonExistentUser_ReturnsUserNotFoundMessage()
        {
            // Arrange
            string email = "nonexistent@example.com";
            string code = "123456";
            string newPassword = "NewPassword123!";
            string filePath = Path.Combine(this.testResetCodesPath, $"{email.ToLower()}_reset_code.txt");
            File.WriteAllText(filePath, $"{code}|{DateTime.UtcNow.AddMinutes(10):O}");
            this.mockUserService.Setup(mockService => mockService.GetUserByEmail(email)).Returns((User)null);

            // Act
            var result = this.passwordResetService.ResetPassword(email, code, newPassword);

            // Assert
            Assert.That(result.message, Is.EqualTo("User not found."));
        }

        [Test]
        public void CleanupExpiredCodes_PreservesNonExpiredFile()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string filePath = Path.Combine(this.testResetCodesPath, $"{email.ToLower()}_reset_code.txt");
            File.WriteAllText(filePath, $"{code}|{DateTime.UtcNow.AddMinutes(10):O}"); // Valid code

            // Act
            this.passwordResetService.CleanupExpiredCodes();

            // Assert
            Assert.That(File.Exists(filePath), Is.True);
        }

        [Test]
        public void CleanupExpiredCodes_HandlesNonExistentDirectory()
        {
            // Arrange
            Directory.Delete(this.testResetCodesPath, true);

            // Act & Assert
            Assert.DoesNotThrow(() => this.passwordResetService.CleanupExpiredCodes());
        }
    }
}