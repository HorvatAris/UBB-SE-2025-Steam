using System;
using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Repositories;
using BusinessLayer.Models;
using Moq;
using Microsoft.Data.SqlClient;
using BusinessLayer.Exceptions;
using NUnit.Framework;

namespace Tests.RepositoryTests
{
    /*
    [TestFixture]
    internal class SessionRepositoryTests
    {
        private Mock<IDataLink> mockDataLink;
        private SessionRepository sessionRepository;

        [SetUp]
        public void Setup()
        {
            mockDataLink = new Mock<IDataLink>();
            sessionRepository = new SessionRepository(mockDataLink.Object);
        }

        [Test]
        public void Constructor_NullDataLink_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SessionRepository(null));
        }

        [Test]
        public void CreateSession_ValidUserId_ReturnsSessionDetails()
        {
            // Arrange
            var userId = 1;
            var sessionId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            var expiresAt = createdAt.AddHours(1);

            var dataTable = new DataTable();
            dataTable.Columns.Add("session_id", typeof(Guid));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("created_at", typeof(DateTime));
            dataTable.Columns.Add("expires_at", typeof(DateTime));
            dataTable.Rows.Add(sessionId, userId, createdAt, expiresAt);

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = sessionRepository.CreateSession(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SessionId, Is.EqualTo(sessionId));
            Assert.That(result.UserId, Is.EqualTo(userId));
            Assert.That(result.CreatedAt, Is.EqualTo(createdAt));
            Assert.That(result.ExpiresAt, Is.EqualTo(expiresAt));
        }

        [Test]
        public void CreateSession_NoRows_ThrowsRepositoryException()
        {
            // Arrange
            var userId = 1;
            var dataTable = new DataTable();

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act & Assert
            Assert.Throws<RepositoryException>(() => sessionRepository.CreateSession(userId));
        }

        [Test]
        public void CreateSession_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var userId = 1;

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => sessionRepository.CreateSession(userId));
        }

        [Test]
        public void DeleteUserSessions_ValidUserId_ExecutesNonQuerySql()
        {
            // Arrange
            var userId = 1;

            // Act
            sessionRepository.DeleteUserSessions(userId);

            // Assert
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void DeleteUserSessions_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var userId = 1;

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => sessionRepository.DeleteUserSessions(userId));
        }

        [Test]
        public void DeleteSession_ValidSessionId_ExecutesNonQuerySql()
        {
            // Arrange
            var sessionId = Guid.NewGuid();

            // Act
            sessionRepository.DeleteSession(sessionId);

            // Assert
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void DeleteSession_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var sessionId = Guid.NewGuid();

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => sessionRepository.DeleteSession(sessionId));
        }

        [Test]
        public void GetSessionById_ValidSessionId_ReturnsSessionDetails()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var userId = 1;
            var createdAt = DateTime.Now;
            var expiresAt = createdAt.AddHours(1);

            var dataTable = new DataTable();
            dataTable.Columns.Add("session_id", typeof(Guid));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("created_at", typeof(DateTime));
            dataTable.Columns.Add("expires_at", typeof(DateTime));
            dataTable.Rows.Add(sessionId, userId, createdAt, expiresAt);

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = sessionRepository.GetSessionById(sessionId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SessionId, Is.EqualTo(sessionId));
            Assert.That(result.UserId, Is.EqualTo(userId));
            Assert.That(result.CreatedAt, Is.EqualTo(createdAt));
            Assert.That(result.ExpiresAt, Is.EqualTo(expiresAt));
        }

        [Test]
        public void GetSessionById_NoRows_ReturnsNull()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var dataTable = new DataTable();

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = sessionRepository.GetSessionById(sessionId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetSessionById_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var sessionId = Guid.NewGuid();

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => sessionRepository.GetSessionById(sessionId));
        }

        [Test]
        public void GetUserFromSession_ValidSessionId_ReturnsUserWithSessionDetails()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var userId = 1;
            var username = "testuser";
            var email = "test@example.com";
            var createdAt = DateTime.Now;
            var expiresAt = createdAt.AddHours(1);
            var lastLogin = DateTime.Now.AddDays(-1);

            var dataTable = new DataTable();
            dataTable.Columns.Add("session_id", typeof(Guid));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("username", typeof(string));
            dataTable.Columns.Add("email", typeof(string));
            dataTable.Columns.Add("developer", typeof(bool));
            dataTable.Columns.Add("created_at", typeof(DateTime));
            dataTable.Columns.Add("expires_at", typeof(DateTime));
            dataTable.Columns.Add("last_login", typeof(DateTime));
            dataTable.Rows.Add(sessionId, userId, username, email, true, createdAt, expiresAt, lastLogin);

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = sessionRepository.GetUserFromSession(sessionId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserId, Is.EqualTo(userId));
            Assert.That(result.Username, Is.EqualTo(username));
            Assert.That(result.Email, Is.EqualTo(email));
            Assert.That(result.Developer, Is.True);
            Assert.That(result.CreatedAt, Is.EqualTo(createdAt));
            Assert.That(result.ExpiresAt, Is.EqualTo(expiresAt));
            Assert.That(result.LastLogin, Is.EqualTo(lastLogin));
        }

        [Test]
        public void GetUserFromSession_LastLoginIsDbNull_SetsLastLoginToNull()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var userId = 1;
            var username = "testuser";
            var email = "test@example.com";
            var createdAt = DateTime.Now;
            var expiresAt = createdAt.AddHours(1);

            var dataTable = new DataTable();
            dataTable.Columns.Add("session_id", typeof(Guid));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("username", typeof(string));
            dataTable.Columns.Add("email", typeof(string));
            dataTable.Columns.Add("developer", typeof(bool));
            dataTable.Columns.Add("created_at", typeof(DateTime));
            dataTable.Columns.Add("expires_at", typeof(DateTime));
            dataTable.Columns.Add("last_login", typeof(DateTime));
            dataTable.Rows.Add(sessionId, userId, username, email, true, createdAt, expiresAt, DBNull.Value);

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = sessionRepository.GetUserFromSession(sessionId);

            // Assert
            Assert.That(result.LastLogin, Is.Null);
        }

        [Test]
        public void GetUserFromSession_NoRows_ReturnsNull()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var dataTable = new DataTable();

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = sessionRepository.GetUserFromSession(sessionId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetUserFromSession_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var sessionId = Guid.NewGuid();

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => sessionRepository.GetUserFromSession(sessionId));
        }

        [Test]
        public void GetExpiredSessions_ReturnsListOfSessionIds()
        {
            // Arrange
            var sessionId1 = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();

            var dataTable = new DataTable();
            dataTable.Columns.Add("session_id", typeof(Guid));
            dataTable.Rows.Add(sessionId1);
            dataTable.Rows.Add(sessionId2);

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = sessionRepository.GetExpiredSessions();

            // Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result, Does.Contain(sessionId1));
            Assert.That(result, Does.Contain(sessionId2));
        }

        [Test]
        public void GetExpiredSessions_NoRows_ReturnsEmptyList()
        {
            // Arrange
            var dataTable = new DataTable();

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = sessionRepository.GetExpiredSessions();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetExpiredSessions_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => sessionRepository.GetExpiredSessions());
        }

        [Test]
        public void CleanupExpiredSessions_ExecutesNonQuerySql()
        {
            // Arrange

            // Act
            sessionRepository.CleanupExpiredSessions();

            // Assert
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), null), Times.Once);
        }

        [Test]
        public void CleanupExpiredSessions_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink
                .Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), null))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => sessionRepository.CleanupExpiredSessions());
        }
    }
    */
}