using System.Runtime.Serialization;
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using BusinessLayer.Data;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Exceptions;
using Microsoft.Data.SqlClient;

namespace Tests.RepositoryTests
{
    /*
    [TestFixture]
    public class WalletRepositoryTests
    {
        private Mock<IDataLink> mockDataLink;
        private WalletRepository walletRepository;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            mockDataLink = new Mock<IDataLink>();
            walletRepository = new WalletRepository(mockDataLink.Object);
        }

        [Test]
        public void Constructor_NullDataLink_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new WalletRepository(null));
        }

        #region GetWallet Tests

        [Test]
        public void GetWallet_ValidWalletId_ReturnsWalletWithCorrectId()
        {
            // Arrange
            int expectedWalletId = 1;
            var walletDataTable = CreateWalletDataTable();
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(walletDataTable);

            // Act
            Wallet resultWallet = walletRepository.GetWallet(expectedWalletId);

            // Assert
            Assert.That(resultWallet.WalletId, Is.EqualTo(expectedWalletId));
        }

        [Test]
        public void GetWallet_ValidWalletId_ReturnsWalletWithCorrectUserId()
        {
            // Arrange
            int walletId = 1;
            int expectedUserId = 1;
            var walletDataTable = CreateWalletDataTable();
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(walletDataTable);

            // Act
            Wallet resultWallet = walletRepository.GetWallet(walletId);

            // Assert
            Assert.That(resultWallet.UserId, Is.EqualTo(expectedUserId));
        }

        [Test]
        public void GetWallet_ValidWalletId_ReturnsWalletWithCorrectBalance()
        {
            // Arrange
            int walletId = 1;
            decimal expectedBalance = 100.50m;
            var walletDataTable = CreateWalletDataTable();
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(walletDataTable);

            // Act
            Wallet resultWallet = walletRepository.GetWallet(walletId);

            // Assert
            Assert.That(resultWallet.Balance, Is.EqualTo(expectedBalance));
        }

        [Test]
        public void GetWallet_ValidWalletId_ReturnsWalletWithCorrectPoints()
        {
            // Arrange
            int walletId = 1;
            int expectedPoints = 200;
            var walletDataTable = CreateWalletDataTable();
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(walletDataTable);

            // Act
            Wallet resultWallet = walletRepository.GetWallet(walletId);

            // Assert
            Assert.That(resultWallet.Points, Is.EqualTo(expectedPoints));
        }

        [Test]
        public void GetWallet_DatabaseException_ThrowsRepositoryException()
        {
            // Arrange
            int walletId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => walletRepository.GetWallet(walletId));
        }

        [Test]
        public void GetWallet_DatabaseException_ExceptionMessageContainsFailedToRetrieve()
        {
            // Arrange
            int walletId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("Database error"));

            // Act
            RepositoryException thrownException = Assert.Throws<RepositoryException>(() => walletRepository.GetWallet(walletId));

            // Assert
            Assert.That(thrownException.Message, Does.Contain("Failed to retrieve wallet"));
        }

        [Test]
        public void GetWallet_NoWalletFound_ThrowsRepositoryException()
        {
            // Arrange
            int walletId = 999;
            var emptyDataTable = new DataTable();
            emptyDataTable.Columns.Add("wallet_id", typeof(int));
            emptyDataTable.Columns.Add("user_id", typeof(int));
            emptyDataTable.Columns.Add("points", typeof(int));
            emptyDataTable.Columns.Add("money_for_games", typeof(decimal));

            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(emptyDataTable);

            // Act & Assert
            Assert.Throws<RepositoryException>(() => walletRepository.GetWallet(walletId));
        }

        [Test]
        public void GetWallet_NoWalletFound_ExceptionMessageContainsNotFound()
        {
            // Arrange
            int walletId = 999;
            var emptyDataTable = new DataTable();
            emptyDataTable.Columns.Add("wallet_id", typeof(int));
            emptyDataTable.Columns.Add("user_id", typeof(int));
            emptyDataTable.Columns.Add("points", typeof(int));
            emptyDataTable.Columns.Add("money_for_games", typeof(decimal));

            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(emptyDataTable);

            // Act
            RepositoryException thrownException = Assert.Throws<RepositoryException>(() => walletRepository.GetWallet(walletId));

            // Assert
            Assert.That(thrownException.Message, Does.Contain("not found"));
        }

        [Test]
        public void GetWallet_ValidWalletId_PassesCorrectParameterToDataLink()
        {
            // Arrange
            int walletId = 1;
            var walletDataTable = CreateWalletDataTable();
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(walletDataTable);

            // Act
            walletRepository.GetWallet(walletId);

            // Assert
            mockDataLink.Verify(dataLinkMock => dataLinkMock.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(parameter => parameter.ParameterName == "@wallet_id").Value) == walletId)),
                Times.Once);
        }

        #endregion

        #region GetWalletIdByUserId Tests

        [Test]
        public void GetWalletIdByUserId_ValidUserId_ReturnsWalletId()
        {
            // Arrange
            int userId = 1;
            int expectedWalletId = 5;
            var walletDataTable = new DataTable();
            walletDataTable.Columns.Add("wallet_id", typeof(int));
            walletDataTable.Rows.Add(expectedWalletId); // Wallet ID 5 for user 1

            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(walletDataTable);

            // Act
            int resultWalletId = walletRepository.GetWalletIdByUserId(userId);

            // Assert
            Assert.That(resultWalletId, Is.EqualTo(expectedWalletId));
        }

        [Test]
        public void GetWalletIdByUserId_NoWalletFound_ThrowsRepositoryException()
        {
            // Arrange
            int userId = 999;
            var emptyDataTable = new DataTable();
            emptyDataTable.Columns.Add("wallet_id", typeof(int));

            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(emptyDataTable);

            // Act & Assert
            Assert.Throws<RepositoryException>(() => walletRepository.GetWalletIdByUserId(userId));
        }

        [Test]
        public void GetWalletIdByUserId_NoWalletFound_ExceptionMessageContainsNoWalletFound()
        {
            // Arrange
            int userId = 999;
            var emptyDataTable = new DataTable();
            emptyDataTable.Columns.Add("wallet_id", typeof(int));

            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(emptyDataTable);

            // Act
            RepositoryException thrownException = Assert.Throws<RepositoryException>(() => walletRepository.GetWalletIdByUserId(userId));

            // Assert
            Assert.That(thrownException.Message, Does.Contain("No wallet found for user ID"));
        }

        [Test]
        public void GetWalletIdByUserId_DatabaseException_ThrowsRepositoryException()
        {
            // Arrange
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => walletRepository.GetWalletIdByUserId(userId));
        }

        [Test]
        public void GetWalletIdByUserId_DatabaseException_ExceptionMessageContainsFailedToRetrieve()
        {
            // Arrange
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("Database error"));

            // Act
            RepositoryException thrownException = Assert.Throws<RepositoryException>(() => walletRepository.GetWalletIdByUserId(userId));

            // Assert
            Assert.That(thrownException.Message, Does.Contain("Failed to retrieve wallet ID"));
        }

        [Test]
        public void GetWalletIdByUserId_ValidUserId_PassesCorrectParameterToDataLink()
        {
            // Arrange
            int userId = 1;
            var walletDataTable = new DataTable();
            walletDataTable.Columns.Add("wallet_id", typeof(int));
            walletDataTable.Rows.Add(5); // Wallet ID 5 for user 1

            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(walletDataTable);

            // Act
            walletRepository.GetWalletIdByUserId(userId);

            // Assert
            mockDataLink.Verify(dataLinkMock => dataLinkMock.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(parameter => parameter.ParameterName == "@user_id").Value) == userId)),
                Times.Once);
        }

        #endregion

        #region AddMoneyToWallet Tests

        [Test]
        public void AddMoneyToWallet_ValidParameters_CallsExecuteNonQuerySql()
        {
            // Arrange
            decimal moneyAmount = 50.25m;
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.AddMoneyToWallet(moneyAmount, userId);

            // Assert
            mockDataLink.Verify(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void AddMoneyToWallet_ValidParameters_CallsExecuteNonQuerySqlWithCorrectAmountParameter()
        {
            // Arrange
            decimal moneyAmount = 50.25m;
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.AddMoneyToWallet(moneyAmount, userId);

            // Assert
            mockDataLink.Verify(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToDecimal(parameters.First(parameter => parameter.ParameterName == "@amount").Value) == moneyAmount)),
                Times.Once);
        }

        [Test]
        public void AddMoneyToWallet_ValidParameters_CallsExecuteNonQuerySqlWithCorrectUserIdParameter()
        {
            // Arrange
            decimal moneyAmount = 50.25m;
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.AddMoneyToWallet(moneyAmount, userId);

            // Assert
            mockDataLink.Verify(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(parameter => parameter.ParameterName == "@user_id").Value) == userId)),
                Times.Once);
        }

        [Test]
        public void AddMoneyToWallet_DatabaseException_ThrowsRepositoryException()
        {
            // Arrange
            decimal moneyAmount = 50.25m;
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => walletRepository.AddMoneyToWallet(moneyAmount, userId));
        }

        [Test]
        public void AddMoneyToWallet_DatabaseException_ExceptionMessageContainsFailedToAdd()
        {
            // Arrange
            decimal moneyAmount = 50.25m;
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new DatabaseOperationException("Database error"));

            // Act
            RepositoryException thrownException = Assert.Throws<RepositoryException>(() => walletRepository.AddMoneyToWallet(moneyAmount, userId));

            // Assert
            Assert.That(thrownException.Message, Does.Contain("Failed to add money"));
        }

        #endregion

        #region AddPointsToWallet Tests

        [Test]
        public void AddPointsToWallet_ValidParameters_CallsExecuteNonQuerySql()
        {
            // Arrange
            int pointsAmount = 100;
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.AddPointsToWallet(pointsAmount, userId);

            // Assert
            mockDataLink.Verify(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void AddPointsToWallet_ValidParameters_CallsExecuteNonQuerySqlWithCorrectAmountParameter()
        {
            // Arrange
            int pointsAmount = 100;
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.AddPointsToWallet(pointsAmount, userId);

            // Assert
            mockDataLink.Verify(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(parameter => parameter.ParameterName == "@amount").Value) == pointsAmount)),
                Times.Once);
        }

        [Test]
        public void AddPointsToWallet_ValidParameters_CallsExecuteNonQuerySqlWithCorrectUserIdParameter()
        {
            // Arrange
            int pointsAmount = 100;
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.AddPointsToWallet(pointsAmount, userId);

            // Assert
            mockDataLink.Verify(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(parameter => parameter.ParameterName == "@user_id").Value) == userId)),
                Times.Once);
        }

        [Test]
        public void AddPointsToWallet_DatabaseException_ThrowsRepositoryException()
        {
            // Arrange
            int pointsAmount = 100;
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => walletRepository.AddPointsToWallet(pointsAmount, userId));
        }

        [Test]
        public void AddPointsToWallet_DatabaseException_ExceptionMessageContainsFailedToAdd()
        {
            // Arrange
            int pointsAmount = 100;
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new DatabaseOperationException("Database error"));

            // Act
            RepositoryException thrownException = Assert.Throws<RepositoryException>(() => walletRepository.AddPointsToWallet(pointsAmount, userId));

            // Assert
            Assert.That(thrownException.Message, Does.Contain("Failed to add points"));
        }

        #endregion

        #region GetMoneyFromWallet Tests

        [Test]
        public void GetMoneyFromWallet_ValidWalletId_ReturnsBalance()
        {
            // Arrange
            int walletId = 1;
            decimal expectedBalance = 100.50m;
            var walletDataTable = CreateWalletDataTable();
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(walletDataTable);

            // Act
            decimal resultBalance = walletRepository.GetMoneyFromWallet(walletId);

            // Assert
            Assert.That(resultBalance, Is.EqualTo(expectedBalance));
        }

        [Test]
        public void GetMoneyFromWallet_WalletNotFound_ThrowsRepositoryException()
        {
            // Arrange
            int walletId = 999;
            var emptyDataTable = new DataTable();
            emptyDataTable.Columns.Add("wallet_id", typeof(int));
            emptyDataTable.Columns.Add("user_id", typeof(int));
            emptyDataTable.Columns.Add("points", typeof(int));
            emptyDataTable.Columns.Add("money_for_games", typeof(decimal));

            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(emptyDataTable);

            // Act & Assert
            Assert.Throws<RepositoryException>(() => walletRepository.GetMoneyFromWallet(walletId));
        }

        [Test]
        public void GetMoneyFromWallet_DatabaseException_ThrowsRepositoryException()
        {
            // Arrange
            int walletId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => walletRepository.GetMoneyFromWallet(walletId));
        }

        #endregion

        #region GetPointsFromWallet Tests

        [Test]
        public void GetPointsFromWallet_ValidWalletId_ReturnsPoints()
        {
            // Arrange
            int walletId = 1;
            int expectedPoints = 200;
            var walletDataTable = CreateWalletDataTable();
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(walletDataTable);

            // Act
            int resultPoints = walletRepository.GetPointsFromWallet(walletId);

            // Assert
            Assert.That(resultPoints, Is.EqualTo(expectedPoints));
        }

        [Test]
        public void GetPointsFromWallet_WalletNotFound_ThrowsRepositoryException()
        {
            // Arrange
            int walletId = 999;
            var emptyDataTable = new DataTable();
            emptyDataTable.Columns.Add("wallet_id", typeof(int));
            emptyDataTable.Columns.Add("user_id", typeof(int));
            emptyDataTable.Columns.Add("points", typeof(int));
            emptyDataTable.Columns.Add("money_for_games", typeof(decimal));

            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(emptyDataTable);

            // Act & Assert
            Assert.Throws<RepositoryException>(() => walletRepository.GetPointsFromWallet(walletId));
        }

        [Test]
        public void GetPointsFromWallet_DatabaseException_ThrowsRepositoryException()
        {
            // Arrange
            int walletId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => walletRepository.GetPointsFromWallet(walletId));
        }

        #endregion

        #region PurchasePoints Tests

        [Test]
        public void PurchasePoints_ValidOffer_CallsExecuteNonQuerySql()
        {
            // Arrange
            var pointsOffer = new PointsOffer(10, 100); // price 10, points 100
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.PurchasePoints(pointsOffer, userId);

            // Assert
            mockDataLink.Verify(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void PurchasePoints_ValidParameters_CallsExecuteNonQuerySqlWithCorrectPriceParameter()
        {
            // Arrange
            var pointsOffer = new PointsOffer(10, 100); // price 10, points 100
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.PurchasePoints(pointsOffer, userId);

            // Assert
            mockDataLink.Verify(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(parameter => parameter.ParameterName == "@price").Value) == pointsOffer.Price)),
                Times.Once);
        }

        [Test]
        public void PurchasePoints_ValidParameters_CallsExecuteNonQuerySqlWithCorrectPointsParameter()
        {
            // Arrange
            var pointsOffer = new PointsOffer(10, 100); // price 10, points 100
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.PurchasePoints(pointsOffer, userId);

            // Assert
            mockDataLink.Verify(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(parameter => parameter.ParameterName == "@numberOfPoints").Value) == pointsOffer.Points)),
                Times.Once);
        }

        [Test]
        public void PurchasePoints_ValidParameters_CallsExecuteNonQuerySqlWithCorrectUserIdParameter()
        {
            // Arrange
            var pointsOffer = new PointsOffer(10, 100); // price 10, points 100
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.PurchasePoints(pointsOffer, userId);

            // Assert
            mockDataLink.Verify(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(parameter => parameter.ParameterName == "@user_id").Value) == userId)),
                Times.Once);
        }

        [Test]
        public void PurchasePoints_DatabaseException_ThrowsRepositoryException()
        {
            // Arrange
            var pointsOffer = new PointsOffer(10, 100); // price 10, points 100
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => walletRepository.PurchasePoints(pointsOffer, userId));
        }

        [Test]
        public void PurchasePoints_DatabaseException_ExceptionMessageContainsFailedToPurchase()
        {
            // Arrange
            var pointsOffer = new PointsOffer(10, 100); // price 10, points 100
            int userId = 1;
            mockDataLink.Setup(dataLinkMock => dataLinkMock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new DatabaseOperationException("Database error"));

            // Act
            RepositoryException thrownException = Assert.Throws<RepositoryException>(() => walletRepository.PurchasePoints(pointsOffer, userId));

            // Assert
            Assert.That(thrownException.Message, Does.Contain("Failed to purchase points"));
        }

        #endregion

        #region BuyWithMoney Tests

        [Test]
        public void BuyWithMoney_DatabaseException_ThrowsRepositoryException()
        {
            // Arrange
            decimal purchaseAmount = 50.25m;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => walletRepository.BuyWithMoney(purchaseAmount, userId));
        }

        [Test]
        public void BuyWithMoney_DatabaseException_ExceptionMessageContainsFailedToPurchase()
        {
            // Arrange
            decimal purchaseAmount = 50.25m;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new DatabaseOperationException("Database error"));

            // Act
            RepositoryException thrownException = Assert.Throws<RepositoryException>(() =>
                walletRepository.BuyWithMoney(purchaseAmount, userId));

            // Assert
            Assert.That(thrownException.Message, Does.Contain("Failed to purchase with money"));
        }

        [Test]
        public void BuyWithMoney_ValidParameters_CallsExecuteNonQuerySqlWithCorrectAmountParameter()
        {
            // Arrange
            decimal purchaseAmount = 50.25m;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.BuyWithMoney(purchaseAmount, userId);

            // Assert
            mockDataLink.Verify(mock => mock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToDecimal(parameters.First(p => p.ParameterName == "@amount").Value) == purchaseAmount)),
                Times.Once);
        }

        [Test]
        public void BuyWithMoney_ValidParameters_CallsExecuteNonQuerySqlWithCorrectUserIdParameter()
        {
            // Arrange
            decimal purchaseAmount = 50.25m;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.BuyWithMoney(purchaseAmount, userId);

            // Assert
            mockDataLink.Verify(mock => mock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(p => p.ParameterName == "@user_id").Value) == userId)),
                Times.Once);
        }

        [Test]
        public void BuyWithMoney_ValidParameters_CallsExecuteNonQuerySql()
        {
            // Arrange
            decimal amount = 50.25m;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.BuyWithMoney(amount, userId);

            // Assert
            mockDataLink.Verify(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void BuyWithMoney_ValidParameters_CallsExecuteNonQuerySqlWithCorrectParameters()
        {
            // Arrange
            decimal amount = 50.25m;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.BuyWithMoney(amount, userId);

            // Assert
            mockDataLink.Verify(mock => mock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToDecimal(parameters.First(p => p.ParameterName == "@amount").Value) == amount &&
                    Convert.ToInt32(parameters.First(p => p.ParameterName == "@user_id").Value) == userId)),
                Times.Once);
        }

        #endregion

        #region BuyWithPoints Tests

        [Test]
        public void BuyWithPoints_DatabaseException_ThrowsRepositoryException()
        {
            // Arrange
            int pointsAmount = 100;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => walletRepository.BuyWithPoints(pointsAmount, userId));
        }

        [Test]
        public void BuyWithPoints_DatabaseException_ExceptionMessageContainsFailedToPurchase()
        {
            // Arrange
            int pointsAmount = 100;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new DatabaseOperationException("Database error"));

            // Act
            RepositoryException thrownException = Assert.Throws<RepositoryException>(() =>
                walletRepository.BuyWithPoints(pointsAmount, userId));

            // Assert
            Assert.That(thrownException.Message, Does.Contain("Failed to purchase with points"));
        }

        [Test]
        public void BuyWithPoints_ValidParameters_CallsExecuteNonQuerySqlWithCorrectAmountParameter()
        {
            // Arrange
            int pointsAmount = 100;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.BuyWithPoints(pointsAmount, userId);

            // Assert
            mockDataLink.Verify(mock => mock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(p => p.ParameterName == "@amount").Value) == pointsAmount)),
                Times.Once);
        }

        [Test]
        public void BuyWithPoints_ValidParameters_CallsExecuteNonQuerySqlWithCorrectUserIdParameter()
        {
            // Arrange
            int pointsAmount = 100;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.BuyWithPoints(pointsAmount, userId);

            // Assert
            mockDataLink.Verify(mock => mock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(p => p.ParameterName == "@user_id").Value) == userId)),
                Times.Once);
        }

        [Test]
        public void BuyWithPoints_ValidParameters_CallsExecuteNonQuerySql()
        {
            // Arrange
            int points = 100;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.BuyWithPoints(points, userId);

            // Assert
            mockDataLink.Verify(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void BuyWithPoints_ValidParameters_CallsExecuteNonQuerySqlWithCorrectParameters()
        {
            // Arrange
            int points = 100;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.BuyWithPoints(points, userId);

            // Assert
            mockDataLink.Verify(mock => mock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(p => p.ParameterName == "@amount").Value) == points &&
                    Convert.ToInt32(parameters.First(p => p.ParameterName == "@user_id").Value) == userId)),
                Times.Once);
        }

        #endregion

        #region AddNewWallet Tests

        [Test]
        public void AddNewWallet_ValidParameters_CallsExecuteNonQuerySql()
        {
            // Arrange
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.AddNewWallet(userId);

            // Assert
            mockDataLink.Verify(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void AddNewWallet_ValidParameters_CallsExecuteNonQuerySqlWithCorrectUserIdParameter()
        {
            // Arrange
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.AddNewWallet(userId);

            // Assert
            mockDataLink.Verify(mock => mock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(p => p.ParameterName == "@user_id").Value) == userId)),
                Times.Once);
        }

        [Test]
        public void AddNewWallet_ExceptionInDataLink_CatchesException()
        {
            // Arrange
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test exception"));

            // Act & Assert
            Assert.DoesNotThrow(() => walletRepository.AddNewWallet(userId));
        }

        #endregion

        #region RemoveWallet Tests

        [Test]
        public void RemoveWallet_ValidParameters_CallsExecuteNonQuerySql()
        {
            // Arrange
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.RemoveWallet(userId);

            // Assert
            mockDataLink.Verify(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void RemoveWallet_ValidParameters_CallsExecuteNonQuerySqlWithCorrectUserIdParameter()
        {
            // Arrange
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.RemoveWallet(userId);

            // Assert
            mockDataLink.Verify(mock => mock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(p => p.ParameterName == "@user_id").Value) == userId)),
                Times.Once);
        }

        [Test]
        public void RemoveWallet_ExceptionInDataLink_CatchesException()
        {
            // Arrange
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test exception"));

            // Act & Assert
            Assert.DoesNotThrow(() => walletRepository.RemoveWallet(userId));
        }

        #endregion

        #region GetAllPointsOffers Tests

        [Test]
        public void GetAllPointsOffers_ReturnsArrayOfPointsOffers()
        {
            // Arrange
            var dataTable = new DataTable();
            dataTable.Columns.Add("numberOfPoints", typeof(int));
            dataTable.Columns.Add("value", typeof(int));
            dataTable.Rows.Add(100, 10);
            dataTable.Rows.Add(500, 45);

            mockDataLink.Setup(mock => mock.ExecuteReaderSql(It.IsAny<string>(), null))
                        .Returns(dataTable);

            // Act
            var result = walletRepository.GetAllPointsOffers();

            // Assert
            Assert.That(result.Length, Is.EqualTo(2));
            Assert.That(result[0].Points, Is.EqualTo(100));
            Assert.That(result[0].Price, Is.EqualTo(10));
            Assert.That(result[1].Points, Is.EqualTo(500));
            Assert.That(result[1].Price, Is.EqualTo(45));
        }

        [Test]
        public void GetAllPointsOffers_DatabaseException_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink.Setup(mock => mock.ExecuteReaderSql(It.IsAny<string>(), null))
                        .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => walletRepository.GetAllPointsOffers());
        }

        #endregion

        #region GetPointsOfferById Tests

        [Test]
        public void GetPointsOfferById_DatabaseException_ThrowsRepositoryException()
        {
            // Arrange
            int pointsOfferId = 1;
            mockDataLink.Setup(mock => mock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => walletRepository.GetPointsOfferById(pointsOfferId));
        }

        [Test]
        public void GetPointsOfferById_DatabaseException_ExceptionMessageContainsFailedToGet()
        {
            // Arrange
            int pointsOfferId = 1;
            mockDataLink.Setup(mock => mock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new DatabaseOperationException("Database error"));

            // Act
            RepositoryException thrownException = Assert.Throws<RepositoryException>(() =>
                walletRepository.GetPointsOfferById(pointsOfferId));

            // Assert
            Assert.That(thrownException.Message, Does.Contain("Failed to get points offer"));
        }

        [Test]
        public void GetPointsOfferById_NoOfferFound_ExceptionMessageContainsNotFound()
        {
            // Arrange
            int nonExistentOfferId = 999;
            var emptyDataTable = new DataTable();
            emptyDataTable.Columns.Add("numberOfPoints", typeof(int));
            emptyDataTable.Columns.Add("value", typeof(int));

            mockDataLink.Setup(mock => mock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(emptyDataTable);

            // Act
            RepositoryException thrownException = Assert.Throws<RepositoryException>(() =>
                walletRepository.GetPointsOfferById(nonExistentOfferId));

            // Assert
            Assert.That(thrownException.Message, Does.Contain("not found"));
        }

        [Test]
        public void GetPointsOfferById_ValidId_PassesCorrectParameterToDataLink()
        {
            // Arrange
            int pointsOfferId = 1;
            var pointsOfferDataTable = new DataTable();
            pointsOfferDataTable.Columns.Add("numberOfPoints", typeof(int));
            pointsOfferDataTable.Columns.Add("value", typeof(int));
            pointsOfferDataTable.Rows.Add(100, 10);

            mockDataLink.Setup(mock => mock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(pointsOfferDataTable);

            // Act
            walletRepository.GetPointsOfferById(pointsOfferId);

            // Assert
            mockDataLink.Verify(mock => mock.ExecuteReaderSql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(p => p.ParameterName == "@offerId").Value) == pointsOfferId)),
                Times.Once);
        }

        [Test]
        public void GetPointsOfferById_ValidId_ReturnsCorrectOffer()
        {
            // Arrange
            int offerId = 1;
            var dataTable = new DataTable();
            dataTable.Columns.Add("numberOfPoints", typeof(int));
            dataTable.Columns.Add("value", typeof(int));
            dataTable.Rows.Add(100, 10);

            mockDataLink.Setup(mock => mock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(dataTable);

            // Act
            var result = walletRepository.GetPointsOfferById(offerId);

            // Assert
            Assert.That(result.Points, Is.EqualTo(100));
            Assert.That(result.Price, Is.EqualTo(10));
        }

        [Test]
        public void GetPointsOfferById_NoOfferFound_ThrowsRepositoryException()
        {
            // Arrange
            int offerId = 999;
            var emptyDataTable = new DataTable();
            emptyDataTable.Columns.Add("numberOfPoints", typeof(int));
            emptyDataTable.Columns.Add("value", typeof(int));

            mockDataLink.Setup(mock => mock.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(emptyDataTable);

            // Act & Assert
            Assert.Throws<RepositoryException>(() => walletRepository.GetPointsOfferById(offerId));
        }

        #endregion

        #region WinPoints Tests

        [Test]
        public void WinPoints_DatabaseException_ThrowsRepositoryException()
        {
            // Arrange
            int winningPoints = 100;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => walletRepository.WinPoints(winningPoints, userId));
        }

        [Test]
        public void WinPoints_DatabaseException_ExceptionMessageContainsFailedToAdd()
        {
            // Arrange
            int winningPoints = 100;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Throws(new DatabaseOperationException("Database error"));

            // Act
            RepositoryException thrownException = Assert.Throws<RepositoryException>(() =>
                walletRepository.WinPoints(winningPoints, userId));

            // Assert
            Assert.That(thrownException.Message, Does.Contain("Failed to add winning points"));
        }

        [Test]
        public void WinPoints_ValidParameters_CallsExecuteNonQuerySqlWithCorrectAmountParameter()
        {
            // Arrange
            int winningPoints = 100;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.WinPoints(winningPoints, userId);

            // Assert
            mockDataLink.Verify(mock => mock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(p => p.ParameterName == "@amount").Value) == winningPoints)),
                Times.Once);
        }

        [Test]
        public void WinPoints_ValidParameters_CallsExecuteNonQuerySqlWithCorrectUserIdParameter()
        {
            // Arrange
            int winningPoints = 100;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.WinPoints(winningPoints, userId);

            // Assert
            mockDataLink.Verify(mock => mock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(p => p.ParameterName == "@user_id").Value) == userId)),
                Times.Once);
        }

        [Test]
        public void WinPoints_ValidParameters_CallsExecuteNonQuerySql()
        {
            // Arrange
            int points = 100;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.WinPoints(points, userId);

            // Assert
            mockDataLink.Verify(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void WinPoints_ValidParameters_CallsExecuteNonQuerySqlWithCorrectParameters()
        {
            // Arrange
            int points = 100;
            int userId = 1;
            mockDataLink.Setup(mock => mock.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()));

            // Act
            walletRepository.WinPoints(points, userId);

            // Assert
            mockDataLink.Verify(mock => mock.ExecuteNonQuerySql(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(parameters =>
                    Convert.ToInt32(parameters.First(p => p.ParameterName == "@amount").Value) == points &&
                    Convert.ToInt32(parameters.First(p => p.ParameterName == "@user_id").Value) == userId)),
                Times.Once);
        }

        #endregion

        #region Helper Methods

        private DataTable CreateWalletDataTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("wallet_id", typeof(int));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("points", typeof(int));
            dataTable.Columns.Add("money_for_games", typeof(decimal));

            // Added a sample row
            dataTable.Rows.Add(1, 1, 200, 100.50m);

            return dataTable;
        }

        #endregion
    }
    */
}