using System;
using System.Collections.Generic;
using BusinessLayer.Models;
using BusinessLayer.Services;
using BusinessLayer.Exceptions;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services.Proxies
{
    public class ReviewServiceProxy : ServiceProxy, IReviewService
    {
        public ReviewServiceProxy(string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
        }

        public bool SubmitReview(Review reviewToSubmit)
        {
            try
            {
                // Set the timestamp if not already set
                if (reviewToSubmit.DateAndTimeWhenReviewWasCreated == default)
                {
                    reviewToSubmit.DateAndTimeWhenReviewWasCreated = DateTime.Now;
                }

                return PostAsync<bool>("Review", reviewToSubmit).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EditReview(Review updatedReview)
        {
            try
            {
                // Update the timestamp
                updatedReview.DateAndTimeWhenReviewWasCreated = DateTime.Now;

                return PutAsync<bool>($"Review/{updatedReview.ReviewIdentifier}", updatedReview).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteReview(int reviewIdentifier)
        {
            try
            {
                return DeleteAsync<bool>($"Review/{reviewIdentifier}").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<Review> GetAllReviewsForAGame(int gameIdentifier)
        {
            try
            {
                return GetAsync<List<Review>>($"Review/game/{gameIdentifier}").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return new List<Review>();
            }
        }

        public (int TotalReviews, double PositivePercentage, double AverageRating) GetReviewStatisticsForGame(int gameIdentifier)
        {
            try
            {
                var stats = GetAsync<ReviewStatistics>($"Review/game/{gameIdentifier}/statistics").GetAwaiter().GetResult();
                return (stats.TotalReviews, stats.PositivePercentage, stats.AverageRating);
            }
            catch (Exception)
            {
                return (0, 0, 0);
            }
        }

        public List<Review> SortReviews(List<Review> reviews, string sortBy)
        {
            // This method can be implemented client-side
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
            // This method can be implemented client-side
            return recommendationFilter switch
            {
                "Positive Only" => reviews.Where(r => r.IsRecommended).ToList(),
                "Negative Only" => reviews.Where(r => !r.IsRecommended).ToList(),
                _ => reviews
            };
        }

        public bool ToggleVote(int reviewIdentifier, string voteType, bool shouldIncrement)
        {
            try
            {
                return PostAsync<bool>($"Review/{reviewIdentifier}/vote", new
                {
                    VoteType = voteType,
                    ShouldIncrement = shouldIncrement
                }).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateReview(int reviewId, Review updatedReview)
        {
            updatedReview.ReviewIdentifier = reviewId;
            return EditReview(updatedReview);
        }

        // Helper class for statistics response
        private class ReviewStatistics
        {
            public int TotalReviews { get; set; }
            public double PositivePercentage { get; set; }
            public double AverageRating { get; set; }
        }
    }
}