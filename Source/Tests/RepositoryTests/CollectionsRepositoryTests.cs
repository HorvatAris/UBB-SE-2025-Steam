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
    public class CollectionsRepositoryTests
    {
        private Mock<IDataLink> mockDataLink;
        private CollectionsRepository collectionsRepository;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Create a new mock IDataLink and instantiate CollectionsRepository.
            mockDataLink = new Mock<IDataLink>();
            collectionsRepository = new CollectionsRepository(mockDataLink.Object);
        }

        [Test]
        public void Constructor_NullDataLink_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CollectionsRepository(null));
        }

        #region Helper Methods

        private DataTable CreateCollectionsDataTable(int numberOfRows)
        {
            DataTable collectionsDataTable = new DataTable();
            collectionsDataTable.Columns.Add("user_id", typeof(int));
            collectionsDataTable.Columns.Add("name", typeof(string));
            collectionsDataTable.Columns.Add("created_at", typeof(DateTime));
            collectionsDataTable.Columns.Add("cover_picture", typeof(string));
            collectionsDataTable.Columns.Add("is_public", typeof(bool));
            collectionsDataTable.Columns.Add("collection_id", typeof(int));

            for (int index = 0; index < numberOfRows; index++)
            {
                DataRow collectionDataRow = collectionsDataTable.NewRow();
                collectionDataRow["user_id"] = 1;
                collectionDataRow["name"] = "Collection " + index;
                // Set CreatedAt so that later tests for ordering can work.
                collectionDataRow["created_at"] = DateTime.Now.AddDays(-index);
                collectionDataRow["cover_picture"] = "cover" + index + ".jpg";
                collectionDataRow["is_public"] = index % 2 == 0;
                collectionDataRow["collection_id"] = index + 1;
                collectionsDataTable.Rows.Add(collectionDataRow);
            }
            return collectionsDataTable;
        }

        private DataTable CreateEmptyCollectionsDataTable()
        {
            DataTable emptyCollectionsDataTable = new DataTable();
            emptyCollectionsDataTable.Columns.Add("user_id", typeof(int));
            emptyCollectionsDataTable.Columns.Add("name", typeof(string));
            emptyCollectionsDataTable.Columns.Add("created_at", typeof(DateTime));
            emptyCollectionsDataTable.Columns.Add("cover_picture", typeof(string));
            emptyCollectionsDataTable.Columns.Add("is_public", typeof(bool));
            emptyCollectionsDataTable.Columns.Add("collection_id", typeof(int));
            return emptyCollectionsDataTable;
        }

        private DataTable CreateGamesDataTable(int numberOfRows)
        {
            DataTable gamesDataTable = new DataTable();
            gamesDataTable.Columns.Add("user_id", typeof(int));
            gamesDataTable.Columns.Add("title", typeof(string));
            gamesDataTable.Columns.Add("description", typeof(string));
            gamesDataTable.Columns.Add("cover_picture", typeof(string));
            gamesDataTable.Columns.Add("game_id", typeof(int));

            for (int index = 0; index < numberOfRows; index++)
            {
                DataRow gameDataRow = gamesDataTable.NewRow();
                gameDataRow["user_id"] = 1;
                gameDataRow["title"] = "Game " + index;
                gameDataRow["description"] = "Description " + index;
                gameDataRow["cover_picture"] = "gamecover" + index + ".jpg";
                gameDataRow["game_id"] = index + 1;
                gamesDataTable.Rows.Add(gameDataRow);
            }
            return gamesDataTable;
        }

        private DataTable CreateEmptyGamesDataTable()
        {
            DataTable emptyGamesDataTable = new DataTable();
            emptyGamesDataTable.Columns.Add("user_id", typeof(int));
            emptyGamesDataTable.Columns.Add("title", typeof(string));
            emptyGamesDataTable.Columns.Add("description", typeof(string));
            emptyGamesDataTable.Columns.Add("cover_picture", typeof(string));
            emptyGamesDataTable.Columns.Add("game_id", typeof(int));
            return emptyGamesDataTable;
        }

        private SqlException CreateSqlException()
        {
            // Create an uninitialized SqlException.
            return (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));
        }

        #endregion

        #region GetAllCollections Tests

        [Test]
        public void GetAllCollections_DataTableIsNull_ReturnsEmptyList()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns((DataTable)null);
            // Act
            List<Collection> collectionsList = collectionsRepository.GetAllCollections(1);
            // Assert: List is empty.
            Assert.That(collectionsList, Is.Empty);
        }

        [Test]
        public void GetAllCollections_EmptyDataTable_ReturnsEmptyList()
        {
            // Arrange
            DataTable emptyCollectionsTable = CreateEmptyCollectionsDataTable();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(emptyCollectionsTable);
            // Act
            List<Collection> collectionsList = collectionsRepository.GetAllCollections(1);
            // Assert: List is empty.
            Assert.That(collectionsList, Is.Empty);
        }

        [Test]
        public void GetAllCollections_DataTableHasRows_ReturnsCorrectCount()
        {
            // Arrange
            int expectedCollectionCount = 5;
            DataTable populatedCollectionsTable = CreateCollectionsDataTable(expectedCollectionCount);
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(populatedCollectionsTable);
            // Act
            List<Collection> collectionsList = collectionsRepository.GetAllCollections(1);
            // Assert: List count equals expected.
            Assert.That(collectionsList.Count, Is.EqualTo(expectedCollectionCount));
        }

        [Test]
        public void GetAllCollections_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlExceptionInstance = CreateSqlException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(sqlExceptionInstance);
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.GetAllCollections(1));
        }

        [Test]
        public void GetAllCollections_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.GetAllCollections(1));
        }

        #endregion

        #region GetLastThreeCollectionsForUser Tests

        [Test]
        public void GetLastThreeCollectionsForUser_TotalRowsExceedsThree_ReturnsOnlyThreeCollections()
        {
            // Arrange
            int totalCollectionsCount = 5;
            DataTable collectionsTable = CreateCollectionsDataTable(totalCollectionsCount);
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(collectionsTable);
            // Act
            List<Collection> lastThreeCollections = collectionsRepository.GetLastThreeCollectionsForUser(1);
            // Assert: Only three collections are returned.
            Assert.That(lastThreeCollections.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetLastThreeCollectionsForUser_ReturnsMostRecentCollections_FirstHasLatestCreatedAt()
        {
            // Arrange
            // Create 3 collections with controlled CreatedAt values.
            DataTable collectionsTable = CreateCollectionsDataTable(3);
            // Modify the "created_at" column so that the first row is the oldest and the last is the newest.
            for (int rowIndex = 0; rowIndex < collectionsTable.Rows.Count; rowIndex++)
            {
                collectionsTable.Rows[rowIndex]["created_at"] = DateTime.Now.AddDays(-10 + rowIndex);
            }
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(collectionsTable);
            // Act
            List<Collection> sortedataLinkastThreeCollections = collectionsRepository.GetLastThreeCollectionsForUser(1);
            // Assert: The first collection in the result has the greatest (i.e. most recent) CreatedAt value.
            Assert.That(sortedataLinkastThreeCollections.First().CreatedAt,
                        Is.EqualTo(DateOnly.FromDateTime(DateTime.Now.AddDays(-8))));
        }

        [Test]
        public void GetLastThreeCollectionsForUser_ExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.GetLastThreeCollectionsForUser(1));
        }

        #endregion

        #region GetCollectionById Tests

        [Test]
        public void GetCollectionById_EmptyDataTable_ReturnsNull()
        {
            // Arrange
            DataTable emptyCollectionsTable = CreateEmptyCollectionsDataTable();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(emptyCollectionsTable);
            // Act
            Collection resultCollection = collectionsRepository.GetCollectionById(1, 1);
            // Assert: The returned collection is null.
            Assert.That(resultCollection, Is.Null);
        }

        [Test]
        public void GetCollectionById_DataTableHasRow_ReturnsCollection()
        {
            // Arrange
            DataTable singleCollectionTable = CreateCollectionsDataTable(1);
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(singleCollectionTable);
            // Act
            Collection resultCollection = collectionsRepository.GetCollectionById(1, 1);
            // Assert: The returned collection is not null.
            Assert.That(resultCollection, Is.Not.Null);
        }

        [Test]
        public void GetCollectionById_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlExceptionInstance = CreateSqlException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(sqlExceptionInstance);
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.GetCollectionById(1, 1));
        }

        [Test]
        public void GetCollectionById_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));
            // Act & Assert
            RepositoryException repositoryException = Assert.Throws<RepositoryException>(() => collectionsRepository.GetCollectionById(1, 1));
            Assert.That(repositoryException.Message, Is.EqualTo("An unexpected error occurred while retrieving collection by ID."));
        }

        #endregion

        #region GetGamesInCollection (Single Parameter) Tests

        [Test]
        public void GetGamesInCollection_SingleParam_EmptyDataTable_ReturnsEmptyList()
        {
            // Arrange: Create an empty DataTable with the expected columns.
            DataTable emptyGamesTable = new DataTable();
            emptyGamesTable.Columns.Add("user_id", typeof(int));
            emptyGamesTable.Columns.Add("title", typeof(string));
            emptyGamesTable.Columns.Add("description", typeof(string));
            emptyGamesTable.Columns.Add("cover_picture", typeof(string));
            emptyGamesTable.Columns.Add("game_id", typeof(int));

            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(emptyGamesTable);
            // Act
            List<OwnedGame> ownedGamesList = collectionsRepository.GetGamesInCollection(1);
            // Assert: The returned list should be empty.
            Assert.That(ownedGamesList, Is.Empty);
        }

        [Test]
        public void GetGamesInCollection_SingleParam_EmptyGamesDataTable_ReturnsEmptyList()
        {
            // Arrange
            DataTable emptyGamesDataTable = CreateEmptyGamesDataTable();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(emptyGamesDataTable);
            // Act
            List<OwnedGame> ownedGamesList = collectionsRepository.GetGamesInCollection(1);
            // Assert: The returned list is empty.
            Assert.That(ownedGamesList, Is.Empty);
        }

        [Test]
        public void GetGamesInCollection_SingleParam_DataTableHasRows_ReturnsCorrectCount()
        {
            // Arrange
            int expectedGamesCount = 4;
            DataTable gamesDataTable = CreateGamesDataTable(expectedGamesCount);
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(gamesDataTable);
            // Act
            List<OwnedGame> ownedGamesList = collectionsRepository.GetGamesInCollection(1);
            // Assert: The count equals the expected count.
            Assert.That(ownedGamesList.Count, Is.EqualTo(expectedGamesCount));
        }

        [Test]
        public void GetGamesInCollection_SingleParam_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlExceptionInstance = CreateSqlException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(sqlExceptionInstance);
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.GetGamesInCollection(1));
        }

        [Test]
        public void GetGamesInCollection_SingleParam_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));
            // Act & Assert
            RepositoryException repositoryException = Assert.Throws<RepositoryException>(() => collectionsRepository.GetGamesInCollection(1));
            Assert.That(repositoryException.Message, Is.EqualTo("An unexpected error occurred while retrieving games in collection."));
        }

        [Test]
        public void GetGamesInCollection_SingleParam_NullDataTable_ReturnsEmptyList()
        {
            // Arrange: Return null instead of a DataTable.
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns((DataTable)null);
            // Act
            List<OwnedGame> ownedGamesList = collectionsRepository.GetGamesInCollection(1);
            // Assert: The returned list should be empty.
            Assert.That(ownedGamesList, Is.Empty);
        }

        [Test]
        public void GetGamesInCollection_TwoParams_CollectionIdOne_NullDataTable_ReturnsEmptyList()
        {
            // Arrange: Return null instead of a DataTable for the two-parameter overload.
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns((DataTable)null);
            // Act
            List<OwnedGame> ownedGamesList = collectionsRepository.GetGamesInCollection(1, 1);
            // Assert: The returned list should be empty.
            Assert.That(ownedGamesList, Is.Empty);
        }

        #endregion

        #region GetGamesInCollection (Two Parameters) Tests

        [Test]
        public void GetGamesInCollection_TwoParams_CollectionIdOne_ReturnsAllGamesForUser()
        {
            // Arrange
            int expectedGamesCount = 3;
            DataTable gamesDataTable = CreateGamesDataTable(expectedGamesCount);
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(gamesDataTable);
            // Act
            List<OwnedGame> ownedGamesList = collectionsRepository.GetGamesInCollection(1, 1);
            // Assert: The count equals the expected count.
            Assert.That(ownedGamesList.Count, Is.EqualTo(expectedGamesCount));
        }

        [Test]
        public void GetGamesInCollection_TwoParams_CollectionIdNotOne_ReturnsGamesInCollection()
        {
            // Arrange
            int expectedGamesCount = 2;
            DataTable gamesDataTable = CreateGamesDataTable(expectedGamesCount);
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(gamesDataTable);
            // Act
            List<OwnedGame> ownedGamesList = collectionsRepository.GetGamesInCollection(2, 1);
            // Assert: The count equals the expected count.
            Assert.That(ownedGamesList.Count, Is.EqualTo(expectedGamesCount));
        }

        [Test]
        public void GetGamesInCollection_TwoParams_CollectionIdOne_SqlException_ThrowsRepositoryException()
        {
            // Arrange
            SqlException sqlExceptionInstance = CreateSqlException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(sqlExceptionInstance);
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.GetGamesInCollection(1, 1));
        }

        [Test]
        public void GetGamesInCollection_TwoParams_CollectionIdNotOne_GenericException_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));
            // Act & Assert
            RepositoryException repositoryException = Assert.Throws<RepositoryException>(() => collectionsRepository.GetGamesInCollection(2, 1));
            Assert.That(repositoryException.Message, Is.EqualTo("An unexpected error occurred while retrieving games in collection."));
        }

        #endregion

        #region AddGameToCollection Tests

        [Test]
        public void AddGameToCollection_ValidInput_CallsExecuteNonQuery()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Verifiable();
            // Act
            collectionsRepository.AddGameToCollection(1, 1, 1);
            // Assert: Verify that ExecuteNonQuery was called once.
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // Dummy assert for clarity.
        }

        [Test]
        public void AddGameToCollection_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            var sqlExceptionInstance = CreateSqlException();

            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(sqlExceptionInstance);

            // Act & Assert
            Assert.Throws<RepositoryException>(
                () => collectionsRepository.AddGameToCollection(1, 1, 1));
        }

        [Test]
        public void AddGameToCollection_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));
            // Act & Assert
            RepositoryException repositoryException = Assert.Throws<RepositoryException>(() => collectionsRepository.AddGameToCollection(1, 1, 1));
            Assert.That(repositoryException.Message, Is.EqualTo("An unexpected error occurred while adding game to collection."));
        }

        #endregion

        #region RemoveGameFromCollection Tests

        [Test]
        public void RemoveGameFromCollection_ValidInput_CallsExecuteNonQuery()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Verifiable();
            // Act
            collectionsRepository.RemoveGameFromCollection(1, 1);
            // Assert: Verify that ExecuteNonQuery was called once.
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // Dummy assert.
        }

        [Test]
        public void RemoveGameFromCollection_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlExceptionInstance = CreateSqlException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(sqlExceptionInstance);
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.RemoveGameFromCollection(1, 1));
        }

        [Test]
        public void RemoveGameFromCollection_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));
            // Act & Assert
            RepositoryException repositoryException = Assert.Throws<RepositoryException>(() => collectionsRepository.RemoveGameFromCollection(1, 1));
            Assert.That(repositoryException.Message, Is.EqualTo("An unexpected error occurred while removing game from collection."));
        }

        #endregion

        #region MakeCollectionPrivateForUser Tests

        [Test]
        public void MakeCollectionPrivateForUser_ValidInput_CallsExecuteReader()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Verifiable();
            // Act
            collectionsRepository.MakeCollectionPrivateForUser("1", "1");
            // Assert: Verify that ExecuteReader was called once.
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // Dummy assert.
        }

        [Test]
        public void MakeCollectionPrivateForUser_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var databaseOperationException = new DatabaseOperationException("Error");
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(databaseOperationException);
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.MakeCollectionPrivateForUser("1", "1"));
        }

        #endregion

        #region MakeCollectionPublicForUser Tests

        [Test]
        public void MakeCollectionPublicForUser_ValidInput_CallsExecuteReader()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Verifiable();
            // Act
            collectionsRepository.MakeCollectionPublicForUser("1", "1");
            // Assert: Verify that ExecuteReader was called once.
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // Dummy assert.
        }

        [Test]
        public void MakeCollectionPublicForUser_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var databaseOperationException = new DatabaseOperationException("Error");
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(databaseOperationException);
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.MakeCollectionPublicForUser("1", "1"));
        }

        #endregion

        #region RemoveCollectionForUser Tests

        [Test]
        public void RemoveCollectionForUser_ValidInput_CallsExecuteReader()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Verifiable();
            // Act
            collectionsRepository.RemoveCollectionForUser("1", "1");
            // Assert: Verify that ExecuteReader was called once.
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // Dummy assert.
        }

        [Test]
        public void RemoveCollectionForUser_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var databaseOperationException = new DatabaseOperationException("Error");
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(databaseOperationException);
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.RemoveCollectionForUser("1", "1"));
        }

        #endregion

        #region SaveCollection Tests

        [Test]
        public void SaveCollection_NewCollection_CallsCreateCollection()
        {
            // Arrange
            var newCollection = new Collection(1, "New Collection", DateOnly.FromDateTime(DateTime.Now), "cover.jpg", true)
            {
                CollectionId = 0 // Simulate new collection
            };
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Verifiable();
            // Act
            collectionsRepository.SaveCollection("1", newCollection);
            // Assert: Verify that ExecuteReader was called with "CreateCollection" once.
            mockDataLink.Verify(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // Dummy assert.
        }

        [Test]
        public void SaveCollection_ExistingCollection_CallsUpdateCollection()
        {
            // Arrange
            var existingCollection = new Collection(1, "Existing Collection", DateOnly.FromDateTime(DateTime.Now), "cover.jpg", false)
            {
                CollectionId = 10 // Non-zero indicates an existing collection.
            };
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Verifiable();
            // Act
            collectionsRepository.SaveCollection("1", existingCollection);
            // Assert: Verify that ExecuteReader was called with "UpdateCollection" once.
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // Dummy assert.
        }

        [Test]
        public void SaveCollection_NewCollection_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange: Testing new collection save operation.
            var newCollection = new Collection(1, "New Collection", DateOnly.FromDateTime(DateTime.Now), "cover.jpg", true)
            {
                CollectionId = 0
            };
            var databaseOperationException = new DatabaseOperationException("Error");
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(databaseOperationException);
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.SaveCollection("1", newCollection));
        }

        #endregion

        #region DeleteCollection Tests

        [Test]
        public void DeleteCollection_ValidInput_CallsExecuteNonQuery()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Verifiable();
            // Act
            collectionsRepository.DeleteCollection(1, 1);
            // Assert: Verify that ExecuteNonQuery was called once.
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // Dummy assert.
        }

        [Test]
        public void DeleteCollection_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlExceptionInstance = CreateSqlException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(sqlExceptionInstance);
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.DeleteCollection(1, 1));
        }

        [Test]
        public void DeleteCollection_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.DeleteCollection(1, 1));
        }

        #endregion

        #region CreateCollection Tests

        [Test]
        public void CreateCollection_ValidInput_CallsExecuteNonQuery()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Verifiable();
            // Act
            collectionsRepository.CreateCollection(1, "New Collection", "cover.jpg", true, DateOnly.FromDateTime(DateTime.Now));
            // Assert: Verify that ExecuteNonQuery was called once.
            mockDataLink.Verify(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // Dummy assert.
        }

        [Test]
        public void CreateCollection_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlExceptionInstance = CreateSqlException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(sqlExceptionInstance);
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.CreateCollection(1, "New Collection", "cover.jpg", true, DateOnly.FromDateTime(DateTime.Now)));
        }

        [Test]
        public void CreateCollection_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.CreateCollection(1, "New Collection", "cover.jpg", true, DateOnly.FromDateTime(DateTime.Now)));
        }

        #endregion

        #region UpdateCollection Tests

        [Test]
        public void UpdateCollection_ValidInput_CallsExecuteReader()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Verifiable();
            // Act
            collectionsRepository.UpdateCollection(1, 1, "Updated Collection", "cover_updated.jpg", false);
            // Assert: Verify that ExecuteReader was called once.
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // Dummy assert.
        }

        [Test]
        public void UpdateCollection_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlExceptionInstance = CreateSqlException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(sqlExceptionInstance);
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.UpdateCollection(1, 1, "Updated Collection", "cover_updated.jpg", false));
        }

        [Test]
        public void UpdateCollection_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.UpdateCollection(1, 1, "Updated Collection", "cover_updated.jpg", false));
        }

        #endregion

        #region GetPublicCollectionsForUser Tests

        [Test]
        public void GetPublicCollectionsForUser_EmptyDataTable_ReturnsEmptyList()
        {
            // Arrange
            DataTable emptyCollectionsTable = CreateEmptyCollectionsDataTable();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(emptyCollectionsTable);
            // Act
            List<Collection> publicCollectionsList = collectionsRepository.GetPublicCollectionsForUser(1);
            // Assert: List is empty.
            Assert.That(publicCollectionsList, Is.Empty);
        }

        [Test]
        public void GetPublicCollectionsForUser_DataTableHasRows_ReturnsCorrectCount()
        {
            // Arrange
            int expectedPublicCollectionsCount = 4;
            DataTable collectionsTable = CreateCollectionsDataTable(expectedPublicCollectionsCount);
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(collectionsTable);
            // Act
            List<Collection> publicCollectionsList = collectionsRepository.GetPublicCollectionsForUser(1);
            // Assert: Count equals expected.
            Assert.That(publicCollectionsList.Count, Is.EqualTo(expectedPublicCollectionsCount));
        }

        [Test]
        public void GetPublicCollectionsForUser_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlExceptionInstance = CreateSqlException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(sqlExceptionInstance);
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.GetPublicCollectionsForUser(1));
        }

        [Test]
        public void GetPublicCollectionsForUser_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.GetPublicCollectionsForUser(1));
        }

        #endregion

        #region GetGamesNotInCollection Tests

        [Test]
        public void GetGamesNotInCollection_EmptyDataTable_ReturnsEmptyList()
        {
            // Arrange: Create an empty DataTable with expected columns.
            DataTable emptyGamesTable = new DataTable();
            emptyGamesTable.Columns.Add("user_id", typeof(int));
            emptyGamesTable.Columns.Add("title", typeof(string));
            emptyGamesTable.Columns.Add("description", typeof(string));
            emptyGamesTable.Columns.Add("cover_picture", typeof(string));
            emptyGamesTable.Columns.Add("game_id", typeof(int));

            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(emptyGamesTable);

            // Act
            List<OwnedGame> gamesNotInCollectionList = collectionsRepository.GetGamesNotInCollection(1, 1);

            // Assert: The returned list should be empty.
            Assert.That(gamesNotInCollectionList, Is.Empty);
        }

        [Test]
        public void GetGamesNotInCollection_DataTableHasRows_ReturnsCorrectCount()
        {
            // Arrange
            int expectedGamesNotInCollectionCount = 3;
            DataTable gamesDataTable = CreateGamesDataTable(expectedGamesNotInCollectionCount);
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(gamesDataTable);
            // Act
            List<OwnedGame> gamesNotInCollectionList = collectionsRepository.GetGamesNotInCollection(1, 1);
            // Assert: Count equals expected.
            Assert.That(gamesNotInCollectionList.Count, Is.EqualTo(expectedGamesNotInCollectionCount));
        }

        [Test]
        public void GetGamesNotInCollection_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlExceptionInstance = CreateSqlException();
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(sqlExceptionInstance);
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.GetGamesNotInCollection(1, 1));
        }

        [Test]
        public void GetGamesNotInCollection_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new Exception("Test error"));
            // Act & Assert
            Assert.Throws<RepositoryException>(() => collectionsRepository.GetGamesNotInCollection(1, 1));
        }

        #endregion
    }
    */
}
