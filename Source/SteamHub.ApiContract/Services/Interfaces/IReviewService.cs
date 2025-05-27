
using SteamHub.ApiContract.Models;

namespace SteamHub.ApiContract.Services.Interfaces
{
    public interface IReviewService
    {
        Task<bool> SubmitReview(Review reviewToSubmit);
        Task<bool> EditReview(Review updatedReview);
        Task<bool> DeleteReview(int reviewIdentifier);
        Task<List<Review>> GetAllReviewsForAGame(int gameIdentifier);
        Task<(int TotalReviews, double PositivePercentage, double AverageRating)> GetReviewStatisticsForGame(int gameIdentifier);
        Task<List<Review>> SortReviews(List<Review> reviews, string sortBy);
        Task<List<Review>> FilterReviewsByRecommendation(List<Review> reviews, string recommendationFilter);
        Task<bool> ToggleVote(int reviewIdentifier, string voteType, bool shouldIncrement);
        Task<bool> UpdateReview(int reviewId, Review updatedReview);
    }
}
