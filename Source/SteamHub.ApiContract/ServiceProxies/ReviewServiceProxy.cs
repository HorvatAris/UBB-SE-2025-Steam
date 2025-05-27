using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class ReviewServiceProxy : IReviewService
    {
        
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions{
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };


        public ReviewServiceProxy(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
        }


        public async Task<bool> SubmitReview(Review reviewToSubmit)
        {
            try
            {
                // Set the timestamp if not already set
                if (reviewToSubmit.DateAndTimeWhenReviewWasCreated == default)
                {
                    reviewToSubmit.DateAndTimeWhenReviewWasCreated = DateTime.Now;
                }

                var response = await _httpClient.PostAsJsonAsync("/api/Review", reviewToSubmit, _options);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<bool>(_options);
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

                var response = await _httpClient.PostAsJsonAsync($"/api/Review/{updatedReview.ReviewIdentifier}", updatedReview, _options);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<bool>(_options);
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
                var response = await _httpClient.DeleteAsync($"/api/Review/{reviewIdentifier}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<bool>(_options);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Review>> GetAllReviewsForAGame(int gameIdentifier)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Review/game/{gameIdentifier}");
                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadFromJsonAsync<List<Review>>(_options))!;
            }
            catch (Exception)
            {
                return new List<Review>();
            }
        }

        public async Task<(int TotalReviews, double PositivePercentage, double AverageRating)> GetReviewStatisticsForGame(int gameIdentifier)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Review/game/{gameIdentifier}/statistics");
                response.EnsureSuccessStatusCode();
                var stats =  (await response.Content.ReadFromJsonAsync<ReviewStatistics>(_options))!;

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
                var response =  await _httpClient.PostAsJsonAsync($"/api/Review/{reviewIdentifier}/vote", new
                {
                    VoteType = voteType,
                    ShouldIncrement = shouldIncrement
                }, _options);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<bool>(_options);
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