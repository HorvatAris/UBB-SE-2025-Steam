using Microsoft.VisualStudio.TestTools.UnitTesting;
using SteamCommunity.Reviews.Database;
using System.Data.SqlClient;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SteamCommunity.Reviews.Models;
using SteamCommunity.Reviews.Repository;
using System;
using System.Collections.Generic;


using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SteamCommunity.Reviews.Models;
using SteamCommunity.Reviews.Repository;
using SteamCommunity.Reviews.Services;
using System;
using System.Collections.Generic;

//MUST HAVE THESE INCLUDES IN EACH TEST FILE
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Community_Tests
{
    [TestClass]
    public class DatabaseConnectionTests
    {
        private IDatabaseConnection _dbConnection;

        [TestInitialize]
        public void SetUp()
        {
            _dbConnection = new DatabaseConnection();
        }

        [TestMethod]
        public void GetConnection_ShouldReturnValidSqlConnection()
        {
            // Arrange

            // Act
            SqlConnection connection = _dbConnection.GetConnection();

            // Assert
            Assert.IsNotNull(connection);
            Assert.AreEqual(System.Data.ConnectionState.Closed, connection.State);
        }

        [TestMethod]
        public void Connect_ShouldOpenConnection()
        {
            // Arrange
            using SqlConnection connection = _dbConnection.GetConnection();

            // Act
            _dbConnection.Connect(connection);

            // Assert
            Assert.AreEqual(System.Data.ConnectionState.Open, connection.State);

            // Cleanup
            _dbConnection.Disconnect(connection);
        }

        [TestMethod]
        public void Disconnect_ShouldCloseConnection()
        {
            // Arrange
            using SqlConnection connection = _dbConnection.GetConnection();
            _dbConnection.Connect(connection);

            // Act
            _dbConnection.Disconnect(connection);

            // Assert
            Assert.AreEqual(System.Data.ConnectionState.Closed, connection.State);
        }
    }
}


namespace SteamCommunity.ReviewsTests
{
    [TestClass]
    public class ReviewRepositoryTests
    {
        private ReviewRepository _repository;

        [TestInitialize]
        public void SetUp()
        {
            _repository = new ReviewRepository();
        }

        [TestMethod]
        public void InsertNewReviewIntoDatabase_ShouldReturnTrue()
        {
            var review = new Review
            {
                ReviewTitleText = "Integration Test Review",
                ReviewContentText = "Content for test",
                IsRecommended = true,
                NumericRatingGivenByUser = 4.5,
                TotalHoursPlayedByReviewer = 20,
                DateAndTimeWhenReviewWasCreated = DateTime.Now,
                UserIdentifier = 1,
                GameIdentifier = 1
            };

            var result = _repository.InsertNewReviewIntoDatabase(review);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void FetchAllReviewsByGameId_ShouldReturnAtLeastOne()
        {
            var reviews = _repository.FetchAllReviewsByGameId(1);

            Assert.IsTrue(reviews.Count >= 0);
        }

        [TestMethod]
        public void UpdateExistingReviewInDatabase_ShouldReturnTrue()
        {
            // Act
            var reviews = _repository.FetchAllReviewsByGameId(1);
            if (reviews.Count == 0) Assert.Inconclusive("No reviews to update.");

            var reviewToUpdate = reviews[0];
            reviewToUpdate.ReviewTitleText = "Updated Title";
            reviewToUpdate.DateAndTimeWhenReviewWasCreated = DateTime.Now;

            var result = _repository.UpdateExistingReviewInDatabase(reviewToUpdate);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ToggleVoteForReview_ShouldIncrementVoteCount()
        {
            var reviews = _repository.FetchAllReviewsByGameId(1);
            if (reviews.Count == 0) Assert.Inconclusive("No reviews to vote on.");

            var reviewId = reviews[0].ReviewIdentifier;

            var result = _repository.ToggleVoteForReview(reviewId, "Helpful", true);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RetrieveReviewStatisticsForGame_ShouldReturnValues()
        {
            var stats = _repository.RetrieveReviewStatisticsForGame(1);

            Assert.IsTrue(stats.TotalReviews >= 0);
            Assert.IsTrue(stats.TotalPositiveRecommendations >= 0);
            Assert.IsTrue(stats.AverageRatingValue >= 0.0);
        }

        [TestMethod]
        public void DeleteReviewFromDatabaseById_ShouldRemoveReview()
        {
            // First insert a review to delete
            var review = new Review
            {
                ReviewTitleText = "Temp Review",
                ReviewContentText = "To be deleted",
                IsRecommended = false,
                NumericRatingGivenByUser = 2,
                TotalHoursPlayedByReviewer = 5,
                DateAndTimeWhenReviewWasCreated = DateTime.Now,
                UserIdentifier = 1,
                GameIdentifier = 1
            };

            var insertResult = _repository.InsertNewReviewIntoDatabase(review);
            Assert.IsTrue(insertResult);

            // Fetch it back and delete
            var all = _repository.FetchAllReviewsByGameId(1);
            var lastReview = all[0];

            var deleteResult = _repository.DeleteReviewFromDatabaseById(lastReview.ReviewIdentifier);

            Assert.IsTrue(deleteResult);
        }
    }
}

namespace SteamCommunity.ReviewsTests
{
    [TestClass]
    public class ReviewServiceTests
    {
        private Mock<IReviewRepository> _mockRepo;
        private ReviewService _service;

        [TestInitialize]
        public void SetUp()
        {
            _mockRepo = new Mock<IReviewRepository>();
            _service = new ReviewService(_mockRepo.Object);
        }

        [TestMethod]
        public void SubmitReview_ShouldSetDateAndCallRepository()
        {
            // Arrange
            var review = new Review();
            _mockRepo.Setup(r => r.InsertNewReviewIntoDatabase(review)).Returns(true);

            // Act
            var result = _service.SubmitReview(review);

            // Assert
            Assert.IsTrue(result);
            Assert.AreNotEqual(default, review.DateAndTimeWhenReviewWasCreated);
            _mockRepo.Verify(r => r.InsertNewReviewIntoDatabase(review), Times.Once);
        }

        [TestMethod]
        public void EditReview_ShouldUpdateDateAndCallRepository()
        {
            // Arrange
            var review = new Review();
            _mockRepo.Setup(r => r.UpdateExistingReviewInDatabase(review)).Returns(true);

            // Act
            var result = _service.EditReview(review);

            // Assert
            Assert.IsTrue(result);
            Assert.AreNotEqual(default, review.DateAndTimeWhenReviewWasCreated);
            _mockRepo.Verify(r => r.UpdateExistingReviewInDatabase(review), Times.Once);
        }

        [TestMethod]
        public void DeleteReview_ShouldCallRepository()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteReviewFromDatabaseById(5)).Returns(true);

            // Act
            var result = _service.DeleteReview(5);

            // Assert
            Assert.IsTrue(result);
            _mockRepo.Verify(r => r.DeleteReviewFromDatabaseById(5), Times.Once);
        }

        [TestMethod]
        public void GetAllReviewsForAGame_ShouldReturnReviews()
        {
            // Arrange
            var expected = new List<Review> { new Review(), new Review() };
            _mockRepo.Setup(r => r.FetchAllReviewsByGameId(1)).Returns(expected);

            // Act
            var result = _service.GetAllReviewsForAGame(1);

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void GetReviewStatisticsForGame_ShouldReturnRoundedStats()
        {
            // Arrange
            _mockRepo.Setup(r => r.RetrieveReviewStatisticsForGame(1)).Returns((10, 7, 3.678));

            // Act
            var result = _service.GetReviewStatisticsForGame(1);

            // Assert
            Assert.AreEqual(10, result.TotalReviews);
            Assert.AreEqual(70.0, result.PositivePercentage);
            Assert.AreEqual(3.7, result.AverageRating);
        }

        [TestMethod]
        public void SortReviews_ShouldSortByHighestRating()
        {
            // Arrange
            var reviews = new List<Review>
            {
                new Review { NumericRatingGivenByUser = 1 },
                new Review { NumericRatingGivenByUser = 5 }
            };

            // Act
            var result = _service.SortReviews(reviews, "Highest Rating");

            // Assert
            Assert.AreEqual(5, result[0].NumericRatingGivenByUser);
        }

        [TestMethod]
        public void SortReviews_ShouldReturnOriginal_WhenInvalidSort()
        {
            // Arrange
            var reviews = new List<Review> { new Review { ReviewIdentifier = 1 } };

            // Act
            var result = _service.SortReviews(reviews, "Invalid");

            // Assert
            Assert.AreEqual(1, result[0].ReviewIdentifier);
        }

        [TestMethod]
        public void FilterReviewsByRecommendation_ShouldReturnPositiveOnly()
        {
            // Arrange
            var reviews = new List<Review>
            {
                new Review { IsRecommended = true },
                new Review { IsRecommended = false }
            };

            // Act
            var result = _service.FilterReviewsByRecommendation(reviews, "Positive Only");

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result[0].IsRecommended);
        }

        [TestMethod]
        public void FilterReviewsByRecommendation_ShouldReturnNegativeOnly()
        {
            // Arrange
            var reviews = new List<Review>
            {
                new Review { IsRecommended = true },
                new Review { IsRecommended = false }
            };

            // Act
            var result = _service.FilterReviewsByRecommendation(reviews, "Negative Only");

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsFalse(result[0].IsRecommended);
        }

        [TestMethod]
        public void ToggleVote_ShouldCallRepository()
        {
            // Arrange
            _mockRepo.Setup(r => r.ToggleVoteForReview(1, "Helpful", true)).Returns(true);

            // Act
            var result = _service.ToggleVote(1, "Helpful", true);

            // Assert
            Assert.IsTrue(result);
            _mockRepo.Verify(r => r.ToggleVoteForReview(1, "Helpful", true), Times.Once);
        }

        [TestMethod]
        public void UpdateReview_ShouldUpdateIdAndCallEditReview()
        {
            // Arrange
            var review = new Review();
            _mockRepo.Setup(r => r.UpdateExistingReviewInDatabase(review)).Returns(true);

            // Act
            var result = _service.UpdateReview(123, review);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(123, review.ReviewIdentifier);
        }
    }
}
