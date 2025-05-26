using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Repositories.Fakes;

namespace Tests.ServiceTests
{
    // Normal flow tests for CollectionsService using FakeCollectionsRepository.
    [TestFixture]
    public class CollectionsServiceTests
    {
        private ICollectionsService collectionsServiceInstance;
        private FakeCollectionsRepository fakeCollectionsRepositoryInstance;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Instantiate the fake collections repository and the service using it.
            fakeCollectionsRepositoryInstance = new FakeCollectionsRepository();
            collectionsServiceInstance = new CollectionsService(fakeCollectionsRepositoryInstance);
        }

        #region GetAllCollections Normal Flow

        [Test]
        public void GetAllCollections_ForValidUser_ReturnsNonNullList()
        {
            // Act: Retrieve all collections for user with ID 1.
            List<Collection> collectionsForUser1 = collectionsServiceInstance.GetAllCollections(1);
            // Assert: The returned list should not be null.
            Assert.That(collectionsForUser1, Is.Not.Null);
        }

        [Test]
        public void GetAllCollections_ForValidUser_ReturnsCountAtLeastThree()
        {
            // Act: Retrieve all collections for user with ID 1.
            List<Collection> collectionsForUser1 = collectionsServiceInstance.GetAllCollections(1);
            // Assert: The count of collections should be at least 3.
            Assert.That(collectionsForUser1.Count, Is.GreaterThanOrEqualTo(3));
        }

        [Test]
        public void GetAllCollections_ForValidUser_AllCollectionsHaveUserIdOne()
        {
            // Act: Retrieve all collections for user with ID 1.
            List<Collection> collectionsForUser1 = collectionsServiceInstance.GetAllCollections(1);
            // Assert: Every collection's UserId should equal 1.
            Assert.That(collectionsForUser1, Has.All.Property("UserId").EqualTo(1));
        }

        #endregion

        #region GetCollectionById Normal Flow

        [Test]
        public void GetCollectionById_ForExistingCollection_ReturnsNonNullCollection()
        {
            // Arrange: Retrieve seeded collection list and pick the first collection's ID.
            List<Collection> seededCollectionsForUser1 = fakeCollectionsRepositoryInstance.GetAllCollections(1);
            int existingCollectionId = seededCollectionsForUser1[0].CollectionId;
            // Act: Retrieve the collection by its ID for user 1.
            Collection retrievedCollection = collectionsServiceInstance.GetCollectionByIdentifier(existingCollectionId, 1);
            // Assert: The returned collection should not be null.
            Assert.That(retrievedCollection, Is.Not.Null);
        }

        [Test]
        public void GetCollectionById_ForExistingCollection_ReturnsExpectedCollectionId()
        {
            // Arrange: Retrieve seeded collections and get the ID of the first collection.
            List<Collection> seededCollectionsForUser1 = fakeCollectionsRepositoryInstance.GetAllCollections(1);
            int expectedCollectionId = seededCollectionsForUser1[0].CollectionId;
            // Act: Retrieve the collection by its ID for user 1.
            Collection retrievedCollection = collectionsServiceInstance.GetCollectionByIdentifier(expectedCollectionId, 1);
            // Assert: The returned collection's CollectionId should equal the expected ID.
            Assert.That(retrievedCollection.CollectionId, Is.EqualTo(expectedCollectionId));
        }

        [Test]
        public void GetCollectionById_ForExistingCollection_ReturnsCollectionWithGamesLoaded()
        {
            // Arrange: Retrieve seeded collections and choose the first collection.
            List<Collection> seededCollectionsForUser1 = fakeCollectionsRepositoryInstance.GetAllCollections(1);
            int selectedCollectionId = seededCollectionsForUser1[0].CollectionId;
            // Act: Retrieve the collection by its ID.
            Collection retrievedCollection = collectionsServiceInstance.GetCollectionByIdentifier(selectedCollectionId, 1);
            // Assert: The Games property of the collection should not be null.
            Assert.That(retrievedCollection.Games, Is.Not.Null);
        }

        #endregion

        #region GetGamesInCollection Normal Flow

        [Test]
        public void GetGamesInCollection_ForValidCollection_ReturnsNonNullList()
        {
            // Act: Retrieve the list of owned games in a collection for user 1.
            List<OwnedGame> ownedGamesForCollection = collectionsServiceInstance.GetGamesInCollection(1);
            // Assert: The returned list should not be null.
            Assert.That(ownedGamesForCollection, Is.Not.Null);
        }

        [Test]
        public void GetGamesInCollection_ForValidCollection_ReturnsExactCountZero()
        {
            // Act: Retrieve the owned games list for collection 1 (per fake repository design).
            List<OwnedGame> ownedGamesForCollection = collectionsServiceInstance.GetGamesInCollection(1);
            // Assert: The count should be exactly 0.
            Assert.That(ownedGamesForCollection.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetGamesInCollection_ForValidCollection_AllGamesHaveUserIdOne()
        {
            // Act: Retrieve the owned games list for collection 1.
            List<OwnedGame> ownedGamesForCollection = collectionsServiceInstance.GetGamesInCollection(1);
            // Assert: Every owned game in the list should have UserId equal to 1.
            Assert.That(ownedGamesForCollection, Has.All.Property("UserId").EqualTo(1));
        }

        #endregion

        #region AddGameToCollection Normal Flow

        [Test]
        public void AddGameToCollection_ForValidInput_DoesNotThrowException()
        {
            // Act & Assert: Adding a game to a collection should not throw an exception.
            Assert.DoesNotThrow(() => collectionsServiceInstance.AddGameToCollection(1, 10, 1));
        }

        #endregion

        #region RemoveGameFromCollection Normal Flow

        [Test]
        public void RemoveGameFromCollection_ForValidInput_DoesNotThrowException()
        {
            // Act & Assert: Removing a game from a collection should not throw an exception.
            Assert.DoesNotThrow(() => collectionsServiceInstance.RemoveGameFromCollection(1, 10));
        }

        #endregion

        #region DeleteCollection Normal Flow

        [Test]
        public void DeleteCollection_ForExistingCollection_RemovesCollectionSuccessfully()
        {
            // Arrange: Create a new collection to later delete.
            collectionsServiceInstance.CreateCollection(1, "Service Delete Test", "test.jpg", true, DateOnly.FromDateTime(DateTime.Now));
            List<Collection> seededCollectionsForUser1 = fakeCollectionsRepositoryInstance.GetAllCollections(1);
            Collection collectionToDelete = seededCollectionsForUser1.FirstOrDefault(collectionToDelete => collectionToDelete.CollectionName == "Service Delete Test");
            // Act: Delete the specified collection.
            collectionsServiceInstance.DeleteCollection(collectionToDelete.CollectionId, 1);
            // Assert: Verify that the collection no longer exists.
            List<Collection> collectionsAfterDeletion = fakeCollectionsRepositoryInstance.GetAllCollections(1);
            bool collectionExists = collectionsAfterDeletion.Any(collectionExists => collectionExists.CollectionId == collectionToDelete.CollectionId);
            Assert.That(collectionExists, Is.False);
        }

        #endregion

        #region CreateCollection Normal Flow

        [Test]
        public void CreateCollection_ForNewCollection_IncreasesCollectionCountByOne()
        {
            // Arrange: Retrieve the initial count of collections for user 1.
            int initialCollectionCount = fakeCollectionsRepositoryInstance.GetAllCollections(1).Count;
            // Act: Create a new collection.
            collectionsServiceInstance.CreateCollection(1, "Service Create Test", "cover.jpg", false, DateOnly.FromDateTime(DateTime.Now));
            int newCollectionCount = fakeCollectionsRepositoryInstance.GetAllCollections(1).Count;
            // Assert: The collection count should increase by one.
            Assert.That(newCollectionCount, Is.EqualTo(initialCollectionCount + 1));
        }

        #endregion

        #region UpdateCollection Normal Flow

        [Test]
        public void UpdateCollection_ForExistingCollection_UpdatesPropertiesCorrectly()
        {
            // Arrange: Retrieve an existing collection and prepare updated properties.
            List<Collection> seededCollectionsForUser1 = fakeCollectionsRepositoryInstance.GetAllCollections(1);
            Collection originalCollection = new Collection(1, seededCollectionsForUser1[0].CollectionName, seededCollectionsForUser1[0].CreatedAt, seededCollectionsForUser1[0].CoverPicture, seededCollectionsForUser1[0].IsPublic)
            {
                CollectionId = seededCollectionsForUser1[0].CollectionId
            };
            int collectionIdToUpdate = originalCollection.CollectionId;
            string updatedCollectionName = "Updated Service Name";
            string updatedCoverPicture = "newcover.jpg";
            bool updatedVisibilitySetting = !originalCollection.IsPublic;
            // Act: Update the collection.
            collectionsServiceInstance.UpdateCollection(collectionIdToUpdate, 1, updatedCollectionName, updatedCoverPicture, updatedVisibilitySetting);
            Collection updatedCollection = fakeCollectionsRepositoryInstance.GetCollectionById(collectionIdToUpdate, 1);
            // Assert: The collection's properties should match the updated values.
            Assert.That(updatedCollection, Has.Property("CollectionName").EqualTo(updatedCollectionName)
                                             .And.Property("CoverPicture").EqualTo(updatedCoverPicture)
                                             .And.Property("IsPublic").EqualTo(updatedVisibilitySetting));
        }

        #endregion

        #region GetPublicCollectionsForUser Normal Flow

        [Test]
        public void GetPublicCollectionsForUser_ForValidUser_ReturnsAllCollectionsAsPublic()
        {
            // Act: Retrieve public collections for user 1.
            List<Collection> publicCollectionsForUser1 = collectionsServiceInstance.GetPublicCollectionsForUser(1);
            // Assert: Every returned collection should have IsPublic equal to true.
            Assert.That(publicCollectionsForUser1, Has.All.Property("IsPublic").EqualTo(true));
        }

        #endregion

        #region GetGamesNotInCollection Normal Flow

        [Test]
        public void GetGamesNotInCollection_ForValidInput_ReturnsExpectedOwnedGamesList()
        {
            // Arrange: Prepare an expected list of owned games.
            List<OwnedGame> expectedOwnedGamesList = new List<OwnedGame>
            {
                new OwnedGame(1, "Title1", "Description1", "cover1.jpg") { GameId = 1 },
                new OwnedGame(1, "Title2", "Description2", "cover2.jpg") { GameId = 2 }
            };
            var collectionsRepositoryMock = new Mock<ICollectionsRepository>();
            collectionsRepositoryMock.Setup(repository => repository.GetGamesNotInCollection(It.IsAny<int>(), It.IsAny<int>()))
                                     .Returns(expectedOwnedGamesList);
            ICollectionsService collectionsServiceMockInstance = new CollectionsService(collectionsRepositoryMock.Object);
            // Act: Retrieve the owned games not in a collection.
            List<OwnedGame> actualOwnedGamesList = collectionsServiceMockInstance.GetGamesNotInCollection(1, 1);
            // Assert: The returned list should exactly equal the expected list.
            Assert.That(actualOwnedGamesList, Is.EqualTo(expectedOwnedGamesList));
        }

        #endregion
    }

    // Exception tests for CollectionsService using Moq.
    [TestFixture]
    public class CollectionsServiceExceptionTests
    {
        #region Constructor Exception Tests

        [Test]
        public void CollectionsService_Constructor_NullRepository_ThrowsArgumentNullException()
        {
            // Assert: Instantiating CollectionsService with a null repository should throw an ArgumentNullException.
            Assert.Throws<ArgumentNullException>(() => new CollectionsService(null));
        }

        #endregion

        #region GetAllCollections Exception Tests

        [Test]
        public void GetAllCollections_WhenRepositoryThrowsRepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Setup the repository mock to throw a RepositoryException.
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.GetAllCollections(It.IsAny<int>()))
                                             .Throws(new RepositoryException("Repo error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert: A ServiceException should be thrown with the expected message.
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.GetAllCollections(1)).Message,
                        Is.EqualTo("Failed to retrieve collections from database"));
        }

        [Test]
        public void GetAllCollections_WhenRepositoryThrowsGenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Setup the repository mock to throw a generic exception.
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.GetAllCollections(It.IsAny<int>()))
                                             .Throws(new Exception("Generic error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert: A ServiceException should be thrown with the expected message.
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.GetAllCollections(1)).Message,
                        Is.EqualTo("An unexpected error occurred while retrieving collections"));
        }

        #endregion

        #region GetCollectionById Exception Tests

        [Test]
        public void GetCollectionById_WhenRepositoryThrowsRepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Setup the repository mock to throw a RepositoryException on GetCollectionById.
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.GetCollectionById(It.IsAny<int>(), It.IsAny<int>()))
                                             .Throws(new RepositoryException("Repo error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.GetCollectionByIdentifier(1, 1)).Message,
                        Is.EqualTo("Failed to retrieve collection."));
        }

        [Test]
        public void GetCollectionById_WhenRepositoryThrowsGenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Setup the repository mock to throw a generic exception.
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.GetCollectionById(It.IsAny<int>(), It.IsAny<int>()))
                                             .Throws(new Exception("Generic error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.GetCollectionByIdentifier(1, 1)).Message,
                        Is.EqualTo("An unexpected error occurred while retrieving collection."));
        }

        [Test]
        public void GetCollectionById_WhenGetGamesInCollectionRepositoryThrowsRepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Prepare a test collection and setup the repository mock so that GetGamesInCollection throws a RepositoryException.
            var testCollectionInstance = new Collection(1, "Test", DateOnly.FromDateTime(DateTime.Now)) { CollectionId = 1 };
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.GetCollectionById(It.IsAny<int>(), It.IsAny<int>()))
                                             .Returns(testCollectionInstance);
            collectionsRepositoryMockInstance.Setup(repository => repository.GetGamesInCollection(It.IsAny<int>(), It.IsAny<int>()))
                                             .Throws(new RepositoryException("Repo error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.GetCollectionByIdentifier(1, 1)).Message,
                        Is.EqualTo("Failed to retrieve collection."));
        }

        [Test]
        public void GetCollectionById_WhenGetGamesInCollectionThrowsGenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Prepare a test collection and setup the repository mock so that GetGamesInCollection throws a generic exception.
            var testCollectionInstance = new Collection(1, "Test", DateOnly.FromDateTime(DateTime.Now)) { CollectionId = 1 };
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.GetCollectionById(It.IsAny<int>(), It.IsAny<int>()))
                                             .Returns(testCollectionInstance);
            collectionsRepositoryMockInstance.Setup(repository => repository.GetGamesInCollection(It.IsAny<int>(), It.IsAny<int>()))
                                             .Throws(new Exception("Generic error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.GetCollectionByIdentifier(1, 1)).Message,
                        Is.EqualTo("An unexpected error occurred while retrieving collection."));
        }

        #endregion

        #region GetGamesInCollection Exception Tests

        [Test]
        public void GetGamesInCollection_WhenRepositoryThrowsRepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Setup the repository mock to throw a RepositoryException on GetGamesInCollection.
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.GetGamesInCollection(It.IsAny<int>()))
                                             .Throws(new RepositoryException("Repo error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.GetGamesInCollection(1)).Message,
                        Is.EqualTo("Failed to retrieve games from database"));
        }

        [Test]
        public void GetGamesInCollection_WhenRepositoryThrowsGenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Setup the repository mock to throw a generic exception on GetGamesInCollection.
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.GetGamesInCollection(It.IsAny<int>()))
                                             .Throws(new Exception("Generic error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.GetGamesInCollection(1)).Message,
                        Is.EqualTo("An unexpected error occurred while retrieving games"));
        }

        #endregion

        #region AddGameToCollection Exception Tests

        [Test]
        public void AddGameToCollection_WhenRepositoryThrowsRepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Setup the repository mock to throw a RepositoryException on AddGameToCollection.
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.AddGameToCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                                             .Throws(new RepositoryException("Repo error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.AddGameToCollection(1, 10, 1)).Message,
                        Is.EqualTo("Failed to add game to collection"));
        }

        [Test]
        public void AddGameToCollection_WhenRepositoryThrowsGenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Setup the repository mock to throw a generic exception on AddGameToCollection.
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.AddGameToCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                                             .Throws(new Exception("Generic error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.AddGameToCollection(1, 10, 1)).Message,
                        Is.EqualTo("An unexpected error occurred"));
        }

        #endregion

        #region RemoveGameFromCollection Exception Tests

        [Test]
        public void RemoveGameFromCollection_WhenRepositoryThrowsRepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Setup the repository mock to throw a RepositoryException on RemoveGameFromCollection.
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.RemoveGameFromCollection(It.IsAny<int>(), It.IsAny<int>()))
                                             .Throws(new RepositoryException("Repo error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.RemoveGameFromCollection(1, 10)).Message,
                        Is.EqualTo("Failed to remove game from collection."));
        }

        [Test]
        public void RemoveGameFromCollection_WhenRepositoryThrowsGenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Setup the repository mock to throw a generic exception on RemoveGameFromCollection.
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.RemoveGameFromCollection(It.IsAny<int>(), It.IsAny<int>()))
                                             .Throws(new Exception("Generic error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.RemoveGameFromCollection(1, 10)).Message,
                        Is.EqualTo("An unexpected error occurred while removing game from collection."));
        }

        #endregion

        #region DeleteCollection Exception Tests

        [Test]
        public void DeleteCollection_WhenRepositoryThrowsGenericException_ThrowsExceptionWithExpectedMessage()
        {
            // Arrange: Setup the repository mock to throw a generic exception on DeleteCollection.
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.DeleteCollection(It.IsAny<int>(), It.IsAny<int>()))
                                             .Throws(new Exception("Generic error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert: The exception message should match the expected text.
            Assert.That(Assert.Throws<Exception>(() => collectionsServiceExceptionInstance.DeleteCollection(1, 1)).Message,
                        Is.EqualTo("Failed to delete collection"));
        }

        #endregion

        #region CreateCollection Exception Tests

        [Test]
        public void CreateCollection_WhenRepositoryThrowsRepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Setup the repository mock to throw a RepositoryException on CreateCollection.
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.CreateCollection(
                                                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<DateOnly>()))
                                             .Throws(new RepositoryException("Repo error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.CreateCollection(1, "Test", "cover.jpg", true, DateOnly.FromDateTime(DateTime.Now))).Message,
                        Is.EqualTo("Failed to create collection in database"));
        }

        [Test]
        public void CreateCollection_WhenRepositoryThrowsGenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Setup the repository mock to throw a generic exception on CreateCollection.
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.CreateCollection(
                                                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<DateOnly>()))
                                             .Throws(new Exception("Generic error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.CreateCollection(1, "Test", "cover.jpg", true, DateOnly.FromDateTime(DateTime.Now))).Message,
                        Is.EqualTo("An unexpected error occurred while creating collection"));
        }

        #endregion

        #region UpdateCollection Exception Tests

        [Test]
        public void UpdateCollection_WhenRepositoryThrowsRepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Setup the repository mock to throw a RepositoryException on UpdateCollection.
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.UpdateCollection(
                                                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                             .Throws(new RepositoryException("Repo error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.UpdateCollection(1, 1, "New Name", "newcover.jpg", true)).Message,
                        Is.EqualTo("Failed to update collection in database"));
        }

        [Test]
        public void UpdateCollection_WhenRepositoryThrowsGenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Setup the repository mock to throw a generic exception on UpdateCollection.
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.UpdateCollection(
                                                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                                             .Throws(new Exception("Generic error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.UpdateCollection(1, 1, "New Name", "newcover.jpg", true)).Message,
                        Is.EqualTo("An unexpected error occurred while updating collection"));
        }

        #endregion

        #region GetPublicCollectionsForUser Exception Tests

        [Test]
        public void GetPublicCollectionsForUser_WhenRepositoryThrowsRepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Setup the repository mock to throw a RepositoryException on GetPublicCollectionsForUser.
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.GetPublicCollectionsForUser(It.IsAny<int>()))
                                             .Throws(new RepositoryException("Repo error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.GetPublicCollectionsForUser(1)).Message,
                        Is.EqualTo("Failed to retrieve public collections from database"));
        }

        [Test]
        public void GetPublicCollectionsForUser_WhenRepositoryThrowsGenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange: Setup the repository mock to throw a generic exception on GetPublicCollectionsForUser.
            var collectionsRepositoryMockInstance = new Mock<ICollectionsRepository>();
            collectionsRepositoryMockInstance.Setup(repository => repository.GetPublicCollectionsForUser(It.IsAny<int>()))
                                             .Throws(new Exception("Generic error"));
            ICollectionsService collectionsServiceExceptionInstance = new CollectionsService(collectionsRepositoryMockInstance.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => collectionsServiceExceptionInstance.GetPublicCollectionsForUser(1)).Message,
                        Is.EqualTo("An unexpected error occurred while retrieving public collections"));
        }

        #endregion
    }
}
