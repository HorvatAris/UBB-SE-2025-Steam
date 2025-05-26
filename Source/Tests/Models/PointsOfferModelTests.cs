using NUnit.Framework;
using BusinessLayer.Models;

namespace Tests.Models
{
    [TestFixture]
    public class PointsOfferModelTests
    {
        [Test]
        public void Constructor_ValidValues_SetsPointsCorrectly()
        {
            // Arrange
            int expectedPrice = 10;
            int expectedPoints = 50;

            // Act
            var offer = new PointsOffer(expectedPrice, expectedPoints);

            // Assert
            Assert.That(offer.Points, Is.EqualTo(expectedPoints));
        }

        [Test]
        public void Constructor_ValidValues_SetsPriceCorrectly()
        {
            // Arrange
            int expectedPrice = 10;
            int expectedPoints = 50;

            // Act
            var offer = new PointsOffer(expectedPrice, expectedPoints);

            // Assert
            Assert.That(offer.Price, Is.EqualTo(expectedPrice));
        }

        [Test]
        public void Price_GetterSetter_ReturnsExpectedValue()
        {
            // Arrange
            var offer = new PointsOffer(5, 25);
            int expectedPrice = 10;

            // Act
            offer.Price = expectedPrice;

            // Assert
            Assert.That(offer.Price, Is.EqualTo(expectedPrice));
        }

        [Test]
        public void Points_GetterSetter_ReturnsExpectedValue()
        {
            // Arrange
            var offer = new PointsOffer(5, 25);
            int expectedPoints = 100;

            // Act
            offer.Points = expectedPoints;

            // Assert
            Assert.That(offer.Points, Is.EqualTo(expectedPoints));
        }
    }
}