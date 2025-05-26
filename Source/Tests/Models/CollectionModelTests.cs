using System;
using BusinessLayer.Models;
using NUnit.Framework;

namespace Tests.Models
{
    [TestFixture]
    public class CollectionPropertyTests
    {
        [Test]
        public void CollectionId_GetterSetter_ReturnsExpectedValue()
        {
            // Arrange
            var collection = CreateValidCollection();

            // Act
            collection.CollectionId = 10;

            // Assert
            Assert.That(collection.CollectionId, Is.EqualTo(10));
        }

        [Test]
        public void UserId_GetterSetter_ReturnsExpectedValue()
        {
            // Arrange
            var collection = CreateValidCollection();

            // Act
            collection.UserId = 99;

            // Assert
            Assert.That(collection.UserId, Is.EqualTo(99));
        }

        [Test]
        public void Name_GetterSetter_ReturnsExpectedValue()
        {
            // Arrange
            var collection = CreateValidCollection();

            // Act
            collection.CollectionName = "Strategy Games";

            // Assert
            Assert.That(collection.CollectionName, Is.EqualTo("Strategy Games"));
        }

        [Test]
        public void CoverPicture_GetterSetter_ReturnsExpectedValue()
        {
            // Arrange
            var collection = CreateValidCollection();

            // Act
            collection.CoverPicture = "https://example.com/image.jpg";

            // Assert
            Assert.That(collection.CoverPicture, Is.EqualTo("https://example.com/image.jpg"));
        }

        [Test]
        public void IsPublic_GetterSetter_ReturnsExpectedValue()
        {
            // Arrange
            var collection = CreateValidCollection();

            // Act
            collection.IsPublic = true;

            // Assert
            Assert.That(collection.IsPublic, Is.True);
        }

        [Test]
        public void CreatedAt_GetterSetter_ReturnsExpectedValue()
        {
            // Arrange
            var date = new DateOnly(2024, 12, 25);
            var collection = CreateValidCollection();

            // Act
            collection.CreatedAt = date;

            // Assert
            Assert.That(collection.CreatedAt, Is.EqualTo(date));
        }

        [Test]
        public void Games_DefaultInitialization_IsNotNull()
        {
            // Arrange
            var collection = CreateValidCollection();

            // Act & Assert
            Assert.That(collection.Games, Is.Not.Null);
        }

        [Test]
        public void Games_AddOwnedGame_GameIsInList()
        {
            // Arrange
            int userIdentifier = 1;
            var collection = CreateValidCollection();
            var ownedGame = new OwnedGame(userIdentifier, "Test", "Test"); // Assuming this is a simple model

            // Act
            collection.Games.Add(ownedGame);

            // Assert
            Assert.That(collection.Games, Contains.Item(ownedGame));
        }

        [Test]
        public void IsAllOwnedGamesCollection_DefaultValue_IsFalse()
        {
            // Arrange
            var collection = CreateValidCollection();

            // Act & Assert
            Assert.That(collection.IsAllOwnedGamesCollection, Is.False);
        }

        private Collection CreateValidCollection()
        {
            int userIdentifier = 1;
            return new Collection(userIdentifier, "Test", DateOnly.FromDateTime(DateTime.Today));
        }
    }
}
