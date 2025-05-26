using BusinessLayer.Models;

namespace Tests.Models
{
    [TestFixture]
    internal class UserSessionModelTests
    {
        [Test]
        public void Instantce_ShouldReturnSameInstance()
        {
            // Act
            var instance1 = UserSession.Instance;
            var instance2 = UserSession.Instance;

            // Assert
            Assert.That(instance1, Is.SameAs(instance2));
        }

        [Test]
        public void UpdateSession_ShouldSetCurrentSessionId()
        {
            // Arrange
            var session = UserSession.Instance;
            var sessionId = Guid.NewGuid();

            // Act
            session.UpdateSession(sessionId, 1, DateTime.Now, DateTime.Now.AddHours(1));

            // Assert
            Assert.That(session.CurrentSessionId, Is.EqualTo(sessionId));
        }

        [Test]
        public void UpdateSession_ShouldSetUserId()
        {
            // Arrange
            var session = UserSession.Instance;
            var userId = 123;

            // Act
            session.UpdateSession(Guid.NewGuid(), userId, DateTime.Now, DateTime.Now.AddHours(1));

            // Assert
            Assert.That(session.UserId, Is.EqualTo(userId));
        }

        [Test]
        public void UpdateSession_ShouldSetCreatedAt()
        {
            // Arrange
            var session = UserSession.Instance;
            var createdAt = DateTime.Now;

            // Act
            session.UpdateSession(Guid.NewGuid(), 1, createdAt, createdAt.AddHours(1));

            // Assert
            Assert.That(session.CreatedAt, Is.EqualTo(createdAt));
        }

        [Test]
        public void UpdateSession_ShouldSetExpiresAt()
        {
            // Arrange
            var session = UserSession.Instance;
            var expiresAt = DateTime.Now.AddHours(1);

            // Act
            session.UpdateSession(Guid.NewGuid(), 1, DateTime.Now, expiresAt);

            // Assert
            Assert.That(session.ExpiresAt, Is.EqualTo(expiresAt));
        }

        [Test]
        public void ClearSession_ShouldResetCurrentSessionId()
        {
            // Arrange
            var session = UserSession.Instance;
            session.UpdateSession(Guid.NewGuid(), 1, DateTime.Now, DateTime.Now.AddHours(1));

            // Act
            session.ClearSession();

            // Assert
            Assert.That(session.CurrentSessionId, Is.Null);
        }

        [Test]
        public void ClearSession_ShouldResetUserId()
        {
            // Arrange
            var session = UserSession.Instance;
            session.UpdateSession(Guid.NewGuid(), 1, DateTime.Now, DateTime.Now.AddHours(1));

            // Act
            session.ClearSession();

            // Assert
            Assert.That(session.UserId, Is.EqualTo(0));
        }

        [Test]
        public void ClearSession_ShouldResetCreatedAt()
        {
            // Arrange
            var session = UserSession.Instance;
            session.UpdateSession(Guid.NewGuid(), 1, DateTime.Now, DateTime.Now.AddHours(1));

            // Act
            session.ClearSession();

            // Assert
            Assert.That(session.CreatedAt, Is.EqualTo(DateTime.MinValue));
        }

        [Test]
        public void ClearSession_ShouldResetExpiresAt()
        {
            // Arrange
            var session = UserSession.Instance;
            session.UpdateSession(Guid.NewGuid(), 1, DateTime.Now, DateTime.Now.AddHours(1));

            // Act
            session.ClearSession();

            // Assert
            Assert.That(session.ExpiresAt, Is.EqualTo(DateTime.MinValue));
        }

        [Test]
        public void IsSessionValid_ShouldReturnTrueForValidSession()
        {
            // Arrange
            var session = UserSession.Instance;
            session.UpdateSession(Guid.NewGuid(), 1, DateTime.Now, DateTime.Now.AddHours(1));

            // Act
            var isValid = session.IsSessionValid();

            // Assert
            Assert.That(isValid, Is.True);
        }

        [Test]
        public void IsSessionValid_ShouldReturnFalseForExpiredSession()
        {
            // Arrange
            var session = UserSession.Instance;
            session.UpdateSession(Guid.NewGuid(), 1, DateTime.Now.AddHours(-2), DateTime.Now.AddHours(-1));

            // Act
            var isValid = session.IsSessionValid();

            // Assert
            Assert.That(isValid, Is.False);
        }

        [Test]
        public void IsSessionValid_ShouldReturnFalseForNullSessionId()
        {
            // Arrange
            var session = UserSession.Instance;
            session.ClearSession();

            // Act
            var isValid = session.IsSessionValid();

            // Assert
            Assert.That(isValid, Is.False);
        }

        [Test]
        public void IsSessionValid_ShouldReturnFalseForZeroUserId()
        {
            // Arrange
            var session = UserSession.Instance;
            session.UpdateSession(Guid.NewGuid(), 0, DateTime.Now, DateTime.Now.AddHours(1));

            // Act
            var isValid = session.IsSessionValid();

            // Assert
            Assert.That(isValid, Is.False);
        }
    }
}