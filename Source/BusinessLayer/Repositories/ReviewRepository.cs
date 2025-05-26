using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Microsoft.Data.SqlClient;
using BusinessLayer.Models;
using BusinessLayer.Data;
using BusinessLayer.DataContext;

namespace BusinessLayer.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext context;

        public ReviewRepository(ApplicationDbContext newContext)
        {
            context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        // Fetch all reviews for a given game
        public List<Review> FetchAllReviewsByGameId(int gameId)
        {
            return context.Reviews
                .Where(r => r.GameIdentifier == gameId)
                .OrderByDescending(r => r.DateAndTimeWhenReviewWasCreated)
                .ToList();
        }

        // Insert a new review into the database
        public bool InsertNewReviewIntoDatabase(Review reviewToInsert)
        {
            // Check if the user exists in ReviewsUsers
            bool userExists = context.ReviewsUsers.Any(ru => ru.UserId == reviewToInsert.UserIdentifier);

            if (!userExists)
            {
                // Get user information from Users table to populate ReviewsUser
                var user = context.Users.FirstOrDefault(u => u.UserId == reviewToInsert.UserIdentifier);

                if (user != null)
                {
                    // Get user profile for profile picture
                    var userProfile = context.UserProfiles.FirstOrDefault(up => up.UserId == user.UserId);

                    // Create new ReviewsUser record
                    var reviewUser = new ReviewsUser
                    {
                        UserId = user.UserId,
                        Name = user.Username,
                        ProfilePicture = null // Or implement logic to read profile picture if needed
                    };

                    context.ReviewsUsers.Add(reviewUser);
                    context.SaveChanges();
                }
                else
                {
                    // User doesn't even exist in Users table
                    return false;
                }
            }

            // Now add the review (user is guaranteed to exist in ReviewsUsers)
            context.Reviews.Add(reviewToInsert);
            return context.SaveChanges() > 0;
        }

        // Update an existing review based on its ID
        public bool UpdateExistingReviewInDatabase(Review reviewToUpdate)
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

            return context.SaveChanges() > 0;
        }

        // Delete a review by its ID
        public bool DeleteReviewFromDatabaseById(int reviewIdToDelete)
        {
            var toRemove = context.Reviews.Find(reviewIdToDelete);
            if (toRemove == null)
            {
                return false;
            }
            context.Reviews.Remove(toRemove);
            return context.SaveChanges() > 0;
        }

        // Toggle Helpful or Funny votes for a review
        public bool ToggleVoteForReview(int reviewIdToVoteOn, string voteTypeAsStringEitherHelpfulOrFunny, bool shouldIncrementVoteCount)
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

            context.SaveChanges();
            return true;
        }
        // Retrieve review statistics for a specific game
        public (int TotalReviews, int TotalPositiveRecommendations, double AverageRatingValue) RetrieveReviewStatisticsForGame(int gameId)
        {
            var stats = context.Reviews
                .Where(r => r.GameIdentifier == gameId)
                .GroupBy(r => 1)
                .Select(g => new
                {
                    TotalReviews = g.Count(),
                    TotalPositiveRecommendations = g.Count(r => r.IsRecommended),
                    AverageRatingValue = g.Average(r => r.NumericRatingGivenByUser)
                })
                .FirstOrDefault();

            return stats == null
                ? (0, 0, 0.0)
                : (stats.TotalReviews, stats.TotalPositiveRecommendations, stats.AverageRatingValue);
        }

        // Helper: Reusable review mapping from SqlDataReader
        private Review MapSqlReaderRowToReviewObject(SqlDataReader sqlDataReaderRow)
        {
            try
            {
                return new Review
                {
                    ReviewIdentifier = Convert.ToInt32(sqlDataReaderRow["ReviewId"]),
                    ReviewTitleText = sqlDataReaderRow["Title"]?.ToString() ?? string.Empty,
                    ReviewContentText = sqlDataReaderRow["Content"]?.ToString() ?? string.Empty,
                    IsRecommended = Convert.ToBoolean(sqlDataReaderRow["IsRecommended"]),
                    NumericRatingGivenByUser = Convert.ToDouble(sqlDataReaderRow["Rating"]),
                    TotalHelpfulVotesReceived = Convert.ToInt32(sqlDataReaderRow["HelpfulVotes"]),
                    TotalFunnyVotesReceived = Convert.ToInt32(sqlDataReaderRow["FunnyVotes"]),
                    TotalHoursPlayedByReviewer = Convert.ToInt32(sqlDataReaderRow["HoursPlayed"]),
                    DateAndTimeWhenReviewWasCreated = Convert.ToDateTime(sqlDataReaderRow["CreatedAt"]),
                    UserIdentifier = Convert.ToInt32(sqlDataReaderRow["UserId"]),
                    GameIdentifier = Convert.ToInt32(sqlDataReaderRow["GameId"]),
                    UserName = sqlDataReaderRow["Username"]?.ToString() ?? "Unknown",
                    ProfilePictureBlob = sqlDataReaderRow["ProfilePictureBlob"] as byte[]
                };
            }
            catch (Exception ex)
            {
                File.WriteAllText("mapping_error.txt", ex.ToString());
                throw;
            }
        }

        /* TODO: Rework or delete this entirely
        // Bind parameters from Review object into SQL Command
        private void BindReviewObjectToSqlCommandParameters(SqlCommand sqlCommandToBindParametersTo, Review reviewDataToBind, bool isUpdateOperation)
        {
            sqlCommandToBindParametersTo.Parameters.AddWithValue("@Title", reviewDataToBind.ReviewTitleText);
            // sqlCommandToBindParametersTo.Parameters.AddWithValue("@Title", reviewDataToBind.TitleOfGame);
            sqlCommandToBindParametersTo.Parameters.AddWithValue("@Content", reviewDataToBind.ReviewContentText);
            sqlCommandToBindParametersTo.Parameters.AddWithValue("@IsRecommended", reviewDataToBind.IsRecommended);
            sqlCommandToBindParametersTo.Parameters.AddWithValue("@Rating", reviewDataToBind.NumericRatingGivenByUser);
            sqlCommandToBindParametersTo.Parameters.AddWithValue("@HoursPlayed", reviewDataToBind.TotalHoursPlayedByReviewer);
            sqlCommandToBindParametersTo.Parameters.AddWithValue("@CreatedAt", reviewDataToBind.DateAndTimeWhenReviewWasCreated);
            sqlCommandToBindParametersTo.Parameters.AddWithValue("@UserId", reviewDataToBind.UserIdentifier);
            sqlCommandToBindParametersTo.Parameters.AddWithValue("@GameId", reviewDataToBind.GameIdentifier);

            if (isUpdateOperation)
            {
                sqlCommandToBindParametersTo.Parameters.AddWithValue("@ReviewId", reviewDataToBind.ReviewIdentifier);
            }
        }

        // Execute a non-query SQL command (INSERT, UPDATE, DELETE) with parameter binding
        private bool ExecuteSqlNonQueryWithParameterBinding(string sqlQueryToExecute, Action<SqlCommand> bindSqlParametersAction)
        {
            using (SqlConnection connectionToExecuteNonQuery = reviewDatabaseConnection.GetConnection())
            using (SqlCommand sqlCommandToExecute = new SqlCommand(sqlQueryToExecute, connectionToExecuteNonQuery))
            {
                bindSqlParametersAction(sqlCommandToExecute);
                connectionToExecuteNonQuery.Open();
                int rowsAffected = sqlCommandToExecute.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        */
    }
}
