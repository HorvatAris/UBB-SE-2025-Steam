using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Data;
using BusinessLayer.Repositories.Interfaces;
using Windows.Web.AtomPub;

namespace BusinessLayer.Repositories
{
    public class ForumRepository : IForumRepository
    {
        // REMOVE: private IDatabaseConnection _dbConnection;
        // ADD: private DatabaseConnection databaseConnection;
        private DatabaseConnection databaseConnection;

        // COMMENTED OUT: Old static instance pattern
        // public static IForumRepository ForumRepositoryInstance = new ForumRepository(new DatabaseConnection());
        // public static IForumRepository GetRepoInstance()
        // {
        //     return ForumRepositoryInstance;
        // }

        // UPDATED: Remove parameter, initialize DatabaseConnection internally
        public ForumRepository()
        {
            databaseConnection = new DatabaseConnection();
        }

        // COMMENTED OUT: Old constructor with injection
        // private ForumRepository(IDatabaseConnection dbConnection)
        // {
        //     _dbConnection = dbConnection;
        // }

        private string TimeSpanFilterToString(TimeSpanFilter filter)
        {
            switch (filter)
            {
                case TimeSpanFilter.Day:
                    return "DAY";
                case TimeSpanFilter.Week:
                    return "WEEK";
                case TimeSpanFilter.Month:
                    return "MONTH";
                case TimeSpanFilter.Year:
                    return "YEAR";
                default:
                    return "";
            }
        }

        public List<ForumPost> GetTopPosts(TimeSpanFilter filter)
        {
            string query;
            switch (filter)
            {
                case TimeSpanFilter.AllTime:
                    query = "SELECT TOP 20 * FROM ForumPosts ORDER BY score DESC";
                    break;
                default:
                    query = $"SELECT TOP 20 * FROM ForumPosts WHERE creation_date >= DATEADD({TimeSpanFilterToString(filter)}, -1, GETDATE()) ORDER BY score DESC";
                    break;
            }
            databaseConnection.Connect();
            DataSet dataSet = databaseConnection.ExecuteQuery(query, "ForumPosts");
            List<ForumPost> posts = new();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                ForumPost post = new()
                {
                    Id = Convert.ToUInt32(row["post_id"]),
                    Title = Convert.ToString(row["title"]),
                    Body = Convert.ToString(row["body"]),
                    Score = Convert.ToInt32(row["score"]),
                    TimeStamp = Convert.ToString(row["creation_date"]),
                    AuthorId = Convert.ToUInt32(row["author_id"]),
                    GameId = row.IsNull("game_id") ? null : Convert.ToUInt32(row["game_id"]),
                };

                posts.Add(post);
            }
            databaseConnection.Disconnect();
            return posts;
        }

        public void CreatePost(string title, string body, uint authorId, string date, uint? gameId)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("title", title);
            data.Add("body", body);
            data.Add("author_id", (int)authorId);
            data.Add("creation_date", date);
            data.Add("score", 0);
            data.Add("game_id", gameId != null ? (int)gameId : DBNull.Value);
            databaseConnection.Connect();
            databaseConnection.ExecuteInsert("ForumPosts", data);
            databaseConnection.Disconnect();
        }

        public void DeletePost(uint postId)
        {
            databaseConnection.Connect();
            databaseConnection.ExecuteDelete("ForumPosts", "post_id", (int)postId);
            databaseConnection.Disconnect();
        }

        public void CreateComment(string body, uint postId, string date, uint authorId)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("body", body);
            data.Add("post_id", (int)postId);
            data.Add("creation_date", date);
            data.Add("author_id", (int)authorId);
            data.Add("score", 0);
            databaseConnection.Connect();
            databaseConnection.ExecuteInsert("ForumComments", data);
            databaseConnection.Disconnect();
        }

        public void DeleteComment(uint commentId)
        {
            databaseConnection.Connect();
            databaseConnection.ExecuteDelete("ForumComments", "comment_id", (int)commentId);
            databaseConnection.Disconnect();
        }

        private int getPostScore(uint id)
        {
            string query = $"SELECT score FROM ForumPosts WHERE post_id = {id}";
            //_dbConnection.Connect();
            DataSet dataSet = databaseConnection.ExecuteQuery(query, "ForumPosts");
            //_dbConnection.Disconnect();
            var score = dataSet.Tables[0].Rows[0]["score"];
            return Convert.ToInt32(score);
        }

        private int getCommentScore(uint id)
        {
            string query = $"SELECT score FROM ForumComments WHERE comment_id = {id}";
            //_dbConnection.Connect();
            DataSet dataSet = databaseConnection.ExecuteQuery(query, "ForumComments");
            //_dbConnection.Disconnect();
            var score = dataSet.Tables[0].Rows[0]["score"];

            return Convert.ToInt32(score);
        }

        public void VoteOnPost(uint postId, int voteValue, int userId)
        {

            int newScore = getPostScore(postId) + voteValue;

            databaseConnection.Connect();

            DataSet likedPost = databaseConnection.ExecuteQuery(
        $"SELECT * FROM UserLikedPost WHERE userId = {userId} AND post_id = {postId}",
        "UserLikedPost");

            DataSet dislikedPost = databaseConnection.ExecuteQuery(
        $"SELECT * FROM UserDislikedPost WHERE userId = {userId} AND post_id = {postId}",
        "UserDislikedPost");

            Dictionary<string, object> data = new Dictionary<string, object>(); 
            data.Add("userId", userId);
            data.Add("post_id", (int)postId);

            if (likedPost.Tables[0].Rows.Count == 0 && dislikedPost.Tables[0].Rows.Count == 0)
            {
                // User has not voted yet, apply the vote normally and insert into the appropriate table
                databaseConnection.ExecuteUpdate("ForumPosts", "score", "post_id", newScore, (int)postId);

                if (voteValue > 0)
                {
                    // User liked the post
                    databaseConnection.ExecuteInsert("UserLikedPost", data);
                }
                else
                {
                    // User disliked the post
                    databaseConnection.ExecuteInsert("UserDislikedPost", data);
                }
            }
            else
            {
                // User has already voted, check if it's the same or opposite vote
                if (likedPost.Tables[0].Rows.Count > 0)
                {
                    if (voteValue > 0)
                    {
                        // Same vote, retract the like
                        databaseConnection.ExecuteDelete("UserLikedPost", "userId", (int)userId);
                        newScore = getPostScore(postId) - voteValue; // Adjust the post score
                    }
                    else
                    {
                        // Opposite vote, retract the like and add a dislike
                        databaseConnection.ExecuteDelete("UserLikedPost", "userId", (int)userId);
                        databaseConnection.ExecuteInsert("UserDislikedPost", data);
                        newScore = getPostScore(postId) + 2 * voteValue; // Adjust the post score accordingly
                    }
                }
                else if (dislikedPost.Tables[0].Rows.Count > 0)
                {
                    if (voteValue < 0)
                    {
                        // Same vote, retract the dislike
                        databaseConnection.ExecuteDelete("UserDislikedPost", "userId", (int)userId);
                        newScore = getPostScore(postId) - voteValue; // Adjust the post score
                    }
                    else
                    {
                        // Opposite vote, retract the dislike and add a like
                        databaseConnection.ExecuteDelete("UserDislikedPost", "userId", (int)userId);
                        databaseConnection.ExecuteInsert("UserLikedPost", data);
                        newScore = getPostScore(postId) + 2 * voteValue; // Adjust the post score accordingly
                    }
                }
            }

            // Update the post score
            databaseConnection.ExecuteUpdate("ForumPosts", "score", "post_id", newScore, (int)postId);

            databaseConnection.Disconnect();
        }

        public void VoteOnComment(uint commentId, int voteValue, int userId)
        {
            int newScore = getCommentScore(commentId) + voteValue;

            databaseConnection.Connect();

            DataSet likedComment = databaseConnection.ExecuteQuery(
        $"SELECT * FROM UserLikedComment WHERE userId = {userId} AND comment_id = {commentId}",
        "UserLikedComment");

            DataSet dislikedComment = databaseConnection.ExecuteQuery(
        $"SELECT * FROM UserDislikedComment WHERE userId = {userId} AND comment_id = {commentId}",
        "UserDislikedComment");

            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("userId", userId);
            data.Add("comment_id", (int)commentId);

            if (likedComment.Tables[0].Rows.Count == 0 && dislikedComment.Tables[0].Rows.Count == 0)
            {
                // User has not voted yet, apply the vote normally and insert into the appropriate table
                databaseConnection.ExecuteUpdate("ForumComments", "score", "comment_id", newScore, (int)commentId);

                if (voteValue > 0)
                {
                    // User liked the comment
                    databaseConnection.ExecuteInsert("UserLikedComment", data);
                }
                else
                {
                    // User disliked the comment
                    databaseConnection.ExecuteInsert("UserDislikedComment", data);
                }
            }
            else
            {
                // User has already voted, check if it's the same or opposite vote
                if (likedComment.Tables[0].Rows.Count > 0)
                {
                    if (voteValue > 0)
                    {
                        // Same vote, retract the like
                        databaseConnection.ExecuteDelete("UserLikedComment", "userId", (int)userId);
                        newScore = getCommentScore(commentId) - voteValue; // Adjust the comment score
                    }
                    else
                    {
                        // Opposite vote, retract the like and add a dislike
                        databaseConnection.ExecuteDelete("UserLikedComment", "userId", (int)userId);
                        databaseConnection.ExecuteInsert("UserDislikedComment", data);
                        newScore = getCommentScore(commentId) + 2 * voteValue; // Adjust the comment score accordingly
                    }
                }
                else if (dislikedComment.Tables[0].Rows.Count > 0)
                {
                    if (voteValue < 0)
                    {
                        // Same vote, retract the dislike
                        databaseConnection.ExecuteDelete("UserDislikedComment", "userId", (int)userId);
                        newScore = getCommentScore(commentId) - voteValue; // Adjust the comment score
                    }
                    else
                    {
                        // Opposite vote, retract the dislike and add a like
                        databaseConnection.ExecuteDelete("UserDislikedComment", "userId", (int)userId);
                        databaseConnection.ExecuteInsert("UserLikedComment", data);
                        newScore = getCommentScore(commentId) + 2 * voteValue; // Adjust the comment score accordingly
                    }
                }
            }

            // Update the comment score
            databaseConnection.ExecuteUpdate("ForumComments", "score", "comment_id", newScore, (int)commentId);

            databaseConnection.Disconnect();
        }

#nullable enable
        public List<ForumPost> GetPagedPosts(uint pageNumber, uint pageSize, bool positiveScoreOnly = false, uint? gameId = null, string? filter = null)
        {
            string query = $"SELECT * FROM ForumPosts WHERE 1 = 1";

            if (gameId != null)
                query += $" AND game_id = {gameId}";

            if (positiveScoreOnly)
                query += " AND score >= 0";

            if (!string.IsNullOrEmpty(filter))
                query += $" AND title LIKE '%{filter}%'";

            query += $" ORDER BY creation_date OFFSET {pageNumber * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            databaseConnection.Connect();
            DataSet dataSet = databaseConnection.ExecuteQuery(query, "ForumPosts");
            List<ForumPost> posts = new();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                ForumPost post = new()
                {
                    Id = Convert.ToUInt32(row["post_id"]),
                    Title = Convert.ToString(row["title"]),
                    Body = Convert.ToString(row["body"]),
                    Score = Convert.ToInt32(row["score"]),
                    TimeStamp = Convert.ToString(row["creation_date"]),
                    AuthorId = Convert.ToUInt32(row["author_id"]),
                    GameId = row.IsNull("game_id") ? null : Convert.ToUInt32(row["game_id"]),
                };

                posts.Add(post);
            }
            databaseConnection.Disconnect();
            return posts;
        }

        public List<ForumComment> GetComments(uint postId)
        {
            string query = $"SELECT * FROM ForumComments WHERE post_id = {postId} ORDER BY creation_date";
            databaseConnection.Connect();
            DataSet dataSet = databaseConnection.ExecuteQuery(query, "ForumComments");
            List<ForumComment> comments = new();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                ForumComment comment = new()
                {
                    Id = Convert.ToUInt32(row["comment_id"]),
                    Body = Convert.ToString(row["body"]),
                    Score = Convert.ToInt32(row["score"]),
                    TimeStamp = Convert.ToString(row["creation_date"]),
                    AuthorId = Convert.ToUInt32(row["author_id"])
                };
                comments.Add(comment);
            }
            databaseConnection.Disconnect();
            return comments;
        }
    }
}