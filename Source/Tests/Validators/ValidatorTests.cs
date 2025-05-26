using System.ComponentModel.DataAnnotations;
using System;
using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Validators;

namespace Tests.Validators
{
    [TestFixture]
    public class ValidatorTests
    {
        // --- CollectionValidator Tests ---
        [Test]
        public void ValidateCollection_ValidCollection_DoesNotThrow()
        {
            // Arrange
            var collection = new Collection(
                userId: 1,
                collectionName: "My Collection",
                createdAt: DateOnly.FromDateTime(DateTime.Today),
                coverPicture: "http://example.com/pic.jpg",
                isPublic: true);
            // Act & Assert
            Assert.That(() => CollectionValidator.ValidateCollection(collection), Throws.Nothing);
        }

        [Test]
        public void ValidateCollection_InvalidUserId_ThrowsException()
        {
            // Arrange
            var collection = new Collection(
                userId: 0,
                collectionName: "My Collection",
                createdAt: DateOnly.FromDateTime(DateTime.Today));
            // Act & Assert
            Assert.That(() => CollectionValidator.ValidateCollection(collection),
                Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void ValidateCollection_TooLongName_ThrowsException()
        {
            // Arrange
            var collection = new Collection(
                userId: 1,
                collectionName: new string('x', 101),
                createdAt: DateOnly.FromDateTime(DateTime.Today));
            // Act & Assert
            Assert.That(() => CollectionValidator.ValidateCollection(collection),
                Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void ValidateCollection_TooLongCoverPicture_ThrowsException()
        {
            // Arrange
            var collection = new Collection(
                userId: 1,
                collectionName: "Valid",
                createdAt: DateOnly.FromDateTime(DateTime.Today),
                coverPicture: new string('x', 256));
            // Act & Assert
            Assert.That(() => CollectionValidator.ValidateCollection(collection),
                Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void IsUserIdValid_Collection_PositiveId_ReturnsTrue()
        {
            // Arrange & Act
            var result = CollectionValidator.IsUserIdValid(5);
            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsUserIdValid_Collection_ZeroId_ReturnsFalse()
        {
            // Arrange & Act
            var result = CollectionValidator.IsUserIdValid(0);
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsUserIdValid_Collection_NegativeId_ReturnsFalse()
        {
            // Arrange & Act
            var result = CollectionValidator.IsUserIdValid(-1);
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsNameValid_ValidName_ReturnsTrue()
        {
            // Arrange & Act
            var result = CollectionValidator.IsNameValid("Test Collection");
            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsNameValid_EmptyName_ReturnsFalse()
        {
            // Arrange & Act
            var result = CollectionValidator.IsNameValid(string.Empty);
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsNameValid_NullName_ReturnsFalse()
        {
            // Arrange & Act
            var result = CollectionValidator.IsNameValid(null);
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsNameValid_WhitespaceName_ReturnsFalse()
        {
            // Arrange & Act
            var result = CollectionValidator.IsNameValid("   ");
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCoverPictureValid_NullCoverPicture_ReturnsTrue()
        {
            // Arrange & Act
            var result = CollectionValidator.IsCoverPictureValid(null);
            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCoverPictureValid_ValidCoverPicture_ReturnsTrue()
        {
            // Arrange & Act
            var result = CollectionValidator.IsCoverPictureValid("http://example.com/image.jpg");
            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCoverPictureValid_TooLongCoverPicture_ReturnsFalse()
        {
            // Arrange & Act
            var result = CollectionValidator.IsCoverPictureValid(new string('x', 256));
            // Assert
            Assert.That(result, Is.False);
        }

        // --- FriendshipValidator Tests ---
        [Test]
        public void ValidateFriendship_ValidFriendship_DoesNotThrow()
        {
            // Arrange
            var friendship = new Friendship(
                friendshipId: 1,
                userId: 1,
                friendId: 2,
                friendUsername: "FriendUser",
                friendProfilePicture: "http://example.com/pfp.jpg");
            // Act & Assert
            Assert.That(() => FriendshipValidator.ValidateFriendship(friendship), Throws.Nothing);
        }

        [Test]
        public void ValidateFriendship_InvalidUserId_ThrowsException()
        {
            // Arrange
            var friendship = new Friendship(
                friendshipId: 1,
                userId: 0,
                friendId: 2);
            // Act & Assert
            Assert.That(() => FriendshipValidator.ValidateFriendship(friendship),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ValidateFriendship_InvalidFriendId_ThrowsException()
        {
            // Arrange
            var friendship = new Friendship(
                friendshipId: 1,
                userId: 1,
                friendId: 0);
            // Act & Assert
            Assert.That(() => FriendshipValidator.ValidateFriendship(friendship),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ValidateFriendship_SelfFriendship_ThrowsException()
        {
            // Arrange
            var friendship = new Friendship(
                friendshipId: 1,
                userId: 1,
                friendId: 1);
            // Act & Assert
            Assert.That(() => FriendshipValidator.ValidateFriendship(friendship),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void IsFriendIdValid_PositiveId_ReturnsTrue()
        {
            // Arrange & Act
            var result = FriendshipValidator.IsFriendIdValid(5);
            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsFriendIdValid_ZeroId_ReturnsFalse()
        {
            // Arrange & Act
            var result = FriendshipValidator.IsFriendIdValid(0);
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsFriendIdValid_NegativeId_ReturnsFalse()
        {
            // Arrange & Act
            var result = FriendshipValidator.IsFriendIdValid(-1);
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsNotSelfFriendship_DifferentIds_ReturnsTrue()
        {
            // Arrange & Act
            var result = FriendshipValidator.IsNotSelfFriendship(1, 2);
            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsNotSelfFriendship_SameIds_ReturnsFalse()
        {
            // Arrange & Act
            var result = FriendshipValidator.IsNotSelfFriendship(1, 1);
            // Assert
            Assert.That(result, Is.False);
        }

        // --- OwnedGameValidator Tests ---
        [Test]
        public void ValidateOwnedGame_ValidGame_DoesNotThrow()
        {
            // Arrange
            var ownedGame = new OwnedGame(
                userId: 1,
                gameTitile: "Elden Ring",
                description: "Awesome RPG",
                coverPicture: null);
            // Act & Assert
            Assert.That(() => OwnedGameValidator.ValidateOwnedGame(ownedGame), Throws.Nothing);
        }

        [Test]
        public void ValidateOwnedGame_InvalidUserId_ThrowsException()
        {
            // Arrange
            var ownedGame = new OwnedGame(
                userId: 0,
                gameTitile: "Valid Game",
                description: "Test Game");
            // Act & Assert
            Assert.That(() => OwnedGameValidator.ValidateOwnedGame(ownedGame),
                Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void ValidateOwnedGame_InvalidTitle_ThrowsException()
        {
            // Arrange
            var ownedGame = new OwnedGame(
                userId: 1,
                gameTitile: string.Empty,
                description: "Test Game");
            // Act & Assert
            Assert.That(() => OwnedGameValidator.ValidateOwnedGame(ownedGame),
                Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void ValidateOwnedGame_TooLongCoverPicture_ThrowsException()
        {
            // Arrange
            var ownedGame = new OwnedGame(
                userId: 1,
                gameTitile: "Valid Game",
                description: "Great Game",
                coverPicture: new string('x', 256));
            // Act & Assert
            Assert.That(() => OwnedGameValidator.ValidateOwnedGame(ownedGame),
                Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void IsUserIdValid_OwnedGame_PositiveId_ReturnsTrue()
        {
            // Arrange & Act
            var result = OwnedGameValidator.IsUserIdValid(5);
            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsUserIdValid_OwnedGame_ZeroId_ReturnsFalse()
        {
            // Arrange & Act
            var result = OwnedGameValidator.IsUserIdValid(0);
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsUserIdValid_OwnedGame_NegativeId_ReturnsFalse()
        {
            // Arrange & Act
            var result = OwnedGameValidator.IsUserIdValid(-1);
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsTitleValid_ValidTitle_ReturnsTrue()
        {
            // Arrange & Act
            var result = OwnedGameValidator.IsTitleValid("Game Title");
            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsTitleValid_EmptyTitle_ReturnsFalse()
        {
            // Arrange & Act
            var result = OwnedGameValidator.IsTitleValid(string.Empty);
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsTitleValid_NullTitle_ReturnsFalse()
        {
            // Arrange & Act
            var result = OwnedGameValidator.IsTitleValid(null);
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsTitleValid_WhitespaceTitle_ReturnsFalse()
        {
            // Arrange & Act
            var result = OwnedGameValidator.IsTitleValid("   ");
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsTitleValid_TooLongTitle_ReturnsFalse()
        {
            // Arrange & Act
            var result = OwnedGameValidator.IsTitleValid(new string('x', 101));
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsCoverPictureValid_OwnedGame_NullCoverPicture_ReturnsTrue()
        {
            // Arrange & Act
            var result = OwnedGameValidator.IsCoverPictureValid(null);
            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCoverPictureValid_OwnedGame_ValidCoverPicture_ReturnsTrue()
        {
            // Arrange & Act
            var result = OwnedGameValidator.IsCoverPictureValid("http://example.com/game.jpg");
            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsCoverPictureValid_OwnedGame_TooLongCoverPicture_ReturnsFalse()
        {
            // Arrange & Act
            var result = OwnedGameValidator.IsCoverPictureValid(new string('x', 256));
            // Assert
            Assert.That(result, Is.False);
        }

        // --- UserValidator Tests ---
        [Test]
        public void ValidateUser_ValidUser_DoesNotThrow()
        {
            // Arrange
            var user = new User
            {
                Username = "Valid User",
                Password = "ValidPass1!",
                Email = "test@example.com"
            };
            // Act & Assert
            Assert.That(() => UserValidator.ValidateUser(user), Throws.Nothing);
        }

        [Test]
        public void ValidateUser_InvalidPassword_ThrowsException()
        {
            // Arrange
            var user = new User
            {
                Username = "Valid User",
                Password = "short",
                Email = "test@example.com"
            };
            // Act & Assert
            Assert.That(() => UserValidator.ValidateUser(user),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ValidateUser_InvalidEmail_ThrowsException()
        {
            // Arrange
            var user = new User
            {
                Username = "Valid User",
                Password = "StrongPass1!",
                Email = "invalidemail@"
            };
            // Act & Assert
            Assert.That(() => UserValidator.ValidateUser(user),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ValidateUser_InvalidUsername_ThrowsException()
        {
            // Arrange
            var user = new User
            {
                Username = string.Empty,
                Password = "StrongPass1!",
                Email = "test@example.com"
            };
            // Act & Assert
            Assert.That(() => UserValidator.ValidateUser(user),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void IsPasswordValid_ValidPassword_ReturnsTrue()
        {
            // Arrange & Act
            var result = UserValidator.IsPasswordValid("ValidPass1!");
            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsPasswordValid_TooShortPassword_ReturnsFalse()
        {
            // Arrange & Act
            var result = UserValidator.IsPasswordValid("Sh0rt!");
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_NoUpperCase_ReturnsFalse()
        {
            // Arrange & Act
            var result = UserValidator.IsPasswordValid("validpass1!");
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_NoLowerCase_ReturnsFalse()
        {
            // Arrange & Act
            var result = UserValidator.IsPasswordValid("VALIDPASS1!");
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_NoDigit_ReturnsFalse()
        {
            // Arrange & Act
            var result = UserValidator.IsPasswordValid("ValidPass!");
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_NoSpecialChar_ReturnsFalse()
        {
            // Arrange & Act
            var result = UserValidator.IsPasswordValid("ValidPass1");
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_NullPassword_ReturnsFalse()
        {
            // Arrange & Act
            var result = UserValidator.IsPasswordValid(null);
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_EmptyPassword_ReturnsFalse()
        {
            // Arrange & Act
            var result = UserValidator.IsPasswordValid(string.Empty);
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsPasswordValid_WhitespacePassword_ReturnsFalse()
        {
            // Arrange & Act
            var result = UserValidator.IsPasswordValid("   ");
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_ValidEmail_ReturnsTrue()
        {
            // Arrange & Act
            var result = UserValidator.IsEmailValid("test@example.com");
            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsEmailValid_NoAtSign_ReturnsFalse()
        {
            // Arrange & Act
            var result = UserValidator.IsEmailValid("testexample.com");
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_NoDomain_ReturnsFalse()
        {
            // Arrange & Act
            var result = UserValidator.IsEmailValid("test@");
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_NoTLD_ReturnsFalse()
        {
            // Arrange & Act
            var result = UserValidator.IsEmailValid("test@example");
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_NullEmail_ReturnsFalse()
        {
            // Arrange & Act
            var result = UserValidator.IsEmailValid(null);
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_EmptyEmail_ReturnsFalse()
        {
            // Arrange & Act
            var result = UserValidator.IsEmailValid(string.Empty);
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsEmailValid_WhitespaceEmail_ReturnsFalse()
        {
            // Arrange & Act
            var result = UserValidator.IsEmailValid("   ");
            // Assert
            Assert.That(result, Is.False);
        }

        // --- Additional Tests for UserValidator ---
        [Test]
        public void IsValidUsername_ValidUsername_ReturnsTrue()
        {
            // Arrange & Act
            var result = UserValidator.IsValidUsername("Username123");
            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsValidUsername_NullUsername_ReturnsFalse()
        {
            // Arrange & Act
            var result = UserValidator.IsValidUsername(null);
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsValidUsername_EmptyUsername_ReturnsFalse()
        {
            // Arrange & Act
            var result = UserValidator.IsValidUsername(string.Empty);
            // Assert
            Assert.That(result, Is.False);
        }
    }
}
