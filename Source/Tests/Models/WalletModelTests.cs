using NUnit.Framework;
using BusinessLayer.Models;

namespace Tests.Models
{
    [TestFixture]
    public class WalletModelTests
    {
        [Test]
        public void WalletId_GetterSetter_ReturnsExpectedValue()
        {
            // Arrange
            var wallet = new Wallet();
            int expectedId = 10;

            // Act
            wallet.WalletId = expectedId;

            // Assert
            Assert.That(wallet.WalletId, Is.EqualTo(expectedId));
        }

        [Test]
        public void UserId_GetterSetter_ReturnsExpectedValue()
        {
            // Arrange
            var wallet = new Wallet();
            int expectedUserId = 5;

            // Act
            wallet.UserId = expectedUserId;

            // Assert
            Assert.That(wallet.UserId, Is.EqualTo(expectedUserId));
        }

        [Test]
        public void Balance_GetterSetter_ReturnsExpectedValue()
        {
            // Arrange
            var wallet = new Wallet();
            decimal expectedBalance = 100.50m;

            // Act
            wallet.Balance = expectedBalance;

            // Assert
            Assert.That(wallet.Balance, Is.EqualTo(expectedBalance));
        }

        [Test]
        public void Points_GetterSetter_ReturnsExpectedValue()
        {
            // Arrange
            var wallet = new Wallet();
            int expectedPoints = 250;

            // Act
            wallet.Points = expectedPoints;

            // Assert
            Assert.That(wallet.Points, Is.EqualTo(expectedPoints));
        }
    }
}