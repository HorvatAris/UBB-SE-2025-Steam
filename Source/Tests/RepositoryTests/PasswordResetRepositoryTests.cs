using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;
using BusinessLayer.Data;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Moq;
using NUnit.Framework;

namespace Tests.RepositoryTests
{
    /*
    [TestFixture]
    public class PasswordResetRepositoryTests
    {
        private Mock<IDataLink> mockDataLink;
        private IPasswordResetRepository passwordResetRepository;

        [SetUp]
        public void Setup()
        {
            this.mockDataLink = new Mock<IDataLink>();
            this.passwordResetRepository = new PasswordResetRepository(this.mockDataLink.Object);
        }

        [Test]
        public void Constructor_WithNullDataLink_ThrowsArgumentNullException()
        {
            // Act & Assert
            try
            {
                new PasswordResetRepository(null);
                Assert.Fail("Expected ArgumentNullException was not thrown");
            }
            catch (ArgumentNullException)
            {
                // Expected
                Assert.Pass();
            }
        }

        [Test]
        public void StoreResetCode_DeleteAndInsertWasCalled()
        {
            // Arrange
            int userId = 1;
            string code = "123456";
            DateTime expiryTime = DateTime.Now.AddMinutes(30);

            // Setup for DELETE SQL command
            this.mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(
                It.Is<string>(sql => sql.Contains("DELETE FROM PasswordResetCodes")),
                It.Is<SqlParameter[]>(parameters => parameters.Length == 1 && (int)parameters[0].Value == userId)))
                .Verifiable();

            // Setup for INSERT SQL command
            this.mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(
                It.Is<string>(sql => sql.Contains("INSERT INTO PasswordResetCodes")),
                It.Is<SqlParameter[]>(parameters => parameters.Length == 3)))
                .Verifiable();

            // Act
            this.passwordResetRepository.StoreResetCode(userId, code, expiryTime);

            // Assert
            this.mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(
                It.Is<string>(sql => sql.Contains("DELETE FROM PasswordResetCodes")),
                It.IsAny<SqlParameter[]>()), Times.Once);

            this.mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(
                It.Is<string>(sql => sql.Contains("INSERT INTO PasswordResetCodes")),
                It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void StoreResetCode_WhenDatabaseOperationExceptionOccurs_ThrowsRepositoryException()
        {
            // Arrange
            int userId = 1;
            string code = "123456";
            DateTime expiryTime = DateTime.Now.AddMinutes(30);

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(
                It.Is<string>(sql => sql.Contains("DELETE FROM PasswordResetCodes")),
                It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            try
            {
                this.passwordResetRepository.StoreResetCode(userId, code, expiryTime);
                Assert.Fail("Expected RepositoryException was not thrown");
            }
            catch (RepositoryException)
            {
                // Expected
                Assert.Pass();
            }
        }

        [Test]
        public void VerifyResetCode_WithValidCode_ReturnsTrue()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            var dataTable = new DataTable();
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("reset_code", typeof(string));
            dataTable.Columns.Add("expiration_time", typeof(DateTime));
            dataTable.Columns.Add("used", typeof(bool));
            dataTable.Rows.Add(1, code, DateTime.UtcNow.AddMinutes(10), false); // Valid code

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.Is<string>(sql => sql.Contains("SELECT")),
                It.Is<SqlParameter[]>(parameters =>
                    parameters.Length == 2 &&
                    (string)parameters[0].Value == email &&
                    (string)parameters[1].Value == code)))
                .Returns(dataTable);

            // Act
            bool result = this.passwordResetRepository.VerifyResetCode(email, code);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void VerifyResetCode_WithExpiredCode_ReturnsFalse()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            var dataTable = new DataTable();
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("reset_code", typeof(string));
            dataTable.Columns.Add("expiration_time", typeof(DateTime));
            dataTable.Columns.Add("used", typeof(bool));
            dataTable.Rows.Add(1, code, DateTime.UtcNow.AddMinutes(-10), false); // Expired code

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.Is<string>(sql => sql.Contains("SELECT")),
                It.Is<SqlParameter[]>(parameters => parameters.Length == 2)))
                .Returns(dataTable);

            // Act
            bool result = this.passwordResetRepository.VerifyResetCode(email, code);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void VerifyResetCode_WithUsedCode_ReturnsFalse()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            var dataTable = new DataTable();
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("reset_code", typeof(string));
            dataTable.Columns.Add("expiration_time", typeof(DateTime));
            dataTable.Columns.Add("used", typeof(bool));
            dataTable.Rows.Add(1, code, DateTime.UtcNow.AddMinutes(10), true); // Used code

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.Is<string>(sql => sql.Contains("SELECT")),
                It.Is<SqlParameter[]>(parameters => parameters.Length == 2)))
                .Returns(dataTable);

            // Act
            bool result = this.passwordResetRepository.VerifyResetCode(email, code);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void VerifyResetCode_WithNoCode_ReturnsFalse()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            var dataTable = new DataTable();
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("reset_code", typeof(string));
            dataTable.Columns.Add("expiration_time", typeof(DateTime));
            dataTable.Columns.Add("used", typeof(bool));

            // No rows added
            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.Is<string>(sql => sql.Contains("SELECT")),
                It.Is<SqlParameter[]>(parameters => parameters.Length == 2)))
                .Returns(dataTable);

            // Act
            bool result = this.passwordResetRepository.VerifyResetCode(email, code);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void VerifyResetCode_WhenDatabaseOperationExceptionOccurs_ThrowsRepositoryException()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters => parameters.Length == 2)))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            try
            {
                this.passwordResetRepository.VerifyResetCode(email, code);
                Assert.Fail("Expected RepositoryException was not thrown");
            }
            catch (RepositoryException)
            {
                // Expected
                Assert.Pass();
            }
        }

        [Test]
        public void ResetPassword_WithValidCodeAndEmail_ReturnsTrue()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string hashedPassword = "HashedPassword123!";
            var dataTable = new DataTable();
            dataTable.Columns.Add("Result", typeof(bool));
            dataTable.Rows.Add(true); // Password reset successful

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.Is<string>(sql => sql.Contains("BEGIN TRANSACTION")),
                It.Is<SqlParameter[]>(parameters =>
                    parameters.Length == 3 &&
                    (string)parameters[0].Value == email &&
                    (string)parameters[1].Value == code &&
                    (string)parameters[2].Value == hashedPassword)))
                .Returns(dataTable);

            // Act
            bool result = this.passwordResetRepository.ResetPassword(email, code, hashedPassword);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void ResetPassword_WithInvalidCodeOrEmail_ReturnsFalse()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string hashedPassword = "HashedPassword123!";
            var dataTable = new DataTable();
            dataTable.Columns.Add("Result", typeof(bool));
            dataTable.Rows.Add(false); // Password reset failed

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.Is<string>(sql => sql.Contains("BEGIN TRANSACTION")),
                It.Is<SqlParameter[]>(parameters => parameters.Length == 3)))
                .Returns(dataTable);

            // Act
            bool result = this.passwordResetRepository.ResetPassword(email, code, hashedPassword);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ResetPassword_WithEmptyResult_ReturnsFalse()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string hashedPassword = "HashedPassword123!";
            var dataTable = new DataTable();
            dataTable.Columns.Add("Result", typeof(bool));

            // No rows added
            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.Is<string>(sql => sql.Contains("BEGIN TRANSACTION")),
                It.Is<SqlParameter[]>(parameters => parameters.Length == 3)))
                .Returns(dataTable);

            // Act
            bool result = this.passwordResetRepository.ResetPassword(email, code, hashedPassword);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ResetPassword_WhenDatabaseOperationExceptionOccurs_ThrowsRepositoryException()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string hashedPassword = "HashedPassword123!";

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters => parameters.Length == 3)))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            try
            {
                this.passwordResetRepository.ResetPassword(email, code, hashedPassword);
                Assert.Fail("Expected RepositoryException was not thrown");
            }
            catch (RepositoryException)
            {
                // Expected
                Assert.Pass();
            }
        }

        [Test]
        public void ValidateResetCode_WithValidCode_ReturnsTrue()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            var dataTable = new DataTable();
            dataTable.Columns.Add("isValid", typeof(bool));
            dataTable.Rows.Add(true); // Valid code

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.Is<string>(sql => sql.Contains("DECLARE @isValid BIT")),
                It.Is<SqlParameter[]>(parameters =>
                    parameters.Length == 2 &&
                    (string)parameters[0].Value == email &&
                    (string)parameters[1].Value == code)))
                .Returns(dataTable);

            // Act
            bool result = this.passwordResetRepository.ValidateResetCode(email, code);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void ValidateResetCode_WithInvalidCode_ReturnsFalse()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            var dataTable = new DataTable();
            dataTable.Columns.Add("isValid", typeof(bool));
            dataTable.Rows.Add(false); // Invalid code

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.Is<string>(sql => sql.Contains("DECLARE @isValid BIT")),
                It.Is<SqlParameter[]>(parameters => parameters.Length == 2)))
                .Returns(dataTable);

            // Act
            bool result = this.passwordResetRepository.ValidateResetCode(email, code);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ValidateResetCode_WhenDatabaseOperationExceptionOccurs_ThrowsRepositoryException()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";

            this.mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters => parameters.Length == 2)))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            try
            {
                this.passwordResetRepository.ValidateResetCode(email, code);
                Assert.Fail("Expected RepositoryException was not thrown");
            }
            catch (RepositoryException)
            {
                // Expected
                Assert.Pass();
            }
        }

        [Test]
        public void CleanupExpiredCodes_ExecutesCorrectSqlCommand()
        {
            // Arrange
            this.mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(
                It.Is<string>(sql => sql.Contains("DELETE FROM PasswordResetCodes")), null))
                .Verifiable();

            // Act
            this.passwordResetRepository.CleanupExpiredCodes();

            // Assert
            this.mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(
                It.Is<string>(sql => sql.Contains("DELETE FROM PasswordResetCodes")),
                It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void CleanupExpiredCodes_WhenDatabaseOperationExceptionOccurs_ThrowsRepositoryException()
        {
            // Arrange
            this.mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            try
            {
                this.passwordResetRepository.CleanupExpiredCodes();
                Assert.Fail("Expected RepositoryException was not thrown");
            }
            catch (RepositoryException)
            {
                // Expected
                Assert.Pass();
            }
        }
    }
    */
}