using NUnit.Framework;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Tests
{
    [TestFixture]
    public class ExceptionTests
    {
        // DatabaseConnectionException Tests
        [Test]
        public void DatabaseConnectionException_ShouldStoreMessage()
        {
            // Arrange
            var message = "Test database connection error";

            // Act
            var exception = new DatabaseConnectionException(message);

            // Assert
            Assert.That(exception.Message, Is.EqualTo(message));
        }

        [Test]
        public void DatabaseConnectionException_ShouldStoreInnerException()
        {
            // Arrange
            var message = "Test database connection error";
            var innerException = new Exception("Inner error");

            // Act
            var exception = new DatabaseConnectionException(message, innerException);

            // Assert
            Assert.That(exception.Message, Is.EqualTo(message));
        }

        // DatabaseOperationException Tests
        [Test]
        public void DatabaseOperationException_ShouldStoreMessage()
        {
            // Arrange
            var message = "Test database operation error";

            // Act
            var exception = new DatabaseOperationException(message);

            // Assert
            Assert.That(exception.Message, Is.EqualTo(message));
        }

        [Test]
        public void DatabaseOperationException_ShouldStoreInnerException()
        {
            // Arrange
            var message = "Test database operation error";
            var innerException = new Exception("Inner error");

            // Act
            var exception = new DatabaseOperationException(message, innerException);

            // Assert
            Assert.That(exception.Message, Is.EqualTo(message));
        }

        // ServiceException Tests
        [Test]
        public void ServiceException_ShouldStoreMessage()
        {
            // Arrange
            var message = "Test service error";

            // Act
            var exception = new ServiceException(message);

            // Assert
            Assert.That(exception.Message, Is.EqualTo(message));
        }

        [Test]
        public void ServiceException_ShouldStoreInnerException()
        {
            // Arrange
            var message = "Test service error";
            var innerException = new Exception("Inner error");

            // Act
            var exception = new ServiceException(message, innerException);

            // Assert
            Assert.That(exception.Message, Is.EqualTo(message));
        }

        // RepositoryException Tests
        [Test]
        public void RepositoryException_ShouldStoreMessage()
        {
            // Arrange
            var message = "Test repository error";

            // Act
            var exception = new RepositoryException(message);

            // Assert
            Assert.That(exception.Message, Is.EqualTo(message));
        }

        [Test]
        public void RepositoryException_ShouldStoreInnerException()
        {
            // Arrange
            var message = "Test repository error";
            var innerException = new Exception("Inner error");

            // Act
            var exception = new RepositoryException(message, innerException);

            // Assert
            Assert.That(exception.Message, Is.EqualTo(message));
        }

        // EmailAlreadyExistsException Tests
        [Test]
        public void EmailAlreadyExistsException_ShouldContainCorrectMessage()
        {
            // Arrange
            var email = "test@example.com";
            var expectedMessage = $"An account with the email '{email}' already exists.";

            // Act
            var exception = new EmailAlreadyExistsException(email);

            // Assert
            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        // UsernameAlreadyTakenException Tests
        [Test]
        public void UsernameAlreadyTakenException_ShouldContainCorrectMessage()
        {
            // Arrange
            var username = "testuser";
            var expectedMessage = $"The username '{username}' is already taken.";

            // Act
            var exception = new UsernameAlreadyTakenException(username);

            // Assert
            Assert.That(exception.Message, Is.EqualTo(expectedMessage));
        }

        // UserValidationException Tests
        [Test]
        public void UserValidationException_ShouldStoreMessage()
        {
            // Arrange
            var message = "Test user validation error";

            // Act
            var exception = new UserValidationException(message);

            // Assert
            Assert.That(exception.Message, Is.EqualTo(message));
        }
    }
}
