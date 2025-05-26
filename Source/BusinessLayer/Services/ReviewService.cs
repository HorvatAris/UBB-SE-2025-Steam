using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository reviewRepository;

        public ReviewService(IReviewRepository newReviewRepository)
        {
            reviewRepository = newReviewRepository ?? throw new ArgumentNullException(nameof(newReviewRepository));
        }

        // TODO: Rework this so tests will work
        // Used in unit tests (mocked)
        /*
        public ReviewService(IReviewRepository? reviewRepository)
        {
            reviewRepository = reviewRepository;
        }
        */

        public bool SubmitReview(Review reviewToSubmit)
        {
            reviewToSubmit.DateAndTimeWhenReviewWasCreated = DateTime.Now;
            return reviewRepository.InsertNewReviewIntoDatabase(reviewToSubmit);
        }

        public bool EditReview(Review updatedReview)
        {
            updatedReview.DateAndTimeWhenReviewWasCreated = DateTime.Now;
            return reviewRepository.UpdateExistingReviewInDatabase(updatedReview);
        }

        public bool DeleteReview(int reviewIdentifier)
        {
            return reviewRepository.DeleteReviewFromDatabaseById(reviewIdentifier);
        }

        public List<Review> GetAllReviewsForAGame(int gameIdentifier)
        {
            return reviewRepository.FetchAllReviewsByGameId(gameIdentifier);
        }

        public (int TotalReviews, double PositivePercentage, double AverageRating) GetReviewStatisticsForGame(int gameIdentifier)
        {
            var (total, positive, average) = reviewRepository.RetrieveReviewStatisticsForGame(gameIdentifier);
            double percentage = total > 0 ? (positive * 100.0) / total : 0.0;
            return (total, Math.Round(percentage, 1), Math.Round(average, 1));
        }

        public List<Review> SortReviews(List<Review> reviews, string sortBy)
        {
            return sortBy switch
            {
                "Newest First" => reviews.OrderByDescending(r => r.DateAndTimeWhenReviewWasCreated).ToList(),
                "Oldest First" => reviews.OrderBy(r => r.DateAndTimeWhenReviewWasCreated).ToList(),
                "Highest Rating" => reviews.OrderByDescending(r => r.NumericRatingGivenByUser).ToList(),
                "Most Helpful" => reviews.OrderByDescending(r => r.TotalHelpfulVotesReceived).ToList(),
                _ => reviews
            };
        }

        public List<Review> FilterReviewsByRecommendation(List<Review> reviews, string recommendationFilter)
        {
            return recommendationFilter switch
            {
                "Positive Only" => reviews.Where(r => r.IsRecommended).ToList(),
                "Negative Only" => reviews.Where(r => !r.IsRecommended).ToList(),
                _ => reviews
            };
        }

        public bool ToggleVote(int reviewIdentifier, string voteType, bool shouldIncrement)
        {
            return reviewRepository.ToggleVoteForReview(reviewIdentifier, voteType, shouldIncrement);
        }

        public bool UpdateReview(int reviewId, Review updatedReview)
        {
            updatedReview.ReviewIdentifier = reviewId;
            return EditReview(updatedReview);
        }
    }
}
