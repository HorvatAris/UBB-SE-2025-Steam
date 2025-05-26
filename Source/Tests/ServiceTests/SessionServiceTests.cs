using System;
using System.Collections.Generic;
using Moq;
using BusinessLayer.Models;
using BusinessLayer.Services;
using BusinessLayer.Repositories.Interfaces;

namespace Tests.ServiceTests
{
    [TestFixture]
    internal class SessionServiceTests
    {
        private Mock<ISessionRepository> mockSessionRepository;
        private Mock<IUsersRepository> mockUsersRepository;
        private SessionService sessionService;

        [SetUp]
        public void SetUp()
        {
            mockSessionRepository = new Mock<ISessionRepository>(MockBehavior.Strict);
            mockUsersRepository = new Mock<IUsersRepository>(MockBehavior.Strict);
            sessionService = new SessionService(mockSessionRepository.Object, mockUsersRepository.Object);

            // Clear the singleton session between tests
            UserSession.Instance.ClearSession();
        }

        [Test]
        public void Constructor_NullSessionRepository_ThrowsArgumentNullException()
        {
            // Arrange
            var usersRepository = new Mock<IUsersRepository>().Object;

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SessionService(null, usersRepository));

            // Assert
            Assert.That(exception.ParamName, Is.EqualTo("sessionRepository"));
        }

        [Test]
        public void Constructor_NullUsersRepository_ThrowsArgumentNullException()
        {
            // Arrange
            var sessionRepository = new Mock<ISessionRepository>().Object;

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SessionService(sessionRepository, null));

            // Assert
            Assert.That(exception.ParamName, Is.EqualTo("usersRepository"));
        }

        [Test]
        public void CreateNewSession_ShouldReturnNewSessionId()
        {
            // Arrange
            var user = new User { UserId = 1 };
            var sessionDetails = new SessionDetails
            {
                SessionId = Guid.NewGuid(),
                UserId = user.UserId,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            mockSessionRepository.Setup(repository => repository.DeleteUserSessions(user.UserId));
            mockSessionRepository.Setup(repository => repository.CreateSession(user.UserId)).Returns(sessionDetails);

            // Act
            var result = sessionService.CreateNewSession(user);

            // Assert
            Assert.That(result, Is.EqualTo(sessionDetails.SessionId));
            mockSessionRepository.Verify(repository => repository.DeleteUserSessions(user.UserId), Times.Once);
            mockSessionRepository.Verify(repository => repository.CreateSession(user.UserId), Times.Once);
        }

        [Test]
        public void EndSession_ShouldClearCurrentSession()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            UserSession.Instance.UpdateSession(sessionId, 1, DateTime.Now, DateTime.Now.AddHours(1));
            mockSessionRepository.Setup(repository => repository.DeleteSession(sessionId));

            // Act
            sessionService.EndSession();

            // Assert
            Assert.That(UserSession.Instance.CurrentSessionId, Is.Null);
            mockSessionRepository.Verify(repository => repository.DeleteSession(sessionId), Times.Once);
        }

        [Test]
        public void GetCurrentUser_ShouldReturnUserIfSessionIsValid()
        {
            // Arrange
            var userId = 1;
            var user = new User { UserId = userId };
            UserSession.Instance.UpdateSession(Guid.NewGuid(), userId, DateTime.Now, DateTime.Now.AddHours(1));
            mockUsersRepository.Setup(repository => repository.GetUserById(userId)).Returns(user);

            // Act
            var result = sessionService.GetCurrentUser();

            // Assert
            Assert.That(result, Is.EqualTo(user));
            mockUsersRepository.Verify(repository => repository.GetUserById(userId), Times.Once);
        }

        [Test]
        public void GetCurrentUser_WithExpiredSession_ShouldClearSessionAndReturnNull()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            // Set an expired session by setting ExpiresAt in the past
            UserSession.Instance.UpdateSession(sessionId, 1, DateTime.Now.AddHours(-2), DateTime.Now.AddHours(-1));
            mockSessionRepository.Setup(repository => repository.DeleteSession(sessionId));

            // Act
            var result = sessionService.GetCurrentUser();

            // Assert
            Assert.That(result, Is.Null);
            Assert.That(UserSession.Instance.CurrentSessionId, Is.Null);
            mockSessionRepository.Verify(repository => repository.DeleteSession(sessionId), Times.Once);
        }

        [Test]
        public void IsUserLoggedIn_ShouldReturnTrueIfSessionIsValid()
        {
            // Arrange
            UserSession.Instance.UpdateSession(Guid.NewGuid(), 1, DateTime.Now, DateTime.Now.AddHours(1));

            // Act
            var result = sessionService.IsUserLoggedIn();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void RestoreSessionFromDatabase_ShouldUpdateSessionIfValid()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var sessionDetails = new SessionDetails
            {
                SessionId = sessionId,
                UserId = 1,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            mockSessionRepository.Setup(repository => repository.GetSessionById(sessionId)).Returns(sessionDetails);

            // Act
            sessionService.RestoreSessionFromDatabase(sessionId);

            // Assert
            Assert.That(UserSession.Instance.CurrentSessionId, Is.EqualTo(sessionId));
            mockSessionRepository.Verify(repository => repository.GetSessionById(sessionId), Times.Once);
        }

        [Test]
        public void RestoreSessionFromDatabase_WithExpiredSession_ShouldDeleteSession()
        {
            // Arrange
            var sessionId = Guid.NewGuid();

            // Create sessionDetails with ExpiresAt in the past
            var sessionDetails = new SessionDetails
            {
                SessionId = sessionId,
                UserId = 1,
                CreatedAt = DateTime.Now.AddHours(-2),
                ExpiresAt = DateTime.Now.AddHours(-1)
            };

            mockSessionRepository.Setup(repository => repository.GetSessionById(sessionId)).Returns(sessionDetails);
            mockSessionRepository.Setup(repository => repository.DeleteSession(sessionId));

            // Act
            sessionService.RestoreSessionFromDatabase(sessionId);

            // Assert
            Assert.That(UserSession.Instance.CurrentSessionId, Is.Null);
            mockSessionRepository.Verify(repository => repository.GetSessionById(sessionId), Times.Once);
            mockSessionRepository.Verify(repository => repository.DeleteSession(sessionId), Times.Once);
        }

        [Test]
        public void CleanupExpiredSessions_ShouldDeleteExpiredSessions()
        {
            // Arrange
            var expiredSessionIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            mockSessionRepository.Setup(repository => repository.GetExpiredSessions()).Returns(expiredSessionIds);
            foreach (var sessionId in expiredSessionIds)
            {
                mockSessionRepository.Setup(repository => repository.DeleteSession(sessionId));
            }

            // Act
            sessionService.CleanupExpiredSessions();

            // Assert
            foreach (var sessionId in expiredSessionIds)
            {
                mockSessionRepository.Verify(repository => repository.DeleteSession(sessionId), Times.Once);
            }
        }

        [Test]
        public void CleanupExpiredSessions_WithInvalidCurrentSession_ShouldClearSession()
        {
            // Arrange
            // Create some expired sessions
            var expiredSessionIds = new List<Guid> { Guid.NewGuid() };
            mockSessionRepository.Setup(repository => repository.GetExpiredSessions()).Returns(expiredSessionIds);
            foreach (var sessionId in expiredSessionIds)
            {
                mockSessionRepository.Setup(repository => repository.DeleteSession(sessionId));
            }

            // Setup a current session that is expired (thus invalid)
            var currentSessionId = Guid.NewGuid();
            UserSession.Instance.UpdateSession(currentSessionId, 1, DateTime.Now.AddHours(-2), DateTime.Now.AddHours(-1));

            // Act
            sessionService.CleanupExpiredSessions();

            // Assert
            Assert.That(UserSession.Instance.CurrentSessionId, Is.Null);
        }
    }
}
