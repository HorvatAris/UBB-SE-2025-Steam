using System;
using System.Collections.Generic;
using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Exceptions;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Moq;
using NUnit.Framework;

namespace Tests.RepositoryTests
{
    /*
    [TestFixture]
    internal class UserRepositoryTests
    {
        private UsersRepository usersRepository;
        private Mock<IDataLink> dataLinkMock;
        private DataTable dataTable;

        [SetUp]
        public void SetUp()
        {
            dataLinkMock = new Mock<IDataLink>();
            dataTable = new DataTable();
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("username", typeof(string));
            dataTable.Columns.Add("email", typeof(string));
            dataTable.Columns.Add("developer", typeof(bool));
            dataTable.Columns.Add("created_at", typeof(DateTime));
            dataTable.Columns.Add("last_login", typeof(DateTime));

            dataTable.Rows.Add(1, "user1", "user1@example.com", true, DateTime.Now, DateTime.Now.AddDays(-1));
            dataTable.Rows.Add(2, "user2", "user2@example.com", false, DateTime.Now, DBNull.Value);

            usersRepository = new UsersRepository(dataLinkMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            dataTable.Rows.Clear();
            dataTable.Dispose();
            dataTable = null!;
        }

        [Test]
        public void GetAllUsers_RetrievesUsersCorrectly_ReturnsUsers()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var users = usersRepository.GetAllUsers();

            // Assert
            Assert.That(users.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetAllUsers_SqlFails_ThrowsRepositoryException()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => usersRepository.GetAllUsers());
        }

        [Test]
        public void GetUserById_UserExists_ReturnsUser()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var user = usersRepository.GetUserById(1);

            // Assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user!.UserId, Is.EqualTo(1));
        }

        [Test]
        public void GetUserById_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var emptyTable = dataTable.Clone();
            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(emptyTable);

            // Act
            var user = usersRepository.GetUserById(999);

            // Assert
            Assert.That(user, Is.Null);
        }

        [Test]
        public void GetUserById_SqlException_ThrowsRepositoryException()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => usersRepository.GetUserById(10));
        }

        [Test]
        public void UpdateUser_CorrectUser_ReturnsUpdatedUser()
        {
            // Arrange
            var updatedUser = new User
            {
                UserId = 1,
                Username = "newname",
                Email = "new@example.com",
                Password = "irrelevant",
                IsDeveloper = true
            };
            var table = dataTable.Clone();
            table.Rows.Add(1, "newname", "new@example.com", true, DateTime.Now, DateTime.Now);

            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(table);

            // Act
            var result = usersRepository.UpdateUser(updatedUser);

            // Assert
            Assert.That(result.Username, Is.EqualTo("newname"));
            Assert.That(result.Email, Is.EqualTo("new@example.com"));
        }

        [Test]
        public void UpdateUser_NonExistentUser_ThrowsRepositoryException()
        {
            // Arrange
            var updatedUser = new User { UserId = 1, Username = "x", Email = "x", Password = "p", IsDeveloper = false };
            var emptyTable = dataTable.Clone();
            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(emptyTable);

            // Act & Assert
            Assert.Throws<RepositoryException>(() => usersRepository.UpdateUser(updatedUser));
        }

        [Test]
        public void UpdateUser_SqlError_ThrowsRepositoryException()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => usersRepository.UpdateUser(new User()));
        }

        [Test]
        public void CreateUser_CorrectUser_ReturnsCreatedUser()
        {
            // Arrange
            var newUser = new User
            {
                Username = "created",
                Email = "created@example.com",
                Password = "irrelevant",
                IsDeveloper = false
            };
            var table = dataTable.Clone();
            table.Rows.Add(5, "created", "created@example.com", false, DateTime.Now, DBNull.Value);

            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(table);

            // Act
            var result = usersRepository.CreateUser(newUser);

            // Assert
            Assert.That(result.UserId, Is.EqualTo(5));
            Assert.That(result.Username, Is.EqualTo("created"));
            Assert.That(result.Email, Is.EqualTo("created@example.com"));
        }

        [Test]
        public void CreateUser_NoRows_ThrowsRepositoryException()
        {
            // Arrange
            var newUser = new User { Username = "x", Email = "x", Password = "p", IsDeveloper = false };
            var emptyTable = dataTable.Clone();
            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(emptyTable);

            // Act & Assert
            Assert.Throws<RepositoryException>(() => usersRepository.CreateUser(newUser));
        }

        [Test]
        public void CreateUser_SqlError_ThrowsRepositoryException()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => usersRepository.CreateUser(new User()));
        }

        [Test]
        public void DeleteUser_ValidUser_CallsExecuteNonQuerySql()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(1);

            // Act
            usersRepository.DeleteUser(42);

            // Assert
            dataLinkMock.Verify(
                dl => dl.ExecuteNonQuerySql(
                    It.IsAny<string>(),
                    It.Is<SqlParameter[]>(p => (int)p[0].Value == 42)),
                Times.Once);
        }

        [Test]
        public void DeleteUser_SqlError_ThrowsRepositoryException()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => usersRepository.DeleteUser(1));
        }

        [Test]
        public void VerifyCredentials_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("user_id", typeof(int));
            table.Columns.Add("username", typeof(string));
            table.Columns.Add("email", typeof(string));
            table.Columns.Add("hashed_password", typeof(string));
            table.Columns.Add("developer", typeof(bool));
            table.Columns.Add("created_at", typeof(DateTime));
            table.Columns.Add("last_login", typeof(DateTime));

            table.Rows.Add(7, "user7", "user7@example.com", "hash", true, DateTime.Now, DBNull.Value);

            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(table);

            // Act
            var user = usersRepository.VerifyCredentials("user7@example.com");

            // Assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user!.Email, Is.EqualTo("user7@example.com"));
        }

        [Test]
        public void VerifyCredentials_InvalidCredentials_ReturnsNull()
        {
            // Arrange
            var emptyTable = new DataTable();
            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(emptyTable);

            // Act
            var user = usersRepository.VerifyCredentials("nope");

            // Assert
            Assert.That(user, Is.Null);
        }

        [Test]
        public void VerifyCredentials_SqlError_ThrowsRepositoryException()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => usersRepository.VerifyCredentials("irrelevant"));
        }

        [Test]
        public void GetUserByEmail_CorrectEmail_ReturnsUser()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("user_id", typeof(int));
            table.Columns.Add("username", typeof(string));
            table.Columns.Add("email", typeof(string));
            table.Columns.Add("developer", typeof(bool));
            table.Columns.Add("created_at", typeof(DateTime));
            table.Columns.Add("last_login", typeof(DateTime));

            table.Rows.Add(8, "user8", "user8@example.com", false, DateTime.Now, DBNull.Value);

            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(table);

            // Act
            var user = usersRepository.GetUserByEmail("user8@example.com");

            // Assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user!.Email, Is.EqualTo("user8@example.com"));
        }

        [Test]
        public void GetUserByEmail_WrongEmail_ReturnsNull()
        {
            // Arrange
            var emptyTable = new DataTable();
            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(emptyTable);

            // Act
            var user = usersRepository.GetUserByEmail("wrong@example.com");

            // Assert
            Assert.That(user, Is.Null);
        }

        [Test]
        public void GetUserByEmail_SqlError_ThrowsRepositoryException()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => usersRepository.GetUserByEmail("x"));
        }

        [Test]
        public void GetUserByUsername_CorrectUsername_ReturnsUser()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("user_id", typeof(int));
            table.Columns.Add("username", typeof(string));
            table.Columns.Add("email", typeof(string));
            table.Columns.Add("developer", typeof(bool));
            table.Columns.Add("created_at", typeof(DateTime));
            table.Columns.Add("last_login", typeof(DateTime));

            table.Rows.Add(9, "user9", "user9@example.com", true, DateTime.Now, DBNull.Value);

            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(table);

            // Act
            var user = usersRepository.GetUserByUsername("user9");

            // Assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user!.Username, Is.EqualTo("user9"));
        }

        [Test]
        public void GetUserByUsername_WrongUsername_ReturnsNull()
        {
            // Arrange
            var emptyTable = new DataTable();
            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(emptyTable);

            // Act
            var user = usersRepository.GetUserByUsername("no_such");

            // Assert
            Assert.That(user, Is.Null);
        }

        [Test]
        public void GetUserByUsername_SqlError_ThrowsRepositoryException()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => usersRepository.GetUserByUsername("irrelevant"));
        }

        [Test]
        public void CheckUserExists_UserExists_ReturnsErrorType()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("ErrorType", typeof(string));
            table.Rows.Add("EMAIL_EXISTS");

            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(table);

            // Act
            var error = usersRepository.CheckUserExists("a@b.com", "user");

            // Assert
            Assert.That(error, Is.EqualTo("EMAIL_EXISTS"));
        }

        [Test]
        public void CheckUserExists_NullErrorType_ReturnsNull()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("ErrorType", typeof(string));
            table.Rows.Add(DBNull.Value);

            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(table);

            // Act
            var error = usersRepository.CheckUserExists("a@b.com", "user");

            // Assert
            Assert.That(error, Is.Null);
        }

        [Test]
        public void CheckUserExists_NoRows_ReturnsNull()
        {
            // Arrange
            var emptyTable = new DataTable();
            emptyTable.Columns.Add("ErrorType", typeof(string));

            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(emptyTable);

            // Act
            var error = usersRepository.CheckUserExists("x", "y");

            // Assert
            Assert.That(error, Is.Null);
        }

        [Test]
        public void CheckUserExists_SqlError_ThrowsRepositoryException()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => usersRepository.CheckUserExists("x", "y"));
        }

        [Test]
        public void ChangeEmail_Called_DoesNotThrow()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Verifiable();

            // Act & Assert
            Assert.DoesNotThrow(() => usersRepository.ChangeEmail(1, "new@example.com"));
        }

        [Test]
        public void ChangeEmail_SqlError_ThrowsRepositoryException()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => usersRepository.ChangeEmail(1, "x"));
        }

        [Test]
        public void ChangePassword_Called_DoesNotThrow()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Verifiable();

            // Act & Assert
            Assert.DoesNotThrow(() => usersRepository.ChangePassword(1, "newPass"));
        }

        [Test]
        public void ChangePassword_SqlError_ThrowsRepositoryException()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => usersRepository.ChangePassword(1, "x"));
        }

        [Test]
        public void ChangeUsername_Called_DoesNotThrow()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Verifiable();

            // Act & Assert
            Assert.DoesNotThrow(() => usersRepository.ChangeUsername(1, "newUser"));
        }

        [Test]
        public void ChangeUsername_SqlError_ThrowsRepositoryException()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => usersRepository.ChangeUsername(1, "x"));
        }

        [Test]
        public void UpdateLastLogin_Called_DoesNotThrow()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Verifiable();

            // Act & Assert
            Assert.DoesNotThrow(() => usersRepository.UpdateLastLogin(1));
        }

        [Test]
        public void UpdateLastLogin_SqlError_ThrowsRepositoryException()
        {
            // Arrange
            dataLinkMock
                .Setup(dl => dl.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => usersRepository.UpdateLastLogin(1));
        }

        [Test]
        public void MapDataRowToUser_ValidDataRow_ReturnsUser()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("user_id", typeof(int));
            table.Columns.Add("username", typeof(string));
            table.Columns.Add("email", typeof(string));
            table.Columns.Add("developer", typeof(bool));
            table.Columns.Add("created_at", typeof(DateTime));
            table.Columns.Add("last_login", typeof(DateTime));
            table.Rows.Add(1, "u", "u@x.com", true, DateTime.Now, DateTime.Now);

            // Act
            var user = usersRepository.MapDataRowToUser(table.Rows[0]);

            // Assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user!.IsDeveloper, Is.True);
        }

        [Test]
        public void MapDataRowToUser_MissingFields_ReturnsNull()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("user_id", typeof(int));
            table.Columns.Add("username", typeof(string));
            table.Columns.Add("email", typeof(string));
            table.Rows.Add(1, "u", DBNull.Value);

            // Act
            var user = usersRepository.MapDataRowToUser(table.Rows[0]);

            // Assert
            Assert.That(user, Is.Null);
        }

        [Test]
        public void MapDataRowToUser_DeveloperNull_DefaultsFalse()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("user_id", typeof(int));
            table.Columns.Add("username", typeof(string));
            table.Columns.Add("email", typeof(string));
            table.Columns.Add("developer", typeof(bool));
            table.Columns.Add("created_at", typeof(DateTime));
            table.Columns.Add("last_login", typeof(DateTime));
            table.Rows.Add(2, "u2", "u2@x.com", DBNull.Value, DBNull.Value, DBNull.Value);

            // Act
            var user = usersRepository.MapDataRowToUser(table.Rows[0]);

            // Assert
            Assert.That(user!.IsDeveloper, Is.False);
            Assert.That(user.LastLogin, Is.Null);
        }

        [Test]
        public void MapDataRowToUserWithPassword_ValidDataRow_ReturnsUser()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("user_id", typeof(int));
            table.Columns.Add("username", typeof(string));
            table.Columns.Add("email", typeof(string));
            table.Columns.Add("developer", typeof(bool));
            table.Columns.Add("created_at", typeof(DateTime));
            table.Columns.Add("last_login", typeof(DateTime));
            table.Columns.Add("hashed_password", typeof(string));
            table.Rows.Add(3, "u3", "u3@x.com", true, DateTime.Now, DBNull.Value, "pwdhash");

            // Act
            var user = usersRepository.MapDataRowToUserWithPassword(table.Rows[0]);

            // Assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user!.Password, Is.EqualTo("pwdhash"));
        }

        [Test]
        public void MapDataRowToUserWithPassword_MissingFields_ReturnsNull()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("user_id", typeof(int));
            table.Columns.Add("username", typeof(string));
            table.Columns.Add("email", typeof(string));
            table.Columns.Add("hashed_password", typeof(string));
            table.Rows.Add(DBNull.Value, "u", "u@x.com", "pwd");

            // Act
            var user = usersRepository.MapDataRowToUserWithPassword(table.Rows[0]);

            // Assert
            Assert.That(user, Is.Null);
        }

        [Test]
        public void MapDataRowToUserWithPassword_DeveloperNull_DefaultsFalse()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("user_id", typeof(int));
            table.Columns.Add("username", typeof(string));
            table.Columns.Add("email", typeof(string));
            table.Columns.Add("developer", typeof(bool));
            table.Columns.Add("created_at", typeof(DateTime));
            table.Columns.Add("last_login", typeof(DateTime));
            table.Columns.Add("hashed_password", typeof(string));
            table.Rows.Add(4, "u4", "u4@x.com", DBNull.Value, DBNull.Value, DBNull.Value, "pwd");

            // Act
            var user = usersRepository.MapDataRowToUserWithPassword(table.Rows[0]);

            // Assert
            Assert.That(user!.IsDeveloper, Is.False);
            Assert.That(user.CreatedAt, Is.EqualTo(DateTime.MinValue));
            Assert.That(user.LastLogin, Is.Null);
            Assert.That(user.Password, Is.EqualTo("pwd"));
        }

        [Test]
        public void Constructor_NullDataLink_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UsersRepository(null!));
        }
    }
    */
}
