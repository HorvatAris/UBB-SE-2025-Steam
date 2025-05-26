using System.Collections.Generic;
using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface IReviewService
    {
        bool SubmitReview(Review reviewToSubmit);
        bool EditReview(Review updatedReview);
        bool DeleteReview(int reviewIdentifier);
        List<Review> GetAllReviewsForAGame(int gameIdentifier);
        (int TotalReviews, double PositivePercentage, double AverageRating) GetReviewStatisticsForGame(int gameIdentifier);
        List<Review> SortReviews(List<Review> reviews, string sortBy);
        List<Review> FilterReviewsByRecommendation(List<Review> reviews, string recommendationFilter);
        bool ToggleVote(int reviewIdentifier, string voteType, bool shouldIncrement);
        bool UpdateReview(int reviewId, Review updatedReview);
    }
}
