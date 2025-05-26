using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Services;
using BusinessLayer.Services.Fakes;
using BusinessLayer.Repositories.Fakes;
using BusinessLayer.Models;
using BusinessLayer.Exceptions;
using BusinessLayer.Utils;

namespace Tests.ServiceTests
{
    [TestFixture]
    internal class UserServiceTests
    {
        private UserService userService;

        [SetUp]
        public void Setup()
        {
            userService = new UserService(new FakeUsersRepository(), new FakeSessionService());
        }

        [Test]
        public void GetAllUsers_Called_ReturnsCorrectCount()
        {
            // Arrange
            // nothing

            // Act
            List<User> users = userService.GetAllUsers();

            // Assert
            Assert.That(users.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetUserById_Called_ReturnsCorrectUser()
        {
            // Arrange
            // nothing

            // Act
            User user = userService.GetUserByIdentifier(1);

            // Assert
            Assert.That(user.UserId, Is.EqualTo(1));
        }

        [Test]
        public void GetUserByEmail_Called_ReturnsCorrectUser()
        {
            // Arrange
            // nothing

            // Act
            User user = userService.GetUserByEmail("email");

            // Assert
            Assert.That(user.Email, Is.EqualTo("email"));
        }

        [Test]
        public void GetUseByUsername_Called_ReturnsCorrectUser()
        {
            // Arrange
            // nothing

            // Act
            User user = userService.GetUserByUsername("user");

            // Assert
            Assert.That(user.Username, Is.EqualTo("user"));
        }

        [Test]
        public void ValidateUserAndEmail_DuplicateEmail_ThrowsException()
        {
            // Arrange
            var email = "DupeEmail";
            var username = string.Empty;

            // Act & Assert
            var ex = Assert.Throws<EmailAlreadyExistsException>(() =>
            {
                userService.ValidateUserAndEmail(email, username);
            });

            Assert.That(ex.Message, Is.EqualTo("An account with the email '" + email + "' already exists."));
        }

        [Test]
        public void ValidateUserAndEmail_DuplicateUser_ThrowsException()
        {
            // Arrange
            var email = string.Empty;
            var username = "DupeUsername";

            // Act & Assert
            var ex = Assert.Throws<UsernameAlreadyTakenException>(() =>
            {
                userService.ValidateUserAndEmail(email, username);
            });

            Assert.That(ex.Message, Is.EqualTo($"The username '{username}' is already taken."));
        }

        [Test]
        public void ValidateUserAndEmail_EmptyEmailAndEmptyUsername_ThrowsException()
        {
            // Arrange
            var email = string.Empty;
            var username = string.Empty;

            // Act & Assert
            var ex = Assert.Throws<UserValidationException>(() =>
            {
                userService.ValidateUserAndEmail(email, username);
            });

            Assert.That(ex.Message, Is.EqualTo("Unknown validation error: OTHER_ERROR"));
        }

        [Test]
        public void CreateUser_CorrectCredentials_UserCreated()
        {
            // Arrange
            User user = new User();
            user.Email = "test@gmail.com";
            user.Username = "username";
            user.Password = "password";

            // Act
            User user1 = userService.CreateUser(user);

            // Assert
            Assert.That(user1.Username, Is.EqualTo("username"));
        }

        [Test]
        public void UpdateUser_CorrectUser_UserUpdated()
        {
            // Arrange
            User user = new User();
            user.Username = "test";

            // Act
            User user1 = userService.UpdateUser(user);

            // Assert
            Assert.That(user1.Username, Is.EqualTo("test"));
        }

        [Test]
        public void DeleteUser_CorrectUser_DoesntThrow()
        {
            // Arrange
            var user = new User();
            user.Username = "test";
            user.UserId = 1;

            // Act and Assert
            Assert.DoesNotThrow(() => userService.DeleteUser(user.UserId));
        }

        [Test]
        public void AcceptChanges_MatchingPasswords_ReturnsTrue()
        {
            // Arrange
            var user = new User();
            user.Username = "test";
            user.Password = "password";
            user.UserId = 1;

            // Act
            bool result = userService.AcceptChanges(user.UserId = 1, "password");

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void AcceptChanges_NotMatchingPasswords_ReturnsFalse()
        {
            // Arrange
            var user = new User();
            user.Username = "test";
            user.Password = "password";
            user.UserId = 1;

            // Act
            bool result = userService.AcceptChanges(user.UserId = 1, "notPassword");

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void UpdateUserEmail_CorrectUser_DoesntThrow()
        {
            // Arrange
            var user = new User();
            user.Username = "test";
            user.UserId = 1;
            user.Email = "test@gmail.com";

            // Act and Assert
            Assert.DoesNotThrow(() => userService.UpdateUserEmail(user.UserId, "test@yahoo.com"));
        }

        [Test]
        public void UpdateUserPassword_CorrectUser_DoesntThrow()
        {
            // Arrange
            var user = new User();
            user.Username = "test";
            user.UserId = 1;
            user.Password = "password";

            // Act and Assert
            Assert.DoesNotThrow(() => userService.UpdateUserPassword(user.UserId, PasswordHasher.HashPassword(user.Password)));
        }

        [Test]
        public void UpdateUserUsername_CorrectUser_DoesntThrow()
        {
            // Arrange
            var user = new User();
            user.Username = "test";
            user.UserId = 1;

            // Act and Assert
            Assert.DoesNotThrow(() => userService.UpdateUserUsername(user.UserId, "newTest"));
        }

        [Test]
        public void Login_UserNotNullAndGoodPassword_ReturnsUser()
        {
            // Arrange
            string mailOrUser = "test";
            string password = "password";

            // Act
            User user = userService.Login(mailOrUser, password);

            // Assert
            Assert.That(user, Is.Not.Null);
        }

        [Test]
        public void Login_UserNotNullAndBadPassword_ReturnsNull()
        {
            // Arrange
            string mailOrUser = "test";
            string password = "passWrong";

            // Act
            User user = userService.Login(mailOrUser, password);

            // Assert
            Assert.That(user, Is.Null);
        }

        [Test]
        public void Logout_Called_NoError()
        {
            // Arrange

            // Act and Assert
            Assert.DoesNotThrow(() => userService.Logout());
        }

        [Test]
        public void GetCurrentUser_Called_ReturnsUser()
        {
            // Arrange

            // Act
            User user = userService.GetCurrentUser();

            // Assert
            Assert.That(user, Is.Not.Null);
        }

        [Test]
        public void IsUserLoggedIn_Called_ReturnsTrue()
        {
            // Arrange

            // Act
            bool value = userService.IsUserLoggedIn();

            // Assert
            Assert.That(value, Is.True);
        }

        [Test]
        public void VerifyUserPassword_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            string password = "password";

            // Act
            bool value = userService.VerifyUserPassword(password);

            // Assert
            Assert.That(value, Is.True);
        }

        [Test]
        public void VerifyUserPassword_WrongPassword_ReturnsFalse()
        {
            // Arrange
            string password = "passWrong";

            // Act
            bool value = userService.VerifyUserPassword(password);

            // Assert
            Assert.That(value, Is.False);
        }

        [Test]
        public void UpdateUserUsername_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            string username = "username";
            string password = "password";

            // Act
            bool value = userService.UpdateUserUsername(username, password);

            // Assert
            Assert.That(value, Is.True);
        }

        [Test]
        public void UpdateUserUsername_WrongPassword_ReturnsFalse()
        {
            // Arrange
            string username = "username";
            string password = "passWrong";

            // Act
            bool value = userService.UpdateUserUsername(username, password);

            // Assert
            Assert.That(value, Is.False);
        }

        [Test]
        public void UpdateUserPassword_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            string newPassword = "test";
            string password = "password";

            // Act
            bool value = userService.UpdateUserPassword(newPassword, password);

            // Assert
            Assert.That(value, Is.True);
        }

        [Test]
        public void UpdateUserPassword_WrongPassword_ReturnsFalse()
        {
            // Arrange
            string newPassword = "test";
            string password = "passWrong";

            // Act
            bool value = userService.UpdateUserPassword(newPassword, password);

            // Assert
            Assert.That(value, Is.False);
        }

        [Test]
        public void UpdateUserEmail_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            string email = "test@gmail.com";
            string password = "password";

            // Act
            bool value = userService.UpdateUserEmail(email, password);

            // Assert
            Assert.That(value, Is.True);
        }

        [Test]
        public void UpdateUserEmail_WrongPassword_ReturnsFalse()
        {
            // Arrange
            string email = "test@gmail.com";
            string password = "passWrong";

            // Act
            bool value = userService.UpdateUserEmail(email, password);

            // Assert
            Assert.That(value, Is.False);
        }

        [Test]
        public void UserService_nullUserRepository_ThrowsException()
        {
            // Arange
            FakeSessionService mockService = new FakeSessionService();

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => new UserService(null, mockService));
        }

        [Test]
        public void UserService_nullSessionService_ThrowsException()
        {
            // Arange
            FakeUsersRepository mockUsersRepository = new FakeUsersRepository();

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => new UserService(mockUsersRepository, null));
        }
    }
}
