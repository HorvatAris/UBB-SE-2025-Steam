using BusinessLayer.Models;
using NUnit.Framework;

namespace Tests.Models
{
    [TestFixture]
    public class FriendshipPropertyTests
    {
        [Test]
        public void FriendshipId_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var friendship = CreateValidFriendship();

            // Act
            friendship.FriendshipId = 101;

            // Assert
            Assert.That(friendship.FriendshipId, Is.EqualTo(101));
        }

        [Test]
        public void UserId_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var friendship = CreateValidFriendship();

            // Act
            friendship.UserId = 77;

            // Assert
            Assert.That(friendship.UserId, Is.EqualTo(77));
        }

        [Test]
        public void FriendId_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var friendship = CreateValidFriendship();

            // Act
            friendship.FriendId = 88;

            // Assert
            Assert.That(friendship.FriendId, Is.EqualTo(88));
        }

        [Test]
        public void FriendUsername_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var friendship = CreateValidFriendship();

            // Act
            friendship.FriendUsername = "gamer123";

            // Assert
            Assert.That(friendship.FriendUsername, Is.EqualTo("gamer123"));
        }

        [Test]
        public void FriendProfilePicture_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var friendship = CreateValidFriendship();

            // Act
            friendship.FriendProfilePicture = "https://profile.img/user.png";

            // Assert
            Assert.That(friendship.FriendProfilePicture, Is.EqualTo("https://profile.img/user.png"));
        }

        [Test]
        public void Constructor_AssignsFriendshipId_AssignsCorrectly()
        {
            // Arrange
            var expected = 5;
            int userIdentifier = 1;
            int friendIdentifier = 1;

            // Act
            var friendship = new Friendship(expected, userIdentifier, friendIdentifier);

            // Assert
            Assert.That(friendship.FriendshipId, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_AssignsUserId_AssignsCorrectly()
        {
            // Arrange
            var expected = 10;
            int friendshipIdentifier = 1;
            int friendIdentifier = 2;

            // Act
            var friendship = new Friendship(friendshipIdentifier, expected, friendIdentifier);

            // Assert
            Assert.That(friendship.UserId, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_AssignsFriendId_AssignsCorrectly()
        {
            // Arrange
            var expected = 20;
            int userIdentifier = 1;
            int friendshipIdentifier = 1;

            // Act
            var friendship = new Friendship(friendshipIdentifier, userIdentifier, expected);

            // Assert
            Assert.That(friendship.FriendId, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_AssignsFriendUsername_AssignsCorrectly()
        {
            // Arrange
            var expected = "co_op_buddy";
            int userIdentifier = 1;
            int friendshipIdentifier = 1;
            int friendIdentifier = 1;

            // Act
            var friendship = new Friendship(friendshipIdentifier, userIdentifier, friendIdentifier, expected);

            // Assert
            Assert.That(friendship.FriendUsername, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_AssignsFriendProfilePicture_AssignsCorrectly()
        {
            // Arrange
            var expected = "https://img.url/pic.png";
            int userIdentifier = 1;
            int friendshipIdentifier = 1;
            int friendIdentifier = 1;

            // Act
            var friendship = new Friendship(userIdentifier, friendshipIdentifier, friendIdentifier, "user", expected);

            // Assert
            Assert.That(friendship.FriendProfilePicture, Is.EqualTo(expected));
        }

        private Friendship CreateValidFriendship()
        {
            int userIdentifier = 1;
            int friendshipIdentifier = 1;
            int friendIdentifier = 2;
            return new Friendship(friendshipIdentifier, userIdentifier, friendIdentifier, "testuser", "https://img.test/user.png");
        }
    }
}
