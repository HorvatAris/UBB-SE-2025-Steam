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
using BusinessLayer.Services.Fakes;

namespace Tests.ServiceTests
{
    [TestFixture]
    public class FriendsServiceTests
    {
        private FakeFriendshipsRepository fakeFriendshipsRepository;
        private FakeUserService fakeUserService;
        private IFriendsService friendsService;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Use the existing fakes for normal flows.
            fakeFriendshipsRepository = new FakeFriendshipsRepository();
            fakeUserService = new FakeUserService();
            friendsService = new FriendsService(fakeFriendshipsRepository, fakeUserService);
        }

        #region Exception Tests using Moq

        [Test]
        public void FriendsServiceConstructor_NullFriendshipRepository_ThrowsArgumentNullException()
        {
            // Arrange

            // Act

            // Assert: A null friendship repository should trigger an ArgumentNullException.
            Assert.Throws<ArgumentNullException>(() => new FriendsService(null, new FakeUserService()));
        }

        [Test]
        public void FriendsServiceConstructor_NullUserService_ThrowsArgumentNullException()
        {
            // Arrange

            // Act

            // Assert: A null user service should trigger an ArgumentNullException.
            Assert.Throws<ArgumentNullException>(() => new FriendsService(new FakeFriendshipsRepository(), null));
        }

        [Test]
        public void GetAllFriendships_RepositoryThrows_ThrowsServiceException()
        {
            // Arrange
            var mockFriendshipsRepository = new Mock<IFriendshipsRepository>();
            mockFriendshipsRepository.Setup(mockFriendshipsRepository => mockFriendshipsRepository.GetAllFriendships(It.IsAny<int>()))
                                     .Throws(new RepositoryException("Repository error"));
            var serviceWithFaultyRepo = new FriendsService(mockFriendshipsRepository.Object, fakeUserService);

            // Act & Assert: Expect a ServiceException with the specified message.
            var exception = Assert.Throws<ServiceException>(() => serviceWithFaultyRepo.GetAllFriendships());
            Assert.That(exception.Message, Is.EqualTo("Error retrieving friendships for user."));
        }

        [Test]
        public void RemoveFriend_RepositoryThrows_ThrowsServiceException()
        {
            // Arrange
            var mockFriendshipsRepository = new Mock<IFriendshipsRepository>();
            mockFriendshipsRepository.Setup(mockFriendshipsRepository => mockFriendshipsRepository.RemoveFriendship(It.IsAny<int>()))
                                     .Throws(new RepositoryException("Repository error"));
            var serviceWithFaultyRepo = new FriendsService(mockFriendshipsRepository.Object, fakeUserService);

            // Act & Assert: Expect a ServiceException with the specified message.
            var exception = Assert.Throws<ServiceException>(() => serviceWithFaultyRepo.RemoveFriend(1));
            Assert.That(exception.Message, Is.EqualTo("Error removing friend."));
        }

        [Test]
        public void GetFriendshipCount_RepositoryThrows_ThrowsServiceException()
        {
            // Arrange
            var mockFriendshipsRepository = new Mock<IFriendshipsRepository>();
            mockFriendshipsRepository.Setup(mockFriendshipsRepository => mockFriendshipsRepository.GetFriendshipCount(It.IsAny<int>()))
                                     .Throws(new RepositoryException("Repository error"));
            var serviceWithFaultyRepo = new FriendsService(mockFriendshipsRepository.Object, fakeUserService);

            // Act & Assert: Expect a ServiceException with the specified message.
            var exception = Assert.Throws<ServiceException>(() => serviceWithFaultyRepo.GetFriendshipCount(1));
            Assert.That(exception.Message, Is.EqualTo("Error retrieving friendship count."));
        }

        [Test]
        public void AreUsersFriends_RepositoryThrows_ThrowsServiceException()
        {
            // Arrange
            var mockFriendshipsRepository = new Mock<IFriendshipsRepository>();
            mockFriendshipsRepository.Setup(mockFriendshipsRepository => mockFriendshipsRepository.GetAllFriendships(It.IsAny<int>()))
                                     .Throws(new RepositoryException("Repository error"));
            var serviceWithFaultyRepo = new FriendsService(mockFriendshipsRepository.Object, fakeUserService);

            // Act & Assert: Expect a ServiceException with the specified message.
            var exception = Assert.Throws<ServiceException>(() => serviceWithFaultyRepo.AreUsersFriends(1, 2));
            Assert.That(exception.Message, Is.EqualTo("Error checking friendship status."));
        }

        [Test]
        public void GetFriendshipId_RepositoryThrows_ThrowsServiceException()
        {
            // Arrange
            var mockFriendshipsRepository = new Mock<IFriendshipsRepository>();
            mockFriendshipsRepository.Setup(mockFriendshipsRepository => mockFriendshipsRepository.GetAllFriendships(It.IsAny<int>()))
                                     .Throws(new RepositoryException("Repository error"));
            var serviceWithFaultyRepo = new FriendsService(mockFriendshipsRepository.Object, fakeUserService);

            // Act & Assert: Expect a ServiceException with the specified message.
            var exception = Assert.Throws<ServiceException>(() => serviceWithFaultyRepo.GetFriendshipIdentifier(1, 2));
            Assert.That(exception.Message, Is.EqualTo("Error retrieving friendship ID."));
        }

        [Test]
        public void AddFriend_RepositoryThrows_ThrowsServiceException()
        {
            // Arrange
            var mockFriendshipsRepository = new Mock<IFriendshipsRepository>();
            mockFriendshipsRepository.Setup(mockFriendshipsRepository => mockFriendshipsRepository.AddFriendship(It.IsAny<int>(), It.IsAny<int>()))
                                     .Throws(new RepositoryException("Repository error"));
            var serviceWithFaultyRepo = new FriendsService(mockFriendshipsRepository.Object, fakeUserService);

            // Act & Assert: Expect a ServiceException with the specified message.
            var exception = Assert.Throws<ServiceException>(() => serviceWithFaultyRepo.AddFriend(1, 2));
            Assert.That(exception.Message, Is.EqualTo("Error adding friend."));
        }

        #endregion

        #region Normal Flow Tests

        [Test]
        public void GetAllFriendships_CurrentUser_ReturnsNonNullFriendshipList()
        {
            // Act: Retrieve all friendships.
            List<Friendship> friendshipList = friendsService.GetAllFriendships();

            // Assert: The result should not be null.
            Assert.That(friendshipList, Is.Not.Null);
        }

        [Test]
        public void GetAllFriendships_CurrentUser_ReturnsNonNegativeCount()
        {
            // Act: Retrieve all friendships.
            List<Friendship> friendshipList = friendsService.GetAllFriendships();

            // Assert: Count is greater than or equal to zero.
            Assert.That(friendshipList.Count, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void GetAllFriendships_CurrentUser_AllFriendshipsHaveUserId1()
        {
            // Act: Retrieve all friendships.
            List<Friendship> friendshipList = friendsService.GetAllFriendships();

            // Assert: Every friendship has UserId equal to 1.
            Assert.That(friendshipList, Has.All.Property("UserId").EqualTo(1));
        }

        [Test]
        public void RemoveFriend_ValidFriendship_RemovesFriendshipAndDecreasesCountByOne()
        {
            // Arrange: Retrieve the initial list of friendships for user with id 1.
            List<Friendship> initialFriendshipList = fakeFriendshipsRepository.GetAllFriendships(1);
            int initialCount = initialFriendshipList.Count;
            Friendship friendshipToRemove = initialFriendshipList.First();

            // Act: Remove the selected friendship.
            friendsService.RemoveFriend(friendshipToRemove.FriendshipId);
            List<Friendship> updatedFriendshipList = fakeFriendshipsRepository.GetAllFriendships(1);

            // Assert: Count should decrease by one.
            Assert.That(updatedFriendshipList.Count, Is.EqualTo(initialCount - 1));
        }

        [Test]
        public void GetFriendshipCount_CurrentUser_ReturnsExpectedCount()
        {
            // Act: Retrieve the friendship count for user with id 1.
            int returnedFriendshipCount = friendsService.GetFriendshipCount(1);

            // Arrange: Get the expected count from the fake repository.
            List<Friendship> allFriendships = fakeFriendshipsRepository.GetAllFriendships(1);
            int expectedFriendshipCount = allFriendships.Count;

            // Assert: The returned count matches the expected count.
            Assert.That(returnedFriendshipCount, Is.EqualTo(expectedFriendshipCount));
        }

        [Test]
        public void AreUsersFriends_Friends_ReturnsTrue()
        {
            // Arrange

            // Act: Check friendship status between user 1 and user 2 (existing friendship).
            bool friendshipStatus = friendsService.AreUsersFriends(1, 2);

            // Assert: The status should be true.
            Assert.That(friendshipStatus, Is.True);
        }

        [Test]
        public void AreUsersFriends_NotFriends_ReturnsFalse()
        {
            // Arrange

            // Act: Check friendship status between user 1 and a non-existing friend id.
            bool friendshipStatus = friendsService.AreUsersFriends(1, 999);

            // Assert: The status should be false.
            Assert.That(friendshipStatus, Is.False);
        }

        [Test]
        public void GetFriendshipId_ExistingRelationship_ReturnsNonNullId()
        {
            // Arrange

            // Act: Retrieve friendship id between user 1 and user 2.
            int? friendshipId = friendsService.GetFriendshipIdentifier(1, 2);

            // Assert: The returned id should not be null.
            Assert.That(friendshipId, Is.Not.Null);
        }

        [Test]
        public void GetFriendshipId_ExistingRelationship_ReturnsExpectedId()
        {
            // Arrange

            // Act: Retrieve friendship id between user 1 and user 2.
            int? friendshipId = friendsService.GetFriendshipIdentifier(1, 2);

            // Assert: The expected id equals 1 (as per the seeded data).
            Assert.That(friendshipId, Is.EqualTo(1));
        }

        [Test]
        public void GetFriendshipId_NonExistingRelationship_ReturnsNull()
        {
            // Arrange

            // Act: Attempt to retrieve friendship id between user 1 and a non-existent friend.
            int? friendshipId = friendsService.GetFriendshipIdentifier(1, 999);

            // Assert: The returned id should be null.
            Assert.That(friendshipId, Is.Null);
        }

        [Test]
        public void AddFriend_NewFriend_AddsFriendshipAndIncreasesCountByOne()
        {
            // Arrange: Retrieve the initial list of friendships for user 1.
            List<Friendship> initialFriendshipList = fakeFriendshipsRepository.GetAllFriendships(1);
            int initialCount = initialFriendshipList.Count;

            // Act: Add a new friend (assume user 4 is not already a friend of user 1).
            friendsService.AddFriend(1, 4);
            List<Friendship> updatedFriendshipList = fakeFriendshipsRepository.GetAllFriendships(1);

            // Assert: Count should increase by one.
            Assert.That(updatedFriendshipList.Count, Is.EqualTo(initialCount + 1));
        }

        #endregion
    }
}
