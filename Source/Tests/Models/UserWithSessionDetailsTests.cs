using BusinessLayer.Models;

namespace Tests.Models
{
    [TestFixture]
    internal class UserWithSessionDetailsTests
    {
        [Test]
        public void SessionId_ShouldBeSetAndRetrievedCorrectly()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var userWithSessionDetails = new UserWithSessionDetails();

            // Act
            userWithSessionDetails.SessionId = sessionId;

            // Assert
            Assert.That(userWithSessionDetails.SessionId, Is.EqualTo(sessionId));
        }

        [Test]
        public void CreatedAt_ShouldBeSetAndRetrievedCorrectly()
        {
            // Arrange
            var createdAt = DateTime.Now;
            var userWithSessionDetails = new UserWithSessionDetails();

            // Act
            userWithSessionDetails.CreatedAt = createdAt;

            // Assert
            Assert.That(userWithSessionDetails.CreatedAt, Is.EqualTo(createdAt));
        }

        [Test]
        public void ExpiresAt_ShouldBeSetAndRetrievedCorrectly()
        {
            // Arrange
            var expiresAt = DateTime.Now.AddHours(1);
            var userWithSessionDetails = new UserWithSessionDetails();

            // Act
            userWithSessionDetails.ExpiresAt = expiresAt;

            // Assert
            Assert.That(userWithSessionDetails.ExpiresAt, Is.EqualTo(expiresAt));
        }

        [Test]
        public void UserId_ShouldBeSetAndRetrievedCorrectly()
        {
            // Arrange
            var userId = 1;
            var userWithSessionDetails = new UserWithSessionDetails();

            // Act
            userWithSessionDetails.UserId = userId;

            // Assert
            Assert.That(userWithSessionDetails.UserId, Is.EqualTo(userId));
        }

        [Test]
        public void Username_ShouldBeSetAndRetrievedCorrectly()
        {
            // Arrange
            var username = "TestUser";
            var userWithSessionDetails = new UserWithSessionDetails();

            // Act
            userWithSessionDetails.Username = username;

            // Assert
            Assert.That(userWithSessionDetails.Username, Is.EqualTo(username));
        }

        [Test]
        public void Email_ShouldBeSetAndRetrievedCorrectly()
        {
            // Arrange
            var email = "testuser@example.com";
            var userWithSessionDetails = new UserWithSessionDetails();

            // Act
            userWithSessionDetails.Email = email;

            // Assert
            Assert.That(userWithSessionDetails.Email, Is.EqualTo(email));
        }

        [Test]
        public void Developer_ShouldBeSetAndRetrievedCorrectly()
        {
            // Arrange
            var developer = true;
            var userWithSessionDetails = new UserWithSessionDetails();

            // Act
            userWithSessionDetails.Developer = developer;

            // Assert
            Assert.That(userWithSessionDetails.Developer, Is.EqualTo(developer));
        }

        [Test]
        public void UserCreatedAt_ShouldBeSetAndRetrievedCorrectly()
        {
            // Arrange
            var userCreatedAt = DateTime.Now.AddYears(-1);
            var userWithSessionDetails = new UserWithSessionDetails();

            // Act
            userWithSessionDetails.UserCreatedAt = userCreatedAt;

            // Assert
            Assert.That(userWithSessionDetails.UserCreatedAt, Is.EqualTo(userCreatedAt));
        }

        [Test]
        public void LastLogin_ShouldBeSetAndRetrievedCorrectly()
        {
            // Arrange
            var lastLogin = DateTime.Now.AddDays(-1);
            var userWithSessionDetails = new UserWithSessionDetails();

            // Act
            userWithSessionDetails.LastLogin = lastLogin;

            // Assert
            Assert.That(userWithSessionDetails.LastLogin, Is.EqualTo(lastLogin));
        }

        [Test]
        public void LastLogin_ShouldAllowNull()
        {
            // Arrange
            var userWithSessionDetails = new UserWithSessionDetails();

            // Act
            userWithSessionDetails.LastLogin = null;

            // Assert
            Assert.That(userWithSessionDetails.LastLogin, Is.Null);
        }
    }
}
