using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ApiContract.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository reviewRepository;
        private readonly IGameRepository _gameRepository;

        public ReviewService(IReviewRepository newReviewRepository, IGameRepository gameRepository)
        {
            reviewRepository = newReviewRepository ?? throw new ArgumentNullException(nameof(newReviewRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async Task<bool> SubmitReview(Review reviewToSubmit)
        {
            reviewToSubmit.DateAndTimeWhenReviewWasCreated = DateTime.Now;
            var insertNewReviewIntoDatabase = await reviewRepository.InsertNewReviewIntoDatabase(reviewToSubmit);
            if (insertNewReviewIntoDatabase)
            {
                await UpdateGameRating(reviewToSubmit.GameIdentifier);
            }
            return insertNewReviewIntoDatabase;
        }

        public async Task<bool> EditReview(Review updatedReview)
        {
            updatedReview.DateAndTimeWhenReviewWasCreated = DateTime.Now;
            var updated = await reviewRepository.UpdateExistingReviewInDatabase(updatedReview);
            if (updated)
            {
                await UpdateGameRating(updatedReview.GameIdentifier);
            }
            return updated;
        }

        public async Task<bool> DeleteReview(int reviewIdentifier)
        {
            var deletedReview = await reviewRepository.GetReviewById(reviewIdentifier);
            if (deletedReview == null)
            {
                return false;
            }
            var deleted = await reviewRepository.DeleteReviewFromDatabaseById(reviewIdentifier);
            if (deleted)
            {
                await UpdateGameRating(deletedReview.GameIdentifier);
            }
            return deleted;
            
        }

        public async Task<List<Review>> GetAllReviewsForAGame(int gameIdentifier)
        {
            return await reviewRepository.FetchAllReviewsByGameId(gameIdentifier);
        }

        public async Task<(int TotalReviews, double PositivePercentage, double AverageRating)> GetReviewStatisticsForGame(int gameIdentifier)
        {
            var (total, positive, average) = await reviewRepository.RetrieveReviewStatisticsForGame(gameIdentifier);
            double percentage = total > 0 ? (positive * 100.0) / total : 0.0;
            return (total, Math.Round(percentage, 1), Math.Round(average, 1));
        }

        public async Task<List<Review>> SortReviews(List<Review> reviews, string sortBy)
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

        public async Task<List<Review>> FilterReviewsByRecommendation(List<Review> reviews, string recommendationFilter)
        {
            return recommendationFilter switch
            {
                "Positive Only" => reviews.Where(r => r.IsRecommended).ToList(),
                "Negative Only" => reviews.Where(r => !r.IsRecommended).ToList(),
                _ => reviews
            };
        }

        public async Task<bool> ToggleVote(int reviewIdentifier, string voteType, bool shouldIncrement)
        {
            return await reviewRepository.ToggleVoteForReview(reviewIdentifier, voteType, shouldIncrement);
        }

        public Task<bool> UpdateReview(int reviewId, Review updatedReview)
        {
            updatedReview.ReviewIdentifier = reviewId;
            return EditReview(updatedReview);
        }

        private async Task UpdateGameRating(int gameId)
        {
            var (total, positive, avg) = await GetReviewStatisticsForGame(gameId);
            var updatedGame = new UpdateGameRequest()
            {
                Rating = (decimal?)avg,
            };
            await _gameRepository.UpdateGameAsync(gameId, updatedGame);
        }
    }
}
