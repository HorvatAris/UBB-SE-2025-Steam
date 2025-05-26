using System;
using System.Linq;
using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Fakes;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class FakeCollectionsRepositoryTests
    {
        private FakeCollectionsRepository fakeCollectionsRepository;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Instantiate the fake repository (which seeds dummy data).
            fakeCollectionsRepository = new FakeCollectionsRepository();
        }

        [Test]
        public void GetAllCollections_ForUser1_ReturnsThreeCollections()
        {
            // Act: Get all collections for user with ID 1.
            var collectionsForUser1 = fakeCollectionsRepository.GetAllCollections(1);

            // Assert: Expect exactly three collections.
            Assert.That(collectionsForUser1.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetAllCollections_NonExistingUser_ReturnsEmptyList()
        {
            // Act: Get all collections for a user that has none (user ID 999).
            var collectionsForNonExistingUser = fakeCollectionsRepository.GetAllCollections(999);

            // Assert: Expect an empty list.
            Assert.That(collectionsForNonExistingUser, Is.Empty);
        }

        [Test]
        public void GetLastThreeCollectionsForUser_ForUser1_ReturnsThreeCollections()
        {
            // Act: Get the last three collections for user with ID 1.
            var lastThreeCollectionsForUser1 = fakeCollectionsRepository.GetLastThreeCollectionsForUser(1);

            // Assert: Expect exactly three collections.
            Assert.That(lastThreeCollectionsForUser1.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetLastThreeCollectionsForUser_ForUser1OrderedByCreatedAtDescending_ReturnsDescendingOrder()
        {
            // Act: Get the last three collections for user with ID 1.
            var lastThreeCollectionsForUser1 = fakeCollectionsRepository.GetLastThreeCollectionsForUser(1);

            // Assert: The first collection's CreatedAt should be greater than or equal to the second's.
            Assert.That(lastThreeCollectionsForUser1.First().CreatedAt,
                        Is.GreaterThanOrEqualTo(lastThreeCollectionsForUser1.ElementAt(1).CreatedAt));
        }

        [Test]
        public void GetCollectionById_ExistingCollection_ReturnsCorrectCollection()
        {
            // Act: Retrieve the collection with CollectionId 2 for user with ID 1.
            var retrievedCollection = fakeCollectionsRepository.GetCollectionById(2, 1);

            // Assert: The collection's name should be "Test Collection 2".
            Assert.That(retrievedCollection.CollectionName, Is.EqualTo("Test Collection 2"));
        }

        [Test]
        public void GetCollectionById_NonExistingCollection_ReturnsNull()
        {
            // Act: Try to retrieve a collection that does not exist (CollectionId 999 for user with ID 1).
            var retrievedCollection = fakeCollectionsRepository.GetCollectionById(999, 1);

            // Assert: Expect the result to be null.
            Assert.That(retrievedCollection, Is.Null);
        }

        [Test]
        public void GetGamesInCollection_WithoutUserId_ReturnsEmptyList()
        {
            // Act: Call the method for retrieving games in a collection using the single-parameter overload.
            var ownedGamesForCollection = fakeCollectionsRepository.GetGamesInCollection(1);

            // Assert: Should always return an empty list.
            Assert.That(ownedGamesForCollection, Is.Empty);
        }

        [Test]
        public void GetGamesInCollection_WithUserId_CollectionId1_ReturnsTwoGames()
        {
            // Act: Call the overload with userId for collection with ID 1.
            var ownedGamesForUserCollection1 = fakeCollectionsRepository.GetGamesInCollection(1, 1);

            // Assert: Expect exactly two games.
            Assert.That(ownedGamesForUserCollection1.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetGamesInCollection_WithUserId_CollectionIdNot1_ReturnsEmptyList()
        {
            // Act: Call the overload with userId for collection with an ID other than 1 (e.g. 2).
            var ownedGamesForUserCollectionNot1 = fakeCollectionsRepository.GetGamesInCollection(2, 1);

            // Assert: Expect an empty list.
            Assert.That(ownedGamesForUserCollectionNot1, Is.Empty);
        }

        [Test]
        public void AddGameToCollection_ValidInput_DoesNotThrowException()
        {
            // Act & Assert: Calling AddGameToCollection should not throw any exception.
            Assert.DoesNotThrow(() => fakeCollectionsRepository.AddGameToCollection(1, 1, 1));
        }

        [Test]
        public void RemoveGameFromCollection_ValidInput_DoesNotThrowException()
        {
            // Act & Assert: Calling RemoveGameFromCollection should not throw any exception.
            Assert.DoesNotThrow(() => fakeCollectionsRepository.RemoveGameFromCollection(1, 1));
        }

        [Test]
        public void MakeCollectionPrivateForUser_ValidInput_DoesNotThrowException()
        {
            // Act & Assert: Calling MakeCollectionPrivateForUser should not throw any exception.
            Assert.DoesNotThrow(() => fakeCollectionsRepository.MakeCollectionPrivateForUser("1", "1"));
        }

        [Test]
        public void MakeCollectionPublicForUser_ValidInput_DoesNotThrowException()
        {
            // Act & Assert: Calling MakeCollectionPublicForUser should not throw any exception.
            Assert.DoesNotThrow(() => fakeCollectionsRepository.MakeCollectionPublicForUser("1", "1"));
        }

        [Test]
        public void RemoveCollectionForUser_ExistingCollection_RemovesCollection()
        {
            // Arrange: Get the collection count before removal for user 1.
            int collectionCountBeforeRemoval = fakeCollectionsRepository.GetAllCollections(1).Count;

            // Act: Remove the collection with CollectionId "1" for user "1".
            fakeCollectionsRepository.RemoveCollectionForUser("1", "1");

            // Assert: The new count should decrease by one.
            int collectionCountAfterRemoval = fakeCollectionsRepository.GetAllCollections(1).Count;
            Assert.That(collectionCountAfterRemoval, Is.EqualTo(collectionCountBeforeRemoval - 1));
        }

        [Test]
        public void SaveCollection_NewCollection_AddsNewCollection()
        {
            // Arrange: Create a new collection with CollectionId = 0.
            var newCollection = new Collection(1, "New Collection", DateOnly.FromDateTime(DateTime.Now), "new.jpg", true);
            newCollection.CollectionId = 0;
            int collectionCountBeforeAddition = fakeCollectionsRepository.GetAllCollections(1).Count;

            // Act: Save the new collection.
            fakeCollectionsRepository.SaveCollection("1", newCollection);

            // Assert: The collection count should increase by one.
            int collectionCountAfterAddition = fakeCollectionsRepository.GetAllCollections(1).Count;
            Assert.That(collectionCountAfterAddition, Is.EqualTo(collectionCountBeforeAddition + 1));
        }

        [Test]
        public void SaveCollection_ExistingCollection_UpdatesCollection()
        {
            // Arrange: Retrieve an existing collection for user 1.
            var existingCollection = fakeCollectionsRepository.GetCollectionById(2, 1);

            // Act: Change its name and then save the updated collection.
            existingCollection.CollectionName = "Updated Name";
            fakeCollectionsRepository.SaveCollection("1", existingCollection);

            // Assert: The updated collection should reflect the new name.
            var updatedCollection = fakeCollectionsRepository.GetCollectionById(2, 1);
            Assert.That(updatedCollection.CollectionName, Is.EqualTo("Updated Name"));
        }

        [Test]
        public void DeleteCollection_ExistingCollection_RemovesCollection()
        {
            // Arrange: Get the collection count for user 1 before deletion.
            int collectionCountBeforeDeletion = fakeCollectionsRepository.GetAllCollections(1).Count;

            // Act: Delete the collection with CollectionId 2 for user 1.
            fakeCollectionsRepository.DeleteCollection(2, 1);

            // Assert: The collection count should decrease by one.
            int collectionCountAfterDeletion = fakeCollectionsRepository.GetAllCollections(1).Count;
            Assert.That(collectionCountAfterDeletion, Is.EqualTo(collectionCountBeforeDeletion - 1));
        }

        [Test]
        public void CreateCollection_ValidInput_AddsNewCollection()
        {
            // Arrange: Get the current collection count for user 1.
            int collectionCountBeforeCreation = fakeCollectionsRepository.GetAllCollections(1).Count;

            // Act: Create a new collection for user 1.
            fakeCollectionsRepository.CreateCollection(1, "Created Collection", "created.jpg", true, DateOnly.FromDateTime(DateTime.Now));

            // Assert: The collection count should increase by one.
            int collectionCountAfterCreation = fakeCollectionsRepository.GetAllCollections(1).Count;
            Assert.That(collectionCountAfterCreation, Is.EqualTo(collectionCountBeforeCreation + 1));
        }

        [Test]
        public void UpdateCollection_ExistingCollection_UpdatesCollection()
        {
            // Arrange: Retrieve a collection for user 1.
            var collectionToUpdate = fakeCollectionsRepository.GetCollectionById(2, 1);

            // Act: Update its details.
            fakeCollectionsRepository.UpdateCollection(2, 1, "Updated Collection", "updated.jpg", false);

            // Assert: The collection's name should be updated to "Updated Collection".
            var updatedCollection = fakeCollectionsRepository.GetCollectionById(2, 1);
            Assert.That(updatedCollection.CollectionName, Is.EqualTo("Updated Collection"));
        }

        [Test]
        public void GetPublicCollectionsForUser_ForUser1_ReturnsOnlyPublicCollections()
        {
            // Act: Retrieve public collections for user 1.
            var publicCollectionsForUser1 = fakeCollectionsRepository.GetPublicCollectionsForUser(1);

            // Assert: Based on seeded data for user 1, expect two public collections.
            Assert.That(publicCollectionsForUser1.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetGamesNotInCollection_ValidInput_ReturnsDummyGame()
        {
            // Act: Retrieve games not in a collection for user 1.
            var dummyGamesNotInCollection = fakeCollectionsRepository.GetGamesNotInCollection(1, 1);

            // Assert: Expect the list to contain one dummy game.
            Assert.That(dummyGamesNotInCollection.Count, Is.EqualTo(1));
        }
    }
}
