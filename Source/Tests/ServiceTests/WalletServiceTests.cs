using System;
using NUnit.Framework;
using Moq;
using BusinessLayer.Services;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Exceptions;

namespace Tests.ServiceTests
{
    [TestFixture]
    public class WalletServiceTests
    {
        private Mock<IWalletRepository> mockWalletRepository;
        private Mock<IUserService> mockUserService;
        private WalletService walletService;
        private User currentUser;
        private const int USER_ID = 1;
        private const int WALLET_ID = 5;

        [SetUp]
        public void SetUp()
        {
            // Set up mocks
            mockWalletRepository = new Mock<IWalletRepository>();
            mockUserService = new Mock<IUserService>();

            // Create a test user
            currentUser = new User { UserId = USER_ID, Username = "testuser" };

            // Set up UserService to return the test user
            mockUserService.Setup(mockUser => mockUser.GetCurrentUser()).Returns(currentUser);

            // Set up WalletRepository to return a wallet ID for the test user
            mockWalletRepository.Setup(mockWallet => mockWallet.GetWalletIdByUserId(USER_ID)).Returns(WALLET_ID);

            // Create service with mocked dependencies
            walletService = new WalletService(mockWalletRepository.Object, mockUserService.Object);
        }

        [Test]
        public void Constructor_NullWalletRepository_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new WalletService(null, mockUserService.Object));
        }

        [Test]
        public void Constructor_NullUserService_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new WalletService(mockWalletRepository.Object, null));
        }

        [Test]
        public void AddMoney_ValidAmount_CallsRepositoryMethod()
        {
            // Arrange
            decimal amount = 50m;

            // Act
            walletService.AddMoney(amount);

            // Assert
            mockWalletRepository.Verify(mockWalletRepository => mockWalletRepository.AddMoneyToWallet(amount, USER_ID), Times.Once);
        }

        [Test]
        public void AddPoints_ValidPoints_CallsRepositoryMethod()
        {
            // Arrange
            int points = 100;

            // Act
            walletService.AddPoints(points);

            // Assert
            mockWalletRepository.Verify(mockWalletRepository => mockWalletRepository.AddPointsToWallet(points, USER_ID), Times.Once);
        }

        [Test]
        public void GetBalance_ReturnsMoney()
        {
            // Arrange
            decimal expectedBalance = 150.50m;
            mockWalletRepository.Setup(mockWalletRepository => mockWalletRepository.GetMoneyFromWallet(WALLET_ID)).Returns(expectedBalance);

            // Act
            decimal result = walletService.GetBalance();

            // Assert
            Assert.That(result, Is.EqualTo(expectedBalance));
        }

        [Test]
        public void GetPoints_ReturnsPoints()
        {
            // Arrange
            int expectedPoints = 250;
            mockWalletRepository.Setup(mockWalletRepository => mockWalletRepository.GetPointsFromWallet(WALLET_ID)).Returns(expectedPoints);

            // Act
            int result = walletService.GetPoints();

            // Assert
            Assert.That(result, Is.EqualTo(expectedPoints));
        }

        [Test]
        public void GetBalance_NoWalletFound_CreatesNewWalletAndReturnsZero()
        {
            // Arrange
            mockWalletRepository.Setup(mockWalletRepository => mockWalletRepository.GetWalletIdByUserId(USER_ID))
                                .Throws(new RepositoryException("No wallet found for user ID 1."));

            // Act
            decimal result = walletService.GetBalance();

            // Assert
            Assert.That(result, Is.EqualTo(0m));
            mockWalletRepository.Verify(mockWalletRepository => mockWalletRepository.AddNewWallet(USER_ID), Times.Once);
        }

        [Test]
        public void GetPoints_NoWalletFound_CreatesNewWalletAndReturnsZero()
        {
            // Arrange
            mockWalletRepository.Setup(mockWalletRepository => mockWalletRepository.GetWalletIdByUserId(USER_ID))
                                .Throws(new RepositoryException("No wallet found for user ID 1."));

            // Act
            int result = walletService.GetPoints();

            // Assert
            Assert.That(result, Is.EqualTo(0));
            mockWalletRepository.Verify(mockWalletRepository => mockWalletRepository.AddNewWallet(USER_ID), Times.Once);
        }

        [Test]
        public void CreateWallet_ValidUserId_CallsRepositoryMethod()
        {
            // Arrange
            int userId = 10;

            // Act
            walletService.CreateWallet(userId);

            // Assert
            mockWalletRepository.Verify(mockWalletRepository => mockWalletRepository.AddNewWallet(userId), Times.Once);
        }

        [Test]
        public void PurchasePoints_NullOffer_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => walletService.PurchasePoints(null));
        }

        [Test]
        public void PurchasePoints_InsufficientBalance_ThrowsInvalidOperationException()
        {
            // Arrange
            var offer = new PointsOffer(100, 200); // Price 100, points 200
            mockWalletRepository.Setup(mockWalletRepository => mockWalletRepository.GetMoneyFromWallet(WALLET_ID)).Returns(50m); // Only 50 in wallet

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => walletService.PurchasePoints(offer));
        }

        [Test]
        public void PurchasePoints_InsufficientBalance_ExceptionMessageContainsInsufficientFunds()
        {
            // Arrange
            var offer = new PointsOffer(100, 200); // Price 100, points 200
            mockWalletRepository.Setup(mockWalletRepository => mockWalletRepository.GetMoneyFromWallet(WALLET_ID)).Returns(50m); // Only 50 in wallet

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => walletService.PurchasePoints(offer));
            Assert.That(exception.Message, Is.EqualTo("Insufficient funds"));
        }

        [Test]
        public void PurchasePoints_SufficientBalance_CallsRepositoryMethod()
        {
            // Arrange
            var offer = new PointsOffer(100, 200); // Price 100, points 200
            mockWalletRepository.Setup(mockWalletRepository => mockWalletRepository.GetMoneyFromWallet(WALLET_ID)).Returns(150m); // 150 in wallet

            // Act
            walletService.PurchasePoints(offer);

            // Assert
            mockWalletRepository.Verify(mockWalletRepository => mockWalletRepository.PurchasePoints(offer, USER_ID), Times.Once);
        }

        [Test]
        public void TryPurchasePoints_NullOffer_ReturnsFalse()
        {
            // Arrange

            // Act
            bool result = walletService.TryPurchasePoints(null);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryPurchasePoints_InsufficientBalance_ReturnsFalse()
        {
            // Arrange
            var offer = new PointsOffer(100, 200); // Price 100, points 200
            mockWalletRepository.Setup(mockWalletRepository => mockWalletRepository.GetMoneyFromWallet(WALLET_ID)).Returns(50m); // Only 50 in wallet

            // Act
            bool result = walletService.TryPurchasePoints(offer);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryPurchasePoints_ExceptionThrown_ReturnsFalse()
        {
            // Arrange
            var offer = new PointsOffer(100, 200); // Price 100, points 200
            mockWalletRepository.Setup(mockWalletRepository => mockWalletRepository.GetMoneyFromWallet(WALLET_ID)).Returns(150m); // 150 in wallet
            mockWalletRepository.Setup(mockWalletRepository => mockWalletRepository.PurchasePoints(offer, USER_ID)).Throws(new Exception("Test exception"));

            // Act
            bool result = walletService.TryPurchasePoints(offer);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TryPurchasePoints_SufficientBalance_ReturnsTrue()
        {
            // Arrange
            var offer = new PointsOffer(100, 200); // Price 100, points 200
            mockWalletRepository.Setup(mockWalletRepository => mockWalletRepository.GetMoneyFromWallet(WALLET_ID)).Returns(150m); // 150 in wallet

            // Act
            bool result = walletService.TryPurchasePoints(offer);

            // Assert
            Assert.That(result, Is.True);
        }
    }
}