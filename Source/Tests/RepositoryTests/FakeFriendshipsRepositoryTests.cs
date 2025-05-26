using System.Linq;
using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Fakes;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class FakeFriendshipsRepositoryTests
    {
        private FakeFriendshipsRepository fakeFriendshipsRepository;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Instantiate the fake friendships repository (with seeded dummy data).
            fakeFriendshipsRepository = new FakeFriendshipsRepository();
        }

        [Test]
        public void GetAllFriendships_ForUser1_ReturnsCorrectCount()
        {
            // Act: Retrieve all friendships for user with ID 1.
            var friendshipsForUser1 = fakeFriendshipsRepository.GetAllFriendships(1);
            // Assert: Expect seeded data to have exactly 2 friendships for user 1.
            Assert.That(friendshipsForUser1.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetAllFriendships_OrderByFriendUsername_ReturnsAliceFirst()
        {
            // Act: Retrieve all friendships for user with ID 1.
            var friendshipsForUser1 = fakeFriendshipsRepository.GetAllFriendships(1);
            // Assert: The first friendship should have FriendUsername "Alice" (alphabetically before "Bob").
            Assert.That(friendshipsForUser1.First().FriendUsername, Is.EqualTo("Alice"));
        }

        [Test]
        public void AddFriendship_ForUser1_AddsNewFriendship()
        {
            // Arrange: Retrieve the initial friendship count for user with ID 1.
            int friendshipCountBeforeAddition = fakeFriendshipsRepository.GetFriendshipCount(1);
            // Act: Add a new friendship for user with ID 1 (e.g., with friend with ID 4).
            fakeFriendshipsRepository.AddFriendship(1, 4);
            // Assert: Expect the friendship count to increase by 1.
            int friendshipCountAfterAddition = fakeFriendshipsRepository.GetFriendshipCount(1);
            Assert.That(friendshipCountAfterAddition, Is.EqualTo(friendshipCountBeforeAddition + 1));
        }

        [Test]
        public void AddFriendship_WhenFriendshipAlreadyExists_ThrowsException()
        {
            // Arrange

            // Act & Assert: Adding a duplicate friendship (for user 1 with friend 2) should throw an exception.
            var exception = Assert.Throws<Exception>(() => fakeFriendshipsRepository.AddFriendship(1, 2));
            Assert.That(exception.Message, Is.EqualTo("Friendship already exists."));
        }

        [Test]
        public void GetFriendshipById_ExistingId_ReturnsFriendship()
        {
            // Arrange

            // Act: Retrieve the friendship with ID 1.
            var retrievedFriendship = fakeFriendshipsRepository.GetFriendshipById(1);
            // Assert: The friendship's ID should be 1.
            Assert.That(retrievedFriendship.FriendshipId, Is.EqualTo(1));
        }

        [Test]
        public void GetFriendshipById_NonExistingId_ReturnsNull()
        {
            // Arrange

            // Act: Try to retrieve a friendship with an ID that does not exist (e.g., 999).
            var retrievedFriendship = fakeFriendshipsRepository.GetFriendshipById(999);
            // Assert: Expect null.
            Assert.That(retrievedFriendship, Is.Null);
        }

        [Test]
        public void RemoveFriendship_ExistingFriendship_RemovesSuccessfully()
        {
            // Arrange: Get the friendship count for user with ID 1 before removal.
            int friendshipCountBeforeRemoval = fakeFriendshipsRepository.GetFriendshipCount(1);
            // Act: Remove the friendship with ID 1.
            fakeFriendshipsRepository.RemoveFriendship(1);
            // Assert: The friendship count should decrease by 1.
            int friendshipCountAfterRemoval = fakeFriendshipsRepository.GetFriendshipCount(1);
            Assert.That(friendshipCountAfterRemoval, Is.EqualTo(friendshipCountBeforeRemoval - 1));
        }

        [Test]
        public void GetFriendshipCount_ForUser1_ReturnsCorrectCount()
        {
            // Arrange

            // Act: Retrieve the friendship count for user with ID 1.
            int friendshipCountForUser1 = fakeFriendshipsRepository.GetFriendshipCount(1);
            // Assert: Expect the seeded data to return a count of 2.
            Assert.That(friendshipCountForUser1, Is.EqualTo(2));
        }

        [Test]
        public void GetFriendshipId_ExistingFriendship_ReturnsCorrectId()
        {
            // Arrange

            // Act: Retrieve the friendship ID for user with ID 1 and friend with ID 2.
            int? friendshipIdValue = fakeFriendshipsRepository.GetFriendshipId(1, 2);
            // Assert: For this seeded data, the expected friendship ID is 1.
            Assert.That(friendshipIdValue, Is.EqualTo(1));
        }

        [Test]
        public void GetFriendshipId_NonExistingFriendship_ReturnsNull()
        {
            // Arrange

            // Act: Retrieve the friendship ID for user with ID 1 and a non-existing friend with ID 999.
            int? friendshipIdValue = fakeFriendshipsRepository.GetFriendshipId(1, 999);
            // Assert: Expect null since the friendship does not exist.
            Assert.That(friendshipIdValue, Is.Null);
        }
    }
}
