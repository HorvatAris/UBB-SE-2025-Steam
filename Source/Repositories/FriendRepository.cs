using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;

namespace BusinessLayer.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        // REMOVE: private readonly DatabaseConnection _dbConnection;
        private readonly DatabaseConnection databaseConnection;

        // REMOVE: Constructor with injection
        // public FriendRepository(DatabaseConnection dbConnection)
        // {
        //     _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        // }

        // ADD: Parameterless constructor
        public FriendRepository()
        {
            databaseConnection = new DatabaseConnection();
        }

        public async Task<IEnumerable<Friend>> GetFriendsAsync(string username)
        {
            var result = new List<Friend>();

            string query = @"
                -- Get friends where the user is User1
                SELECT u.Username, u.Email, u.ProfilePhotoPath
                FROM Friends f
                JOIN FriendUsers u ON f.User2Username = u.Username
                WHERE f.User1Username = @Username
                
                UNION
                
                -- Get friends where the user is User2
                SELECT u.Username, u.Email, u.ProfilePhotoPath
                FROM Friends f
                JOIN FriendUsers u ON f.User1Username = u.Username
                WHERE f.User2Username = @Username";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", SqlDbType.NVarChar) { Value = username }
            };

            // CHANGED: Use synchronous methods since DatabaseConnection doesn't have async methods
            databaseConnection.Connect();
            try
            {
                var dataSet = databaseConnection.ExecuteQuery(query, "Friends");
                dataSet.Tables["Friends"].Load(new SqlDataAdapter(query, databaseConnection.GetConnection()).SelectCommand);

                foreach (DataRow row in dataSet.Tables["Friends"].Rows)
                {
                    result.Add(new Friend
                    {
                        Username = row["Username"].ToString(),
                        Email = row["Email"].ToString(),
                        ProfilePhotoPath = row["ProfilePhotoPath"]?.ToString()
                    });
                }
            }
            finally
            {
                databaseConnection.Disconnect();
            }

            return result;
        }

        public async Task<bool> AddFriendAsync(string user1Username, string user2Username, string friendEmail, string friendProfilePhotoPath)
        {
            // Since DatabaseConnection doesn't support async, we need to:
            // 1. Either make this method synchronous
            // 2. Or use DataLink instead
            // For now, keeping it async but using synchronous operations internally
            return await Task.Run(() =>
            {
                try
                {
                    databaseConnection.Connect();
                    try
                    {
                        // Using DatabaseConnection.ExecuteUpdate or manual command
                        string sql = @"
                            INSERT INTO Friends (User1Username, User2Username)
                            VALUES (
                                CASE WHEN @User1Username <= @User2Username THEN @User1Username ELSE @User2Username END,
                                CASE WHEN @User1Username <= @User2Username THEN @User2Username ELSE @User1Username END
                            )";

                        using (var command = new SqlCommand(sql, databaseConnection.GetConnection()))
                        {
                            command.Parameters.AddWithValue("@User1Username", user1Username);
                            command.Parameters.AddWithValue("@User2Username", user2Username);
                            command.ExecuteNonQuery();
                        }
                        return true;
                    }
                    finally
                    {
                        databaseConnection.Disconnect();
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        public async Task<bool> RemoveFriendAsync(string user1Username, string user2Username)
        {
            return await Task.Run(() =>
            {
                try
                {
                    databaseConnection.Connect();
                    try
                    {
                        string sql = @"
                            DELETE FROM Friends
                            WHERE (User1Username = @User1Username AND User2Username = @User2Username)
                                OR (User1Username = @User2Username AND User2Username = @User1Username)";

                        using (var command = new SqlCommand(sql, databaseConnection.GetConnection()))
                        {
                            command.Parameters.AddWithValue("@User1Username", user1Username);
                            command.Parameters.AddWithValue("@User2Username", user2Username);
                            command.ExecuteNonQuery();
                        }
                        return true;
                    }
                    finally
                    {
                        databaseConnection.Disconnect();
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }
    }
}