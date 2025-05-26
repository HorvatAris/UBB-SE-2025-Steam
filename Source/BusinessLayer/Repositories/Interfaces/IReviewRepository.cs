using System.Collections.Generic;
using BusinessLayer.Models;

namespace BusinessLayer.Repositories
{
    public interface IReviewRepository
    {
        List<Review> FetchAllReviewsByGameId(int gameId);

        bool InsertNewReviewIntoDatabase(Review reviewToInsert);

        bool UpdateExistingReviewInDatabase(Review reviewToUpdate);

        bool DeleteReviewFromDatabaseById(int reviewIdToDelete);

        bool ToggleVoteForReview(int reviewIdToVoteOn, string voteTypeAsStringEitherHelpfulOrFunny, bool shouldIncrementVoteCount);

        (int TotalReviews, int TotalPositiveRecommendations, double AverageRatingValue)
            RetrieveReviewStatisticsForGame(int gameIdToFetchStatsFor);
    }
}
