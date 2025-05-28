using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class ReviewServiceProxy : ServiceProxy, IReviewService
    {
        
        public ReviewServiceProxy(string baseUrl = "https://localhost:7241") : base(baseUrl){}


        public async Task<bool> SubmitReview(Review reviewToSubmit)
        {
            try
            {
                // Set the timestamp if not already set
                if (reviewToSubmit.DateAndTimeWhenReviewWasCreated == default)
                {
                    reviewToSubmit.DateAndTimeWhenReviewWasCreated = DateTime.Now;
                }

                return await PostAsync<bool>("/api/Review", reviewToSubmit);

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> EditReview(Review updatedReview)
        {
            try
            {
                // Update the timestamp
                updatedReview.DateAndTimeWhenReviewWasCreated = DateTime.Now;

                return await PutAsync<bool>($"/api/Review/{updatedReview.ReviewIdentifier}", updatedReview);
                
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteReview(int reviewIdentifier)
        {
            try
            {
                return await DeleteAsync<bool>($"/api/Review/{reviewIdentifier}");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Review>> GetAllReviewsForAGame(int gameIdentifier)
        {
            return await GetAsync<List<Review>>($"/api/Review/game/{gameIdentifier}");
        }

        public async Task<(int TotalReviews, double PositivePercentage, double AverageRating)> GetReviewStatisticsForGame(int gameIdentifier)
        {
            try
            {
                var stats = await GetAsync<ReviewStatistics>($"/api/Review/game/{gameIdentifier}/statistics");

                return (stats.TotalReviews, stats.PositivePercentage, stats.AverageRating);
            }
            catch (Exception)
            {
                return (0, 0, 0);
            }
        }

        public async Task<List<Review>> SortReviews(List<Review> reviews, string sortBy)
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

        public async Task<List<Review>> FilterReviewsByRecommendation(List<Review> reviews, string recommendationFilter)
        {
            // This method can be implemented client-side
            return recommendationFilter switch
            {
                "Positive Only" => reviews.Where(r => r.IsRecommended).ToList(),
                "Negative Only" => reviews.Where(r => !r.IsRecommended).ToList(),
                _ => reviews
            };
        }

        public async Task<bool> ToggleVote(int reviewIdentifier, string voteType, bool shouldIncrement)
        {
            try
            {
                return await PostAsync<bool>($"/api/Review/{reviewIdentifier}/vote", new
                {
                    VoteType = voteType,
                    ShouldIncrement = shouldIncrement
                });
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateReview(int reviewId, Review updatedReview)
        {
            updatedReview.ReviewIdentifier = reviewId;
            return await EditReview(updatedReview);
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