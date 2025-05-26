using System;
using System.Runtime.Serialization;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using BusinessLayer.Data;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;
using Microsoft.Data.SqlClient;
using BusinessLayer.Services;

namespace Tests.RepositoryTests
{
    /*
    [TestFixture]
    public class FriendshipsRepositoryTests
    {
        private Mock<IDataLink> mockDataLink;
        private FriendshipsRepository friendshipsRepository;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Create a new mock IDataLink and instantiate the FriendshipsRepository.
            mockDataLink = new Mock<IDataLink>();
            friendshipsRepository = new FriendshipsRepository(mockDataLink.Object);
        }

        [Test]
        public void Constructor_NullDataLink_ThrowsArgumentNullException()
        {
            // Assert: Passing a null IDataLink should throw an ArgumentNullException.
            Assert.Throws<ArgumentNullException>(() => new FriendshipsRepository(null));
        }

        #region Helper Methods

        private DataTable CreateFriendshipsDataTable(params (int friendshipId, int userId, int friendId)[] rowData)
        {
            DataTable friendshipsDataTable = new DataTable();
            friendshipsDataTable.Columns.Add("friendship_id", typeof(int));
            friendshipsDataTable.Columns.Add("user_id", typeof(int));
            friendshipsDataTable.Columns.Add("friend_id", typeof(int));

            foreach (var (friendshipId, userId, friendId) in rowData)
            {
                var dataRow = friendshipsDataTable.NewRow();
                dataRow["friendship_id"] = friendshipId;
                dataRow["user_id"] = userId;
                dataRow["friend_id"] = friendId;
                friendshipsDataTable.Rows.Add(dataRow);
            }
            return friendshipsDataTable;
        }

        private DataTable CreateFriendshipsDataTableWithUsernames(params (int friendshipId, int userId, int friendId, string friendUsername, string friendProfilePicture)[] rowData)
        {
            DataTable friendshipsDataTable = new DataTable();
            friendshipsDataTable.Columns.Add("friendship_id", typeof(int));
            friendshipsDataTable.Columns.Add("user_id", typeof(int));
            friendshipsDataTable.Columns.Add("friend_id", typeof(int));
            friendshipsDataTable.Columns.Add("friend_username", typeof(string));
            friendshipsDataTable.Columns.Add("friend_profile_picture", typeof(string));

            foreach (var (friendshipId, userId, friendId, friendUsername, friendProfilePicture) in rowData)
            {
                var dataRow = friendshipsDataTable.NewRow();
                dataRow["friendship_id"] = friendshipId;
                dataRow["user_id"] = userId;
                dataRow["friend_id"] = friendId;
                dataRow["friend_username"] = friendUsername;
                dataRow["friend_profile_picture"] = friendProfilePicture;
                friendshipsDataTable.Rows.Add(dataRow);
            }
            return friendshipsDataTable;
        }

        private DataTable CreateUserDataTable(string username)
        {
            DataTable userDataTable = new DataTable();
            userDataTable.Columns.Add("username", typeof(string));
            var userRow = userDataTable.NewRow();
            userRow["username"] = username;
            userDataTable.Rows.Add(userRow);
            return userDataTable;
        }

        private DatabaseOperationException CreateDatabaseOperationException()
        {
            return new DatabaseOperationException("Database error", CreateSqlException());
        }

        private SqlException CreateSqlException()
        {
            // Create an uninitialized SqlException.
            return (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));
        }

        #endregion

        #region GetAllFriendships Tests

        [Test]
        public void GetAllFriendships_ForUser_ReturnsCorrectCount()
        {
            // Arrange
            var friendshipsTable = CreateFriendshipsDataTableWithUsernames(
                (1, 1, 2, "Alice", "alice.jpg"),
                (2, 1, 3, "Bob", "bob.jpg"));
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(friendshipsTable);

            // Act
            var friendshipsForUser = friendshipsRepository.GetAllFriendships(1);

            // Assert: There should be exactly 2 friendships.
            Assert.That(friendshipsForUser.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetAllFriendships_ForUser_AssignsFriendUsernameCorrectly()
        {
            // Arrange
            var friendshipsTable = CreateFriendshipsDataTableWithUsernames(
                (1, 1, 2, "Charlie", "charlie.jpg"));
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(friendshipsTable);

            // Act
            var friendshipsForUser = friendshipsRepository.GetAllFriendships(1);

            // Assert: FriendUsername should be "Charlie".
            Assert.That(friendshipsForUser.First().FriendUsername, Is.EqualTo("Charlie"));
        }

        [Test]
        public void GetAllFriendships_ForUser_AssignsFriendProfilePictureCorrectly()
        {
            // Arrange
            var friendshipsTable = CreateFriendshipsDataTableWithUsernames(
                (1, 1, 2, "Diana", "diana.png"));
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(friendshipsTable);

            // Act
            var friendshipsForUser = friendshipsRepository.GetAllFriendships(1);

            // Assert: FriendProfilePicture should be "diana.png".
            Assert.That(friendshipsForUser.First().FriendProfilePicture, Is.EqualTo("diana.png"));
        }

        [Test]
        public void GetAllFriendships_ForUser_OrdersByFriendUsername_ReturnsCorrectOrder()
        {
            // Arrange: Already ordered in SQL query, simulate pre-ordered results
            var friendshipsTable = CreateFriendshipsDataTableWithUsernames(
                (2, 1, 3, "Anna", "anna.jpg"),
                (1, 1, 2, "Zoe", "zoe.jpg"));

            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(friendshipsTable);

            // Act
            var friendshipsForUser = friendshipsRepository.GetAllFriendships(1);

            // Assert: The first friendship's FriendUsername should be "Anna" (already ordered in query result)
            Assert.That(friendshipsForUser.First().FriendUsername, Is.EqualTo("Anna"));
        }

        [Test]
        public void GetAllFriendships_DatabaseOperationException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            var databaseException = CreateDatabaseOperationException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(databaseException);

            // Act & Assert
            var repositoryException = Assert.Throws<RepositoryException>(() => friendshipsRepository.GetAllFriendships(1));
            Assert.That(repositoryException.Message, Is.EqualTo("Database error while retrieving friendships."));
        }

        [Test]
        public void GetAllFriendships_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));

            // Act & Assert
            var repositoryException = Assert.Throws<RepositoryException>(() => friendshipsRepository.GetAllFriendships(1));
            Assert.That(repositoryException.Message, Is.EqualTo("An unexpected error occurred while retrieving friendships."));
        }

        #endregion

        #region GetFriendshipById Tests

        [Test]
        public void GetFriendshipById_ExistingId_ReturnsFriendship()
        {
            // Arrange
            var friendshipDataTable = CreateFriendshipsDataTable((10, 1, 2));
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(friendshipDataTable);

            // Act
            var retrievedFriendship = friendshipsRepository.GetFriendshipById(10);

            // Assert: The retrieved friendship's ID should be 10.
            Assert.That(retrievedFriendship.FriendshipId, Is.EqualTo(10));
        }

        [Test]
        public void GetFriendshipById_NonExistingId_ReturnsNull()
        {
            // Arrange: Return an empty DataTable.
            DataTable emptyFriendshipTable = new DataTable();
            emptyFriendshipTable.Columns.Add("friendship_id", typeof(int));
            emptyFriendshipTable.Columns.Add("user_id", typeof(int));
            emptyFriendshipTable.Columns.Add("friend_id", typeof(int));
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(emptyFriendshipTable);

            // Act
            var retrievedFriendship = friendshipsRepository.GetFriendshipById(999);

            // Assert: The friendship should be null.
            Assert.That(retrievedFriendship, Is.Null);
        }

        [Test]
        public void GetFriendshipById_DatabaseOperationException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            var databaseException = CreateDatabaseOperationException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(databaseException);

            // Act & Assert
            var repositoryException = Assert.Throws<RepositoryException>(() => friendshipsRepository.GetFriendshipById(1));
            Assert.That(repositoryException.Message, Is.EqualTo("Database error while retrieving friendship by ID."));
        }

        [Test]
        public void GetFriendshipById_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));

            // Act & Assert
            var repositoryException = Assert.Throws<RepositoryException>(() => friendshipsRepository.GetFriendshipById(1));
            Assert.That(repositoryException.Message, Is.EqualTo("An unexpected error occurred while retrieving friendship by ID."));
        }

        #endregion

        #region AddFriendship Tests

        [Test]
        public void AddFriendship_ValidData_DoesNotThrow()
        {
            // Arrange: Setup check for both users exist and friendship doesn't exist
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(
                    It.IsAny<string>(),
                    It.Is<SqlParameter[]>(p => p.Length == 1 && ((int)p[0].Value == 1 || (int)p[0].Value == 2))))
                .Returns(1); // Both users exist

            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p.Length == 2)))
                .Returns(0); // Friendship doesn't exist

            // Setup ExecuteNonQuery for adding friendship
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act & Assert: Adding a valid friendship should not throw.
            Assert.DoesNotThrow(() => friendshipsRepository.AddFriendship(1, 2));
        }

        [Test]
        public void AddFriendship_WhenUserDoesNotExist_ThrowsRepositoryException()
        {
            // Arrange: User 1 doesn't exist
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p.Length == 1 && (int)p[0].Value == 1)))
                .Returns(0);

            // Act & Assert: Expect a RepositoryException because user 1 does not exist.
            var repositoryException = Assert.Throws<RepositoryException>(() => friendshipsRepository.AddFriendship(1, 2));
            Assert.That(repositoryException.Message, Is.EqualTo("User with ID 1 does not exist."));
        }

        [Test]
        public void AddFriendship_WhenFriendDoesNotExist_ThrowsRepositoryException()
        {
            // Arrange: User 1 exists but friend 2 doesn't
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p.Length == 1 && (int)p[0].Value == 1)))
                .Returns(1);

            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p.Length == 1 && (int)p[0].Value == 2)))
                .Returns(0);

            // Act & Assert: Expect a RepositoryException because friend with ID 2 does not exist.
            var repositoryException = Assert.Throws<RepositoryException>(() => friendshipsRepository.AddFriendship(1, 2));
            Assert.That(repositoryException.Message, Is.EqualTo("User with ID 2 does not exist."));
        }

        [Test]
        public void AddFriendship_WhenFriendshipAlreadyExists_ThrowsRepositoryException()
        {
            // Arrange: Both users exist
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p.Length == 1)))
                .Returns(1);

            // Friendship already exists
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p.Length == 2)))
                .Returns(1);

            // Act & Assert: Adding a duplicate friendship should throw a RepositoryException.
            var repositoryException = Assert.Throws<RepositoryException>(() => friendshipsRepository.AddFriendship(1, 2));
            Assert.That(repositoryException.Message, Is.EqualTo("Friendship already exists."));
        }

        [Test]
        public void AddFriendship_DatabaseOperationException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange: Both users exist
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p.Length == 1)))
                .Returns(1);

            // Friendship doesn't exist
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p.Length == 2)))
                .Returns(0);

            // ExecuteNonQuery throws database exception
            var databaseException = CreateDatabaseOperationException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(databaseException);

            // Act & Assert
            var repositoryException = Assert.Throws<RepositoryException>(() => friendshipsRepository.AddFriendship(1, 2));
            Assert.That(repositoryException.Message, Is.EqualTo("Database error while adding friendship."));
        }

        [Test]
        public void AddFriendship_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange: Both users exist
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p.Length == 1)))
                .Returns(1);

            // Friendship doesn't exist
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p.Length == 2)))
                .Returns(0);

            // ExecuteNonQuery throws generic exception
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));

            // Act & Assert
            var repositoryException = Assert.Throws<RepositoryException>(() => friendshipsRepository.AddFriendship(1, 2));
            Assert.That(repositoryException.Message, Is.EqualTo("An unexpected error occurred while adding friendship."));
        }

        #endregion

        #region RemoveFriendship Tests

        [Test]
        public void RemoveFriendship_ValidInput_CallsExecuteNonQuerySql()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Verifiable();

            // Act
            friendshipsRepository.RemoveFriendship(5);

            // Assert: Verify that ExecuteNonQuerySql was called once.
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void RemoveFriendship_DatabaseOperationException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            var databaseException = CreateDatabaseOperationException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(databaseException);

            // Act & Assert
            var repositoryException = Assert.Throws<RepositoryException>(() => friendshipsRepository.RemoveFriendship(5));
            Assert.That(repositoryException.Message, Is.EqualTo("Database error while removing friendship."));
        }

        [Test]
        public void RemoveFriendship_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));

            // Act & Assert
            var repositoryException = Assert.Throws<RepositoryException>(() => friendshipsRepository.RemoveFriendship(5));
            Assert.That(repositoryException.Message, Is.EqualTo("An unexpected error occurred while removing friendship."));
        }

        #endregion

        #region GetFriendshipCount Tests

        [Test]
        public void GetFriendshipCount_ForUser_ReturnsCorrectCount()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(7);

            // Act
            int friendshipCount = friendshipsRepository.GetFriendshipCount(1);

            // Assert: Count should be 7.
            Assert.That(friendshipCount, Is.EqualTo(7));
        }

        [Test]
        public void GetFriendshipCount_DatabaseOperationException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            var databaseException = CreateDatabaseOperationException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(databaseException);

            // Act & Assert
            var repositoryException = Assert.Throws<RepositoryException>(() => friendshipsRepository.GetFriendshipCount(1));
            Assert.That(repositoryException.Message, Is.EqualTo("Database error while retrieving friendship count."));
        }

        [Test]
        public void GetFriendshipCount_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));

            // Act & Assert
            var repositoryException = Assert.Throws<RepositoryException>(() => friendshipsRepository.GetFriendshipCount(1));
            Assert.That(repositoryException.Message, Is.EqualTo("An unexpected error occurred while retrieving friendship count."));
        }

        #endregion

        #region GetFriendshipId Tests

        [Test]
        public void GetFriendshipId_ExistingFriendship_ReturnsCorrectId()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int?>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(15);

            // Act
            int? retrievedFriendshipId = friendshipsRepository.GetFriendshipId(1, 2);

            // Assert: The returned friendship ID should be 15.
            Assert.That(retrievedFriendshipId, Is.EqualTo(15));
        }

        [Test]
        public void GetFriendshipId_NonExistingFriendship_ReturnsNull()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int?>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns((int?)null);

            // Act
            int? retrievedFriendshipId = friendshipsRepository.GetFriendshipId(1, 2);

            // Assert: The returned friendship ID should be null.
            Assert.That(retrievedFriendshipId, Is.Null);
        }

        [Test]
        public void GetFriendshipId_DatabaseOperationException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            var databaseException = CreateDatabaseOperationException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int?>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(databaseException);

            // Act & Assert
            var repositoryException = Assert.Throws<RepositoryException>(() => friendshipsRepository.GetFriendshipId(1, 2));
            Assert.That(repositoryException.Message, Is.EqualTo("Database error while retrieving friendship ID."));
        }

        [Test]
        public void GetFriendshipId_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int?>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));

            // Act & Assert
            var repositoryException = Assert.Throws<RepositoryException>(() => friendshipsRepository.GetFriendshipId(1, 2));
            Assert.That(repositoryException.Message, Is.EqualTo("An unexpected error occurred while retrieving friendship ID."));
        }

        #endregion
    }
    */
}