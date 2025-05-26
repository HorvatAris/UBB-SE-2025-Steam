using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
// using Windows.Security.Authentication.OnlineId;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;

namespace BusinessLayer.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private DatabaseConnection databaseConnection;
        public const int PAGE_SIZE = 9;

        public NewsRepository()
        {
            databaseConnection = new DatabaseConnection();
        }

        public int UpdatePostLikeCount(int postId)
        {
            try
            {
                databaseConnection.Connect();

                string query = $"UPDATE NewsPosts SET nrLikes = nrLikes + 1 WHERE id = {postId}";

                using (var command = new SqlCommand(query, databaseConnection.GetConnection()))
                {
                    int executionResult = command.ExecuteNonQuery();
                    return executionResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Cannot update post's like count: " + ex.Message);
            }
            finally
            {
                databaseConnection.Disconnect();
            }
        }

        public int UpdatePostDislikeCount(int postId)
        {
            try
            {
                databaseConnection.Connect();

                string query = $"UPDATE NewsPosts SET nrDislikes = nrDislikes + 1 WHERE id = {postId}";

                using (var command = new SqlCommand(query, databaseConnection.GetConnection()))
                {
                    int executionResult = command.ExecuteNonQuery();
                    return executionResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Cannot update post's dislike count: " + ex.Message);
            }
            finally
            {
                databaseConnection.Disconnect();
            }
        }

        public int AddRatingToPost(int postId, int userId, int ratingType)
        {
            try
            {
                databaseConnection.Connect();

                string query = $"INSERT INTO Ratings VALUES({postId}, {userId}, {ratingType})";

                using (var command = new SqlCommand(query, databaseConnection.GetConnection()))
                {
                    int executionResult = command.ExecuteNonQuery();
                    return executionResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Cannot insert rating: " + ex.Message);
            }
            finally
            {
                databaseConnection.Disconnect();
            }
        }

        public int RemoveRatingFromPost(int postId, int userId)
        {
            try
            {
                databaseConnection.Connect();

                string query = $"DELETE FROM Ratings WHERE postId={postId} AND authorId={userId}";

                using (var command = new SqlCommand(query, databaseConnection.GetConnection()))
                {
                    int executionResult = command.ExecuteNonQuery();
                    return executionResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Cannot remove post rating: " + ex.Message);
            }
            finally
            {
                databaseConnection.Disconnect();
            }
        }

        public int AddCommentToPost(int postId, string commentContent, int userId, DateTime commentDate)
        {
            try
            {
                databaseConnection.Connect();

                string query = $"INSERT INTO NewsComments VALUES({userId}, {postId}, N'{commentContent}', '{commentDate}')";

                using (var command = new SqlCommand(query, databaseConnection.GetConnection()))
                {
                    int executionResult = command.ExecuteNonQuery();
                    return executionResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Cannot insert new comment to post: " + ex.Message);
            }
            finally
            {
                databaseConnection.Disconnect();
            }
        }

        public int UpdateComment(int commentId, string commentContent)
        {
            try
            {
                databaseConnection.Connect();

                string query = $"UPDATE NewsComments SET content=N'{commentContent}' WHERE id={commentId}";

                using (var command = new SqlCommand(query, databaseConnection.GetConnection()))
                {
                    int executionResult = command.ExecuteNonQuery();
                    return executionResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Cannot update comment: " + ex.Message);
            }
            finally
            {
                databaseConnection.Disconnect();
            }
        }

        public int DeleteCommentFromDatabase(int commentId)
        {
            try
            {
                databaseConnection.Connect();

                string query = $"DELETE FROM NewsComments WHERE id={commentId}";

                using (var command = new SqlCommand(query, databaseConnection.GetConnection()))
                {
                    int executionResult = command.ExecuteNonQuery();
                    return executionResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Cannot delete comment from database: " + ex.Message);
            }
            finally
            {
                databaseConnection.Disconnect();
            }
        }

        public List<Comment> LoadFollowingComments(int postId)
        {
            try
            {
                databaseConnection.Connect();

                List<Comment> followingComments = new List<Comment>();

                string readQuery = $"""
        SELECT * FROM NewsComments WHERE postId={postId}
        """;

                var dataSet = databaseConnection.ExecuteQuery(readQuery, "NewsComments");

                foreach (DataRow row in dataSet.Tables["NewsComments"].Rows)
                {
                    var comment = new Comment
                    {
                        Id = Convert.ToUInt32(row["id"]),
                        AuthorId = Convert.ToUInt32(row["authorId"]),
                        Body = row["content"].ToString(),
                        TimeStamp = row["commentDate"].ToString(),
                        Score = 0 // Set default score if not in database
                    };
                    followingComments.Add(comment);
                }

                return followingComments;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: The rest of the comments could not be loaded: " + ex.Message);
            }
            finally
            {
                databaseConnection.Disconnect();
            }
        }

        public int AddPostToDatabase(int userId, string postContent, DateTime postDate)
        {
            try
            {
                databaseConnection.Connect();

                string query = $"INSERT INTO NewsPosts VALUES({userId}, N'{postContent}', '{postDate}', 0, 0, 0)";

                using (var command = new SqlCommand(query, databaseConnection.GetConnection()))
                {
                    int executionResult = command.ExecuteNonQuery();
                    return executionResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Cannot add post to database: " + ex.Message);
            }
            finally
            {
                databaseConnection.Disconnect();
            }
        }

        public int UpdatePost(int postId, string postContent)
        {
            try
            {
                databaseConnection.Connect();

                string query = $"UPDATE NewsPosts SET content=N'{postContent}' WHERE id={postId}";

                using (var command = new SqlCommand(query, databaseConnection.GetConnection()))
                {
                    int executionResult = command.ExecuteNonQuery();
                    return executionResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Cannot update post: " + ex.Message);
            }
            finally
            {
                databaseConnection.Disconnect();
            }
        }

        public int DeletePostFromDatabase(int postId)
        {
            try
            {
                databaseConnection.Connect();

                string query = $"DELETE FROM NewsPosts WHERE id={postId}";

                using (var command = new SqlCommand(query, databaseConnection.GetConnection()))
                {
                    int executionResult = command.ExecuteNonQuery();
                    return executionResult;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Cannot delete post from database: " + ex.Message);
            }
            finally
            {
                databaseConnection.Disconnect();
            }
        }

        public List<Post> LoadFollowingPosts(int pageNumber, int userId, string searchedText)
        {
            try
            {
                databaseConnection.Connect();

                List<Post> followingPosts = new List<Post>();

                int offset = (pageNumber - 1) * PAGE_SIZE;

                string readQuery = $"""
                    SELECT 
                        id,
                        authorId,
                        content,
                        uploadDate,
                        nrLikes,
                        nrDislikes,
                        nrComments
                    FROM (
                        SELECT 
                            *,
                            ROW_NUMBER() OVER (ORDER BY uploadDate DESC) AS RowNum
                        FROM NewsPosts WHERE content LIKE @search
                    ) AS _
                    WHERE RowNum > {offset} AND RowNum <= {offset + PAGE_SIZE}
                    """;

                using (var command = new SqlCommand(readQuery, databaseConnection.GetConnection()))
                {
                    command.Parameters.AddWithValue("@search", $"%{searchedText}%");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var post = new Post
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                AuthorId = Convert.ToInt32(reader["authorId"]),
                                Content = reader["content"].ToString(),
                                UploadDate = Convert.ToDateTime(reader["uploadDate"]),
                                NrLikes = Convert.ToInt32(reader["nrLikes"]),
                                NrDislikes = Convert.ToInt32(reader["nrDislikes"]),
                                NrComments = Convert.ToInt32(reader["nrComments"])
                            };
                            followingPosts.Add(post);
                        }
                    }
                }

                foreach (var post in followingPosts)
                {
                    string scalarQuery = $"SELECT ratingType FROM Ratings WHERE postId={post.Id} AND authorId={userId}";
                    using (var command = new SqlCommand(scalarQuery, databaseConnection.GetConnection()))
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int ratingType = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader[0]);
                            post.ActiveUserRating = ratingType == 1; // true for positive rating (1), false for others
                        }
                        else
                        {
                            post.ActiveUserRating = null; // No rating found for this user
                        }
                    }
                }

                return followingPosts;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: The rest of the posts could not be loaded: " + ex.Message);
            }
            finally
            {
                databaseConnection.Disconnect();
            }
        }
    }
}