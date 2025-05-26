using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Services.Fakes;

namespace Tests.ServiceTests
{
    [TestFixture]
    public class FakeFriendsServiceTests
    {
        private IFriendsService fakeFriendsService;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Instantiate the fake friends service.
            // (Assuming FakeFriendsService is parameterless or has its own seeded data.)
            fakeFriendsService = new FakeFriendsService();
        }

        #region GetAllFriendships

        [Test]
        public void GetAllFriendships_CurrentUser_ReturnsFriendships()
        {
            // Act: Retrieve all friendships.
            List<Friendship> friendshipList = fakeFriendsService.GetAllFriendships();

            // Assert: Expect at least one friendship to be returned.
            Assert.That(friendshipList.Count, Is.GreaterThan(0));
        }

        [Test]
        public void GetAllFriendships_CurrentUser_SortedByFriendUsername()
        {
            // Act: Retrieve all friendships.
            List<Friendship> friendshipList = fakeFriendsService.GetAllFriendships();

            // Assert: The list should be sorted by FriendUsername in ascending order.
            for (int i = 1; i < friendshipList.Count; i++)
            {
                int compareResult = string.Compare(friendshipList[i - 1].FriendUsername, friendshipList[i].FriendUsername, StringComparison.Ordinal);
                Assert.That(compareResult, Is.LessThanOrEqualTo(0));
            }
        }

        #endregion

        #region RemoveFriend

        [Test]
        public void RemoveFriend_ValidFriendship_RemovesFriendship()
        {
            // Arrange: Get the initial list of friendships.
            List<Friendship> initialFriendships = fakeFriendsService.GetAllFriendships();
            int initialCount = initialFriendships.Count;
            int friendshipIdToRemove = initialFriendships.First().FriendshipId;

            // Act: Remove the selected friendship.
            fakeFriendsService.RemoveFriend(friendshipIdToRemove);
            List<Friendship> updatedFriendships = fakeFriendsService.GetAllFriendships();

            // Assert: Expect the friendship count to have decreased by one.
            Assert.That(updatedFriendships.Count, Is.EqualTo(initialCount - 1));
        }

        [Test]
        public void RemoveFriend_NonExistentFriendship_NoChangeToFriendshipCount()
        {
            // Arrange: Get the initial list of friendships.
            List<Friendship> initialFriendships = fakeFriendsService.GetAllFriendships();
            int initialCount = initialFriendships.Count;
            int nonExistentFriendshipId = -1; // Assume negative ids are never used

            // Act: Attempt to remove a non-existent friendship.
            fakeFriendsService.RemoveFriend(nonExistentFriendshipId);
            List<Friendship> updatedFriendships = fakeFriendsService.GetAllFriendships();

            // Assert: Count should remain the same.
            Assert.That(updatedFriendships.Count, Is.EqualTo(initialCount));
        }

        #endregion

        #region GetFriendshipCount

        [Test]
        public void GetFriendshipCount_CurrentUser_ReturnsCorrectCount()
        {
            // Arrange: Retrieve expected friendship count from all friendships.
            int expectedCount = fakeFriendsService.GetAllFriendships().Count;

            // Act: Get the friendship count for user with id 1.
            int returnedCount = fakeFriendsService.GetFriendshipCount(1);

            // Assert: The returned count matches the expected count.
            Assert.That(returnedCount, Is.EqualTo(expectedCount));
        }

        #endregion

        #region AreUsersFriends

        [Test]
        public void AreUsersFriends_ExistingRelationship_ReturnsTrue()
        {
            // Arrange: Use an existing friendship from the seeded data.
            Friendship existingFriendship = fakeFriendsService.GetAllFriendships().First();
            int userId = existingFriendship.UserId;
            int friendId = existingFriendship.FriendId;

            // Act: Check if the two users are friends.
            bool friendshipExists = fakeFriendsService.AreUsersFriends(userId, friendId);

            // Assert: Expect true for an existing relationship.
            Assert.That(friendshipExists, Is.True);
        }

        [Test]
        public void AreUsersFriends_NonExistingRelationship_ReturnsFalse()
        {
            // Arrange: Define a user and a non-existent friend id.
            int userId = 1;
            int nonExistentFriendId = 999;

            // Act: Check friendship status.
            bool friendshipExists = fakeFriendsService.AreUsersFriends(userId, nonExistentFriendId);

            // Assert: Expect false for a non-existent relationship.
            Assert.That(friendshipExists, Is.False);
        }

        #endregion

        #region GetFriendshipId

        [Test]
        public void GetFriendshipId_ExistingRelationship_ReturnsCorrectId()
        {
            // Arrange: Use an existing friendship.
            Friendship existingFriendship = fakeFriendsService.GetAllFriendships().First();
            int userId = existingFriendship.UserId;
            int friendId = existingFriendship.FriendId;
            int expectedFriendshipId = existingFriendship.FriendshipId;

            // Act: Retrieve the friendship id.
            int? returnedFriendshipId = fakeFriendsService.GetFriendshipIdentifier(userId, friendId);

            // Assert: The returned id equals the expected friendship id.
            Assert.That(returnedFriendshipId, Is.EqualTo(expectedFriendshipId));
        }

        [Test]
        public void GetFriendshipId_NonExistingRelationship_ReturnsNull()
        {
            // Arrange: Use a valid user id and a friend id that does not exist.
            int userId = 1;
            int nonExistentFriendId = 999;

            // Act: Retrieve the friendship id.
            int? returnedFriendshipId = fakeFriendsService.GetFriendshipIdentifier(userId, nonExistentFriendId);

            // Assert: Expect null for a non-existent relationship.
            Assert.That(returnedFriendshipId, Is.Null);
        }

        #endregion

        #region AddFriend

        [Test]
        public void AddFriend_NewFriend_AddsFriendship()
        {
            // Arrange: Get the initial list of friendships.
            List<Friendship> initialFriendships = fakeFriendsService.GetAllFriendships();
            int initialCount = initialFriendships.Count;
            int newFriendId = 100; // Assume friend id 100 is not already a friend

            // Act: Add the new friendship.
            fakeFriendsService.AddFriend(1, newFriendId);
            List<Friendship> updatedFriendships = fakeFriendsService.GetAllFriendships();

            // Assert: The friendship count increased by one.
            Assert.That(updatedFriendships.Count, Is.EqualTo(initialCount + 1));
        }

        [Test]
        public void AddFriend_DuplicateFriendship_ThrowsException()
        {
            // Arrange: Use an existing friendship from the seeded data.
            Friendship existingFriendship = fakeFriendsService.GetAllFriendships().First();
            int userId = existingFriendship.UserId;
            int friendId = existingFriendship.FriendId;

            // Act & Assert: Adding the same friendship should throw an exception.
            var exception = Assert.Throws<Exception>(() => fakeFriendsService.AddFriend(userId, friendId));
            Assert.That(exception.Message, Is.EqualTo("Friendship already exists."));
        }

        #endregion
    }
}
