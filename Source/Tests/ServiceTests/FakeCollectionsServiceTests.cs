using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Services.Fakes;

namespace Tests.ServiceTests
{
    [TestFixture]
    public class FakeCollectionsServiceTests
    {
        #region GetAllCollections

        [Test]
        public void GetAllCollections_UserIdProvided_ReturnsOnlyUserCollections()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act
            var userCollections = fakeCollectionsService.GetAllCollections(1);

            // Assert: Expect exactly 3 collections for user 1 (seeded data).
            Assert.That(userCollections.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetAllCollections_UserHasNoCollections_ReturnsEmptyList()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act
            var emptyCollections = fakeCollectionsService.GetAllCollections(999);

            // Assert: Expect an empty list for a user with no collections.
            Assert.That(emptyCollections, Is.Empty);
        }

        #endregion

        #region GetCollectionById

        [Test]
        public void GetCollectionById_ValidUserAndCollectionId1_ReturnsCollectionWithGames()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act: For collectionId 1 and user 1, dummy games are attached.
            var collectionWithGames = fakeCollectionsService.GetCollectionByIdentifier(1, 1);

            // Assert: Expect the dummy games list to have exactly 2 games.
            Assert.That(collectionWithGames.Games.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetCollectionById_InvalidUser_ReturnsNull()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act: Request a collection for a non-matching user.
            var collectionForWrongUser = fakeCollectionsService.GetCollectionByIdentifier(1, 999);

            // Assert: Expect null since the collection does not belong to user 999.
            Assert.That(collectionForWrongUser, Is.Null);
        }

        [Test]
        public void GetCollectionById_ValidUserNonFirstCollection_ReturnsCollectionWithoutGames()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act: For a collection other than 1, no dummy games are attached.
            var collectionWithoutGames = fakeCollectionsService.GetCollectionByIdentifier(2, 1);

            // Assert: Expect the games list to remain as-is (seeded fakes do not add games).
            Assert.That(collectionWithoutGames.Games.Count, Is.EqualTo(0));
        }

        #endregion

        #region GetGamesInCollection Overloads

        [Test]
        public void GetGamesInCollection_WithoutUserId_ReturnsEmptyList()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act: Call the overload that does not use userId.
            var gamesWithoutUserContext = fakeCollectionsService.GetGamesInCollection(2);

            // Assert: Expect an empty list.
            Assert.That(gamesWithoutUserContext, Is.Empty);
        }

        [Test]
        public void GetGamesInCollection_WithUserIdAndCollection1_ReturnsGames()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act: For collectionId 1 and user 1, dummy games should be returned.
            var gamesWithUserContext = fakeCollectionsService.GetGamesInCollection(1, 1);

            // Assert: Expect exactly 2 games.
            Assert.That(gamesWithUserContext.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetGamesInCollection_WithUserIdNonFirstCollection_ReturnsEmptyList()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act: For a collection other than 1, falls back to the overload with no user context.
            var gamesForNonFirstCollection = fakeCollectionsService.GetGamesInCollection(2, 1);

            // Assert: Expect an empty list.
            Assert.That(gamesForNonFirstCollection, Is.Empty);
        }

        #endregion

        #region AddGameToCollection, RemoveGameFromCollection, MakeCollectionPrivateForUser, MakeCollectionPublicForUser

        [Test]
        public void AddGameToCollection_ValidInput_DoesNotThrowException()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act & Assert: Verify that adding a game does not throw an exception.
            Assert.DoesNotThrow(() => fakeCollectionsService.AddGameToCollection(1, 10, 1));
        }

        [Test]
        public void RemoveGameFromCollection_ValidInput_DoesNotThrowException()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act & Assert: Verify that removing a game does not throw an exception.
            Assert.DoesNotThrow(() => fakeCollectionsService.RemoveGameFromCollection(1, 10));
        }

        [Test]
        public void MakeCollectionPrivateForUser_ValidInput_DoesNotThrowException()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act & Assert: Verify that making a collection private does not throw.
            Assert.DoesNotThrow(() => fakeCollectionsService.MakeCollectionPrivateForUser("1", "1"));
        }

        [Test]
        public void MakeCollectionPublicForUser_ValidInput_DoesNotThrowException()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act & Assert: Verify that making a collection public does not throw.
            Assert.DoesNotThrow(() => fakeCollectionsService.MakeCollectionPublicForUser("1", "1"));
        }

        #endregion

        #region RemoveCollectionForUser

        [Test]
        public void RemoveCollectionForUser_ValidCollection_RemovesCollection()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act: Remove collection with id 2 for user 1.
            fakeCollectionsService.RemoveCollectionForUser("1", "2");
            var remainingCollections = fakeCollectionsService.GetAllCollections(1);

            // Assert: Expect no collection with CollectionId == 2.
            Assert.That(remainingCollections.Any(remainingCollections => remainingCollections.CollectionId == 2), Is.False);
        }

        [Test]
        public void RemoveCollectionForUser_NonExistingCollection_DoesNothing()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();
            int initialCollectionCount = fakeCollectionsService.GetAllCollections(1).Count;

            // Act: Attempt to remove a non-existing collection.
            fakeCollectionsService.RemoveCollectionForUser("1", "999");
            int finalCollectionCount = fakeCollectionsService.GetAllCollections(1).Count;

            // Assert: Expect the count to remain unchanged.
            Assert.That(finalCollectionCount, Is.EqualTo(initialCollectionCount));
        }

        #endregion

        #region SaveCollection

        [Test]
        public void SaveCollection_NewCollection_AddsNewCollection()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();
            var newCollection = new Collection(1, "New Collection", DateOnly.FromDateTime(DateTime.Now)) { CollectionId = 0 };

            // Act: Save the new collection.
            fakeCollectionsService.SaveCollection("1", newCollection);
            var collectionsAfterSave = fakeCollectionsService.GetAllCollections(1);

            // Assert: Expect a collection with the name "New Collection" to exist.
            Assert.That(collectionsAfterSave.Any(collectionsAfterSave => collectionsAfterSave.CollectionName == "New Collection"), Is.True);
        }

        [Test]
        public void SaveCollection_UpdateExistingCollection_UpdatesCollection()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();
            var existingCollection = fakeCollectionsService.GetAllCollections(1).First();
            existingCollection.CollectionName = "Updated Name";

            // Act: Save (update) the existing collection.
            fakeCollectionsService.SaveCollection("1", existingCollection);
            var updatedCollection = fakeCollectionsService.GetCollectionByIdentifier(existingCollection.CollectionId, 1);

            // Assert: Expect the updated collection's name to be "Updated Name".
            Assert.That(updatedCollection.CollectionName, Is.EqualTo("Updated Name"));
        }

        [Test]
        public void SaveCollection_UpdateNonExistingCollection_DoesNotAddNewCollection()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();
            // Create a collection with a non-existing CollectionId.
            var nonExistingCollection = new Collection(1, "Non Existing", DateOnly.FromDateTime(DateTime.Now)) { CollectionId = 999 };
            int countBeforeUpdate = fakeCollectionsService.GetAllCollections(1).Count;

            // Act: Attempt to update (which will not match any record).
            fakeCollectionsService.SaveCollection("1", nonExistingCollection);
            int countAfterUpdate = fakeCollectionsService.GetAllCollections(1).Count;

            // Assert: Count remains the same.
            Assert.That(countAfterUpdate, Is.EqualTo(countBeforeUpdate));
        }

        #endregion

        #region DeleteCollection

        [Test]
        public void DeleteCollection_ValidUserAndCollection_RemovesCollection()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();
            int initialCount = fakeCollectionsService.GetAllCollections(1).Count;

            // Act: Delete collection with id 1 for user 1.
            fakeCollectionsService.DeleteCollection(1, 1);
            int newCount = fakeCollectionsService.GetAllCollections(1).Count;

            // Assert: Expect the new count to be one less than the initial count.
            Assert.That(newCount, Is.EqualTo(initialCount - 1));
        }

        [Test]
        public void DeleteCollection_NonExistingCollection_DoesNothing()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();
            int countBeforeDelete = fakeCollectionsService.GetAllCollections(1).Count;

            // Act: Attempt to delete a non-existing collection.
            fakeCollectionsService.DeleteCollection(999, 1);
            int countAfterDelete = fakeCollectionsService.GetAllCollections(1).Count;

            // Assert: Expect the count to remain unchanged.
            Assert.That(countAfterDelete, Is.EqualTo(countBeforeDelete));
        }

        #endregion

        #region CreateCollection

        [Test]
        public void CreateCollection_ValidInput_AddsNewCollection()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();
            int initialCollectionCount = fakeCollectionsService.GetAllCollections(1).Count;

            // Act: Create a new collection for user 1.
            fakeCollectionsService.CreateCollection(1, "Created Collection", "coverCreated.jpg", true, DateOnly.FromDateTime(DateTime.Now));
            int newCollectionCount = fakeCollectionsService.GetAllCollections(1).Count;

            // Assert: Expect the new count to be one more than the initial count.
            Assert.That(newCollectionCount, Is.EqualTo(initialCollectionCount + 1));
        }

        #endregion

        #region UpdateCollection

        [Test]
        public void UpdateCollection_ValidInput_UpdatesCollectionProperties()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act: Update collection with id 2 for user 1.
            fakeCollectionsService.UpdateCollection(2, 1, "Updated Collection", "newCover.jpg", false);
            var updatedCollection = fakeCollectionsService.GetCollectionByIdentifier(2, 1);

            // Assert: Expect the collection's name to be "Updated Collection".
            Assert.That(updatedCollection.CollectionName, Is.EqualTo("Updated Collection"));
        }

        #endregion

        #region GetPublicCollectionsForUser

        [Test]
        public void GetPublicCollectionsForUser_ValidUser_ReturnsOnlyPublicCollections()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act: Get public collections for user 1.
            var publicCollections = fakeCollectionsService.GetPublicCollectionsForUser(1);

            // Assert: Expect exactly 2 public collections (from seeded data).
            Assert.That(publicCollections.Count, Is.EqualTo(2));
        }

        #endregion

        #region GetGamesNotInCollection

        [Test]
        public void GetGamesNotInCollection_ValidUserAndCollection_ReturnsExpectedGame()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act: Retrieve games not in collection for user 1.
            var gamesNotInCollection = fakeCollectionsService.GetGamesNotInCollection(5, 1);

            // Assert: Expect at least one game with GameId equal to 3.
            Assert.That(gamesNotInCollection.Any(gamesNotInCollection => gamesNotInCollection.GameId == 3), Is.True);
        }

        #endregion

        #region GetLastThreeCollectionsForUser

        [Test]
        public void GetLastThreeCollectionsForUser_ValidUser_ReturnsLastThreeSortedCollections()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act: Retrieve the last three collections for user 1.
            var lastThreeCollections = fakeCollectionsService.GetLastThreeCollectionsForUser(1);

            // Assert: Expect exactly 3 collections and the first's CreatedAt is not earlier than the second's.
            Assert.That(lastThreeCollections.Count == 3
                && (lastThreeCollections.Count < 2 || lastThreeCollections[0].CreatedAt.CompareTo(lastThreeCollections[1].CreatedAt) >= 0), Is.True);
        }

        [Test]
        public void GetLastThreeCollectionsForUser_UserWithFewerThanThree_ReturnsAllCollections()
        {
            // Arrange
            var fakeCollectionsService = new FakeCollectionsService();

            // Act: For user 2, only one collection exists.
            var userCollections = fakeCollectionsService.GetLastThreeCollectionsForUser(2);

            // Assert: Expect exactly 1 collection.
            Assert.That(userCollections.Count, Is.EqualTo(1));
        }

        #endregion
    }
}
