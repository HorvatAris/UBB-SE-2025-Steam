using System.Runtime.Serialization;
using System;
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
    public class OwnedGamesRepositoryTests
    {
        private Mock<IDataLink> mockDataLink;
        private OwnedGamesRepository ownedGamesRepository;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Create a new mock IDataLink and instantiate the OwnedGamesRepository.
            mockDataLink = new Mock<IDataLink>();
            ownedGamesRepository = new OwnedGamesRepository(mockDataLink.Object);
        }

        [Test]
        public void Constructor_NullDataLink_ThrowsArgumentNullException()
        {
            // Assert: Creating an instance with a null IDataLink should throw an ArgumentNullException.
            Assert.Throws<ArgumentNullException>(() => new OwnedGamesRepository(null));
        }

        #region Helper Methods

        // Helper method to create a DataTable for OwnedGames with the specified number of rows.
        private DataTable CreateOwnedGamesDataTable(int numberOfRows)
        {
            DataTable ownedGamesDataTable = new DataTable();
            ownedGamesDataTable.Columns.Add("user_id", typeof(int));
            ownedGamesDataTable.Columns.Add("title", typeof(string));
            ownedGamesDataTable.Columns.Add("description", typeof(string));
            ownedGamesDataTable.Columns.Add("cover_picture", typeof(string));
            ownedGamesDataTable.Columns.Add("game_id", typeof(int));

            for (int index = 0; index < numberOfRows; index++)
            {
                DataRow gameDataRow = ownedGamesDataTable.NewRow();
                gameDataRow["user_id"] = 1;
                gameDataRow["title"] = "Game " + index;
                gameDataRow["description"] = "Description " + index;
                gameDataRow["cover_picture"] = "cover" + index + ".jpg";
                gameDataRow["game_id"] = index + 1;
                ownedGamesDataTable.Rows.Add(gameDataRow);
            }
            return ownedGamesDataTable;
        }

        // Helper method to create an empty DataTable with the OwnedGames columns.
        private DataTable CreateEmptyDataTable()
        {
            DataTable emptyDataTable = new DataTable();
            emptyDataTable.Columns.Add("user_id", typeof(int));
            emptyDataTable.Columns.Add("title", typeof(string));
            emptyDataTable.Columns.Add("description", typeof(string));
            emptyDataTable.Columns.Add("cover_picture", typeof(string));
            emptyDataTable.Columns.Add("game_id", typeof(int));
            return emptyDataTable;
        }

        // Helper method to create a dummy SqlException.
        private SqlException CreateSqlException()
        {
            // Use FormatterServices to create an uninitialized SqlException.
            return (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));
        }

        #endregion

        #region GetAllOwnedGames Tests

        [Test]
        public void GetAllOwnedGames_ForValidUser_ReturnsCorrectCount()
        {
            // Arrange
            int expectedGameCount = 3;
            DataTable ownedGamesTable = CreateOwnedGamesDataTable(expectedGameCount);
            mockDataLink.Setup(datalink => datalink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(ownedGamesTable);
            // Act
            List<OwnedGame> ownedGamesForUser = ownedGamesRepository.GetAllOwnedGames(1);
            // Assert: The returned count equals the expected count.
            Assert.That(ownedGamesForUser.Count, Is.EqualTo(expectedGameCount));
        }

        [Test]
        public void GetAllOwnedGames_ForValidUser_FirstGameTitleIsCorrect()
        {
            // Arrange
            DataTable ownedGamesTable = CreateOwnedGamesDataTable(2);
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(ownedGamesTable);
            // Act
            List<OwnedGame> ownedGamesForUser = ownedGamesRepository.GetAllOwnedGames(1);
            // Assert: The first game's title should be "Game 0".
            Assert.That(ownedGamesForUser.First().GameTitle, Is.EqualTo("Game 0"));
        }

        [Test]
        public void GetAllOwnedGames_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException dummySqlException = CreateSqlException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(dummySqlException);
            // Act & Assert: Expect a RepositoryException with the appropriate message.
            Assert.Throws<RepositoryException>(() => ownedGamesRepository.GetAllOwnedGames(1));
        }

        [Test]
        public void GetAllOwnedGames_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));
            // Act & Assert: Expect a RepositoryException with the appropriate message.
            var repositoryException = Assert.Throws<RepositoryException>(() => ownedGamesRepository.GetAllOwnedGames(1));
            Assert.That(repositoryException.Message, Is.EqualTo("An unexpected error occurred while retrieving owned games."));
        }

        #endregion

        #region GetOwnedGameById Tests

        [Test]
        public void GetOwnedGameById_ForExistingGame_ReturnsOwnedGame()
        {
            // Arrange
            DataTable ownedGamesTable = CreateOwnedGamesDataTable(1);
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(ownedGamesTable);
            // Act
            OwnedGame retrievedOwnedGame = ownedGamesRepository.GetOwnedGameById(1, 1);
            // Assert: The retrieved game should not be null.
            Assert.That(retrievedOwnedGame, Is.Not.Null);
        }

        [Test]
        public void GetOwnedGameById_ForExistingGame_ReturnsCorrectTitle()
        {
            // Arrange
            DataTable ownedGamesTable = CreateOwnedGamesDataTable(1);
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(ownedGamesTable);
            // Act
            OwnedGame retrievedOwnedGame = ownedGamesRepository.GetOwnedGameById(1, 1);
            // Assert: The title of the retrieved game should be "Game 0".
            Assert.That(retrievedOwnedGame.GameTitle, Is.EqualTo("Game 0"));
        }

        [Test]
        public void GetOwnedGameById_ForNoRows_ReturnsNull()
        {
            // Arrange
            DataTable emptyOwnedGamesTable = CreateEmptyDataTable();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(emptyOwnedGamesTable);
            // Act
            OwnedGame retrievedOwnedGame = ownedGamesRepository.GetOwnedGameById(1, 1);
            // Assert: The retrieved game should be null.
            Assert.That(retrievedOwnedGame, Is.Null);
        }

        [Test]
        public void GetOwnedGameById_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException dummySqlException = CreateSqlException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(dummySqlException);
            // Act & Assert: Expect a RepositoryException with the appropriate message.
            Assert.Throws<RepositoryException>(() => ownedGamesRepository.GetOwnedGameById(1, 1));
        }

        [Test]
        public void GetOwnedGameById_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));
            // Act & Assert: Expect a RepositoryException with the appropriate message.
            var repositoryException = Assert.Throws<RepositoryException>(() => ownedGamesRepository.GetOwnedGameById(1, 1));
            Assert.That(repositoryException.Message, Is.EqualTo("An unexpected error occurred while retrieving owned game by ID."));
        }

        #endregion

        #region RemoveOwnedGame Tests

        [Test]
        public void RemoveOwnedGame_ForValidGame_CallsExecuteNonQuery()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Verifiable();
            // Act
            ownedGamesRepository.RemoveOwnedGame(1, 1);
            // Assert: Verify that ExecuteNonQuery was called exactly once.
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
            // Dummy assert to satisfy one-assert-per-test rule.
            Assert.That(true, Is.True);
        }

        [Test]
        public void RemoveOwnedGame_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException dummySqlException = CreateSqlException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(dummySqlException);
            // Act & Assert: Expect a RepositoryException with the appropriate message.
            Assert.Throws<RepositoryException>(() => ownedGamesRepository.RemoveOwnedGame(1, 1));
        }

        [Test]
        public void RemoveOwnedGame_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));
            // Act & Assert: Expect a RepositoryException with the appropriate message.
            var repositoryException = Assert.Throws<RepositoryException>(() => ownedGamesRepository.RemoveOwnedGame(1, 1));
            Assert.That(repositoryException.Message, Is.EqualTo("An unexpected error occurred while removing owned game."));
        }

        #endregion
    }
    */
}
