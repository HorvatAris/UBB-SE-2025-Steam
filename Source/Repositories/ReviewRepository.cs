using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Data;
using BusinessLayer.Models;
using BusinessLayer.Data;

namespace BusinessLayer.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private DatabaseConnection databaseConnection;

        public ReviewRepository()
        {
            databaseConnection = new DatabaseConnection();
        }

        // Fetch all reviews for a given game
        public List<Review> FetchAllReviewsByGameId(int gameId)
        {
            var listOfReviewsForGame = new List<Review>();
            string sqlQueryToGetReviewsForGame = @"
                                              SELECT r.*, u.Name AS Username, 
                                              u.ProfilePicture AS ProfilePictureBlob
                                                FROM Reviews r
                                                INNER JOIN ReviewsUsers u ON r.UserId = u.UserId
                                                WHERE r.GameId = @GameId
                                               ORDER BY r.CreatedAt DESC";
            ;

            using (SqlConnection connection = databaseConnection.GetConnection())
            using (SqlCommand sqlCommandToFetchReviews = new SqlCommand(sqlQueryToGetReviewsForGame, connection))
            {
                sqlCommandToFetchReviews.Parameters.AddWithValue("@GameId", gameId);
                connection.Open();

                using (SqlDataReader sqlDataReaderForReviewRows = sqlCommandToFetchReviews.ExecuteReader())
                {
                    while (sqlDataReaderForReviewRows.Read())
                        listOfReviewsForGame.Add(MapSqlReaderRowToReviewObject(sqlDataReaderForReviewRows));
                }
            }

            return listOfReviewsForGame;
        }

        // Insert a new review into the database
        public bool InsertNewReviewIntoDatabase(Review reviewToInsert)
        {
            string sqlQueryToInsertReview = @"
                INSERT INTO Reviews 
                (Title, Content, IsRecommended, Rating, HelpfulVotes, FunnyVotes, HoursPlayed, CreatedAt, UserId, GameId)
                VALUES 
                (@Title, @Content, @IsRecommended, @Rating, 0, 0, @HoursPlayed, @CreatedAt, @UserId, @GameId)";

            return ExecuteSqlNonQueryWithParameterBinding(sqlQueryToInsertReview, sqlCommand =>
            {
                BindReviewObjectToSqlCommandParameters(sqlCommand, reviewToInsert, isUpdateOperation: false);
            });
        }

        // Update an existing review based on its ID
        public bool UpdateExistingReviewInDatabase(Review reviewToUpdate)
        {
            string sqlQueryToUpdateReview = @"
                UPDATE Reviews
                SET Title = @Title,
                    Content = @Content,
                    IsRecommended = @IsRecommended,
                    Rating = @Rating,
                    HoursPlayed = @HoursPlayed,
                    CreatedAt = @CreatedAt
                WHERE ReviewId = @ReviewId";

            return ExecuteSqlNonQueryWithParameterBinding(sqlQueryToUpdateReview, sqlCommand =>
            {
                BindReviewObjectToSqlCommandParameters(sqlCommand, reviewToUpdate, isUpdateOperation: true);
            });
        }

        // Delete a review by its ID
        public bool DeleteReviewFromDatabaseById(int reviewIdToDelete)
        {
            string sqlQueryToDeleteReview = "DELETE FROM Reviews WHERE ReviewId = @ReviewId";

            return ExecuteSqlNonQueryWithParameterBinding(sqlQueryToDeleteReview, sqlCommand =>
            {
                sqlCommand.Parameters.AddWithValue("@ReviewId", reviewIdToDelete);
            });
        }

        // Toggle Helpful or Funny votes for a review
        public bool ToggleVoteForReview(int reviewIdToVoteOn, string voteTypeAsStringEitherHelpfulOrFunny, bool shouldIncrementVoteCount)
        {
            string voteColumnNameToUpdate = voteTypeAsStringEitherHelpfulOrFunny == "Helpful" ? "HelpfulVotes" : "FunnyVotes";
            string voteOperationSymbol = shouldIncrementVoteCount ? "+" : "-";

            string sqlQueryToUpdateVoteCount = $@"
                UPDATE Reviews
                SET {voteColumnNameToUpdate} = {voteColumnNameToUpdate} {voteOperationSymbol} 1
                WHERE ReviewId = @ReviewId";

            return ExecuteSqlNonQueryWithParameterBinding(sqlQueryToUpdateVoteCount, sqlCommand =>
            {
                sqlCommand.Parameters.AddWithValue("@ReviewId", reviewIdToVoteOn);
            });
        }





        // Retrieve review statistics for a specific game
        public (int TotalReviews, int TotalPositiveRecommendations, double AverageRatingValue) RetrieveReviewStatisticsForGame(int gameIdToFetchStatsFor)
        {
            string sqlQueryToGetReviewStatistics = @"
                SELECT 
                    COUNT(*) AS TotalReviews,
                    SUM(CASE WHEN IsRecommended = 1 THEN 1 ELSE 0 END) AS PositiveReviews,
                    AVG(Rating) AS AvgRating
                FROM Reviews
                WHERE GameId = @GameId";

            int totalReviewsCount = 0, totalPositiveRecommendationsCount = 0;
            double averageRatingForGame = 0.0;

            using (SqlConnection connectionForStatisticsQuery = databaseConnection.GetConnection())
            using (SqlCommand sqlCommandForStatisticsQuery = new SqlCommand(sqlQueryToGetReviewStatistics, connectionForStatisticsQuery))
            {
                sqlCommandForStatisticsQuery.Parameters.AddWithValue("@GameId", gameIdToFetchStatsFor);
                connectionForStatisticsQuery.Open();

                using (SqlDataReader readerForStatistics = sqlCommandForStatisticsQuery.ExecuteReader())
                {
                    if (readerForStatistics.Read())
                    {
                        totalReviewsCount = Convert.ToInt32(readerForStatistics["TotalReviews"]);
                        totalPositiveRecommendationsCount = readerForStatistics.IsDBNull(readerForStatistics.GetOrdinal("PositiveReviews")) ? 0 : Convert.ToInt32(readerForStatistics["PositiveReviews"]);
                        averageRatingForGame = readerForStatistics["AvgRating"] != DBNull.Value
                            ? Convert.ToDouble(readerForStatistics["AvgRating"])
                            : 0.0;
                    }
                }
            }

            return (totalReviewsCount, totalPositiveRecommendationsCount, averageRatingForGame);
        }




        // Helper: Reusable review mapping from SqlDataReader
        private Review MapSqlReaderRowToReviewObject(SqlDataReader sqlDataReaderRow)
        {
            try
            {
                return new Review
                {
                    ReviewIdentifier = Convert.ToInt32(sqlDataReaderRow["ReviewId"]),
                    ReviewTitleText = sqlDataReaderRow["Title"]?.ToString() ?? "",
                    ReviewContentText = sqlDataReaderRow["Content"]?.ToString() ?? "",
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
            using (SqlConnection connectionToExecuteNonQuery = databaseConnection.GetConnection())
            using (SqlCommand sqlCommandToExecute = new SqlCommand(sqlQueryToExecute, connectionToExecuteNonQuery))
            {
                bindSqlParametersAction(sqlCommandToExecute);
                connectionToExecuteNonQuery.Open();
                int rowsAffected = sqlCommandToExecute.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
