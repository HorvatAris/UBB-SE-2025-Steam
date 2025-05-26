using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using BusinessLayer.Data;
using BusinessLayer.Models;

namespace BusinessLayer.Data
{
    public class NewsDatabase : INewsDatabase
    {
        private const string SEARCH_VALUE = "@search";
        private readonly DatabaseConnection databaseConnection;

        public SqlConnection Connection => databaseConnection.GetConnection();
        public string ConnectionString => databaseConnection.GetConnectionString();

        public NewsDatabase()
        {
            databaseConnection = new DatabaseConnection();
        }

        /// <summary>
        /// Open a new database connection
        /// </summary>
        public void Connect()
        {
            try
            {
                databaseConnection.Connect();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Close the database connection
        /// </summary>
        public void Disconnect()
        {
            databaseConnection.Disconnect();
        }

        /// <summary>
        /// Create a new command with the given query and execute it
        /// </summary>
        /// <param name="query">Query to be executed given as string</param>
        /// <returns>Result of the execution</returns>
        public int ExecuteQuery(string query)
        {
            try
            {
                SqlCommand command = new SqlCommand(query, Connection);
                int executionResult = command.ExecuteNonQuery();
                return executionResult;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Create a new command with the query given as string
        /// Create a new reader with the new command and return it
        /// </summary>
        /// <param name="query">Query given as string</param>
        /// <returns>The new reader instance</returns>
        public SqlDataReader ExecuteReader(string query)
        {
            SqlCommand command = new SqlCommand(query, Connection);
            SqlDataReader reader = command.ExecuteReader();
            return reader;
        }

        /// <summary>
        /// Fetch the data of the comments found with the reader and return them as a list
        /// </summary>
        /// <param name="query">Query given as string</param>
        /// <returns>The list of the comments found by the reader</returns>
        public List<Comment> FetchCommentsData(string query)
        {
            List<Comment> fetchedComments = new List<Comment>();
            var reader = ExecuteReader(query);

            while (reader.Read())
            {
                fetchedComments.Add(new()
                {
                    Id = (uint)(int)reader["id"],
                    AuthorId = (uint)(int)reader["authorId"],
                    Body = (string)reader["content"],
                    TimeStamp = ((DateTime)reader["uploadDate"]).ToString(),
                    Score = 0 // Set default score if not in database
                });
            }

            reader.Close();
            return fetchedComments;
        }

        /// <summary>
        /// Create a new command with the query given as string
        /// Create a reader with the new command that will be used to search the news articles
        /// </summary>
        /// <param name="query">Search query given as string</param>
        /// <param name="searchedText">Searched text</param>
        /// <returns>The new reader instance for searching</returns>
        public SqlDataReader ExecuteSearchReader(string query, string searchedText)
        {
            SqlCommand command = new SqlCommand(query, Connection);
            command.Parameters.AddWithValue(SEARCH_VALUE, searchedText == " " ? "%" : $"%{searchedText}%");
            SqlDataReader reader = command.ExecuteReader();
            return reader;
        }

        /// <summary>
        /// Get all the posts that the user searched for, otherwise just return all the posts as a list
        /// </summary>
        /// <param name="query">Search query given as string</param>
        /// <param name="searchedText">Search text to match with the posts</param>
        /// <returns>All the posts found as a list</returns>
        public List<Post> FetchPostsData(string query, string searchedText)
        {
            List<Post> fetchedPosts = new List<Post>();
            var reader = ExecuteSearchReader(query, searchedText);

            while (reader.Read())
            {
                Post post = new()
                {
                    Id = (int)reader["id"],
                    AuthorId = (int)reader["authorId"],
                    Content = (string)reader["content"],
                    UploadDate = (DateTime)reader["uploadDate"],
                    NrLikes = (int)reader["nrLikes"],
                    NrDislikes = (int)reader["nrDislikes"],
                    NrComments = (int)reader["nrComments"]
                };
                fetchedPosts.Add(post);
            }

            reader.Close();
            return fetchedPosts;
        }

        /// <summary>
        /// Create a new command with the query and execute it with the .ExecuteScalar() method
        /// </summary>
        /// <param name="query">Query to be executed given as string</param>
        /// <returns>Result of the execution as bool/null value</returns>
        public bool? ExecuteScalar(string query)
        {
            SqlCommand command = new SqlCommand(query, Connection);
            var executionResult = (bool?)command.ExecuteScalar();
            return executionResult;
        }
    }
}