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
                .Where(r => r.GameIdentifier == gameId || gameId == 0)
                .OrderByDescending(r => r.DateAndTimeWhenReviewWasCreated)
                .Select(r => new Review
                {
                    ReviewIdentifier = r.ReviewIdentifier,
                    ReviewTitleText = r.ReviewTitleText,
                    ReviewContentText = r.ReviewContentText,
                    IsRecommended = r.IsRecommended,
                    NumericRatingGivenByUser = r.NumericRatingGivenByUser,
                    TotalHelpfulVotesReceived = r.TotalHelpfulVotesReceived,
                    TotalFunnyVotesReceived = r.TotalFunnyVotesReceived,
                    TotalHoursPlayedByReviewer = r.TotalHelpfulVotesReceived,
                    UserIdentifier = r.UserIdentifier,
                    DateAndTimeWhenReviewWasCreated = r.DateAndTimeWhenReviewWasCreated,
                    GameIdentifier = r.GameIdentifier,
                    Username = r.User.Username,
                    TitleOfGame = r.Game.Name
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

        // Update an existing review based on its ID
        public async Task<bool> UpdateExistingReviewInDatabase(Review reviewToUpdate)
        {
            var existing = context.Reviews.FirstOrDefault(r => r.ReviewIdentifier == reviewToUpdate.ReviewIdentifier);
            if (existing == null)
            {
                return false;
            }
            existing.ReviewTitleText = reviewToUpdate.ReviewTitleText;
            existing.ReviewContentText = reviewToUpdate.ReviewContentText;
            existing.IsRecommended = reviewToUpdate.IsRecommended;
            existing.NumericRatingGivenByUser = reviewToUpdate.NumericRatingGivenByUser;
            existing.TotalHoursPlayedByReviewer = reviewToUpdate.TotalHoursPlayedByReviewer;
            existing.DateAndTimeWhenReviewWasCreated = reviewToUpdate.DateAndTimeWhenReviewWasCreated;

            return await context.SaveChangesAsync() > 0;
        }

        // Delete a review by its ID
        public async Task<bool> DeleteReviewFromDatabaseById(int reviewIdToDelete)
        {
            var toRemove = await context.Reviews.FindAsync(reviewIdToDelete);
            if (toRemove == null)
            {
                return false;
            }
            context.Reviews.Remove(toRemove);
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
                .Where(r => r.GameIdentifier == gameId)
                .GroupBy(r => 1)
                .Select(g => new
                {
                    TotalReviews = g.Count(),
                    TotalPositiveRecommendations = g.Count(r => r.IsRecommended),
                    AverageRatingValue = g.Average(r => r.NumericRatingGivenByUser)
                })
                .FirstOrDefaultAsync();

            return stats == null
                ? (0, 0, 0.0)
                : (stats.TotalReviews, stats.TotalPositiveRecommendations, stats.AverageRatingValue);
        }
        
    }
}
