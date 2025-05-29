using SteamHub.ApiContract.Models;

namespace SteamHub.ApiContract.Repositories
{
    public interface IReviewRepository
    {
        Task<List<Review>> FetchAllReviewsByGameId(int gameId);

        Task<bool> InsertNewReviewIntoDatabase(Review reviewToInsert);

        Task<bool> UpdateExistingReviewInDatabase(Review reviewToUpdate);

        Task<bool> DeleteReviewFromDatabaseById(int reviewIdToDelete);

        Task<bool> ToggleVoteForReview(int reviewIdToVoteOn, string voteTypeAsStringEitherHelpfulOrFunny, bool shouldIncrementVoteCount);

        Task<(int TotalReviews, int TotalPositiveRecommendations, double AverageRatingValue)> 
            RetrieveReviewStatisticsForGame(int gameIdToFetchStatsFor);
        
        Task<Review?> GetReviewById(int reviewId);
        
    }
}
