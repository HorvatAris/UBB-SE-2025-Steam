using Microsoft.EntityFrameworkCore;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Repositories;
using ReviewEntity = SteamHub.Api.Entities.Review;

namespace SteamHub.Api.Context.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext context;

        public ReviewRepository(DataContext newContext)
        {
            context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        // Fetch all reviews for a given game
        public async Task<List<Review>> FetchAllReviewsByGameId(int gameId)
        {
            return await context.Reviews
                .Where(review => review.GameIdentifier == gameId || gameId == 0)
                .OrderByDescending(review => review.DateAndTimeWhenReviewWasCreated)
                .Select(review => new Review
                {
                    ReviewIdentifier = review.ReviewIdentifier,
                    ReviewTitleText = review.ReviewTitleText,
                    ReviewContentText = review.ReviewContentText,
                    IsRecommended = review.IsRecommended,
                    NumericRatingGivenByUser = review.NumericRatingGivenByUser,
                    TotalHelpfulVotesReceived = review.TotalHelpfulVotesReceived,
                    TotalFunnyVotesReceived = review.TotalFunnyVotesReceived,
                    TotalHoursPlayedByReviewer = review.TotalHelpfulVotesReceived,
                    UserIdentifier = review.UserIdentifier,
                    DateAndTimeWhenReviewWasCreated = review.DateAndTimeWhenReviewWasCreated,
                    GameIdentifier = review.GameIdentifier,
                    Username = review.User.Username,
                    TitleOfGame = review.Game.Name
                }).ToListAsync();
        }

        // Insert a new review into the database
        public async Task<bool> InsertNewReviewIntoDatabase(Review reviewToInsert)
        {
            context.Reviews.Add(new ReviewEntity
            {
                ReviewIdentifier = reviewToInsert.ReviewIdentifier,
                ReviewTitleText = reviewToInsert.ReviewTitleText,
                ReviewContentText = reviewToInsert.ReviewContentText,
                IsRecommended = reviewToInsert.IsRecommended,
                NumericRatingGivenByUser = reviewToInsert.NumericRatingGivenByUser,
                TotalHelpfulVotesReceived = reviewToInsert.TotalHelpfulVotesReceived,
                TotalFunnyVotesReceived = reviewToInsert.TotalFunnyVotesReceived,
                TotalHoursPlayedByReviewer = reviewToInsert.TotalHelpfulVotesReceived,
                UserIdentifier = reviewToInsert.UserIdentifier,
                DateAndTimeWhenReviewWasCreated = reviewToInsert.DateAndTimeWhenReviewWasCreated,
                GameIdentifier = reviewToInsert.GameIdentifier,
               
            });
            return await context.SaveChangesAsync() > 0;
        }

        // Update an existing_review review based on its ID
        public async Task<bool> UpdateExistingReviewInDatabase(Review reviewToUpdate)
        {
            var existing_review = context.Reviews.FirstOrDefault(r => r.ReviewIdentifier == reviewToUpdate.ReviewIdentifier);
            if (existing_review == null)
            {
                return false;
            }
            existing_review.ReviewTitleText = reviewToUpdate.ReviewTitleText;
            existing_review.ReviewContentText = reviewToUpdate.ReviewContentText;
            existing_review.IsRecommended = reviewToUpdate.IsRecommended;
            existing_review.NumericRatingGivenByUser = reviewToUpdate.NumericRatingGivenByUser;
            existing_review.TotalHoursPlayedByReviewer = reviewToUpdate.TotalHoursPlayedByReviewer;
            existing_review.DateAndTimeWhenReviewWasCreated = reviewToUpdate.DateAndTimeWhenReviewWasCreated;

            return await context.SaveChangesAsync() > 0;
        }

        // Delete a review by its ID
        public async Task<bool> DeleteReviewFromDatabaseById(int reviewIdToDelete)
        {
            var reviewToRemove = await context.Reviews.FindAsync(reviewIdToDelete);
            if (reviewToRemove == null)
            {
                return false;
            }
            context.Reviews.Remove(reviewToRemove);
            return await context.SaveChangesAsync() > 0;
        }

        // Toggle Helpful or Funny votes for a review
        public async Task<bool> ToggleVoteForReview(int reviewIdToVoteOn, string voteTypeAsStringEitherHelpfulOrFunny, bool shouldIncrementVoteCount)
        {
            var review = context.Reviews.Find(reviewIdToVoteOn);
            if (review == null)
            {
                return false;
            }

            // Get the current vote count
            int currentVoteCount = voteTypeAsStringEitherHelpfulOrFunny == "Helpful" ? review.TotalHelpfulVotesReceived : review.TotalFunnyVotesReceived;

            // If shouldIncrementVoteCount is true, we're adding a vote
            // If false, we're removing a vote
            if (shouldIncrementVoteCount)
            {
                // Check if the user has already voted
                if (currentVoteCount > 0)
                {
                    // User has already voted, so we'll remove their vote
                    if (voteTypeAsStringEitherHelpfulOrFunny == "Helpful")
                    {
                        review.TotalHelpfulVotesReceived--;
                    }
                    else
                    {
                        review.TotalFunnyVotesReceived--;
                    }
                }
                else
                {
                    // User hasn't voted yet, so we'll add their vote
                    if (voteTypeAsStringEitherHelpfulOrFunny == "Helpful")
                    {
                        review.TotalHelpfulVotesReceived++;
                    }
                    else
                    {
                        review.TotalFunnyVotesReceived++;
                    }
                }
            }
            else
            {
                // Removing a vote
                if (voteTypeAsStringEitherHelpfulOrFunny == "Helpful")
                {
                    review.TotalHelpfulVotesReceived = Math.Max(0, review.TotalHelpfulVotesReceived - 1);
                }
                else
                {
                    review.TotalFunnyVotesReceived = Math.Max(0, review.TotalFunnyVotesReceived - 1);
                }
            }

            await context.SaveChangesAsync();
            return true;
        }
        // Retrieve review statistics for a specific game
        public async Task<(int TotalReviews, int TotalPositiveRecommendations, double AverageRatingValue)> RetrieveReviewStatisticsForGame(int gameId)
        {
            var stats =await context.Reviews
                .Where(review => review.GameIdentifier == gameId)
                .GroupBy(review => 1)
                .Select(game => new
                {
                    TotalReviews = game.Count(),
                    TotalPositiveRecommendations = game.Count(r => r.IsRecommended),
                    AverageRatingValue = game.Average(r => r.NumericRatingGivenByUser)
                })
                .FirstOrDefaultAsync();

            return stats == null
                ? (0, 0, 0.0)
                : (stats.TotalReviews, stats.TotalPositiveRecommendations, stats.AverageRatingValue);
        }
        
    }
}
