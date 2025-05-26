using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;

namespace BusinessLayer.Repositories
{
    public class FriendRequestRepository : IFriendRequestRepository
    {
        private DatabaseConnection databaseConnection;

        public FriendRequestRepository()
        {
            databaseConnection = new DatabaseConnection();
        }

        public async Task<IEnumerable<FriendRequest>> GetFriendRequestsAsync(string username)
        {
            return await Task.Run(() =>
            {
                var result = new List<FriendRequest>();

                // Define the SQL query to retrieve friend requests
                string query = @"
                    SELECT SenderUsername, SenderEmail, SenderProfilePhotoPath, RequestDate
                    FROM FriendRequests
                    WHERE ReceiverUsername = @ReceiverUsername";

                databaseConnection.Connect();
                try
                {
                    var dataSet = databaseConnection.ExecuteQuery(query, "FriendRequests");

                    using (var command = new SqlCommand(query, databaseConnection.GetConnection()))
                    {
                        command.Parameters.AddWithValue("@ReceiverUsername", username);
                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataSet, "FriendRequests");
                        }
                    }

                    foreach (DataRow row in dataSet.Tables["FriendRequests"].Rows)
                    {
                        result.Add(new FriendRequest
                        {
                            Username = row["SenderUsername"].ToString(),
                            Email = row["SenderEmail"].ToString(),
                            ProfilePhotoPath = row["SenderProfilePhotoPath"].ToString(),
                            RequestDate = Convert.ToDateTime(row["RequestDate"]),
                            ReceiverUsername = username
                        });
                    }
                }
                finally
                {
                    databaseConnection.Disconnect();
                }

                return result;
            });
        }

        public async Task<bool> AddFriendRequestAsync(FriendRequest request)
        {
            return await Task.Run(() =>
            {
                try
                {
                    databaseConnection.Connect();
                    try
                    {
                        // Define the SQL query to insert a friend request
                        string query = @"
                            INSERT INTO FriendRequests (SenderUsername, SenderEmail, SenderProfilePhotoPath, ReceiverUsername, RequestDate)
                            VALUES (@SenderUsername, @SenderEmail, @SenderProfilePhotoPath, @ReceiverUsername, @RequestDate)";

                        using (var command = new SqlCommand(query, databaseConnection.GetConnection()))
                        {
                            command.Parameters.AddWithValue("@SenderUsername", request.Username);
                            command.Parameters.AddWithValue("@SenderEmail", request.Email);
                            command.Parameters.AddWithValue("@SenderProfilePhotoPath", request.ProfilePhotoPath);
                            command.Parameters.AddWithValue("@ReceiverUsername", request.ReceiverUsername);
                            command.Parameters.AddWithValue("@RequestDate", request.RequestDate);
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

        public async Task<bool> DeleteFriendRequestAsync(string senderUsername, string receiverUsername)
        {
            return await Task.Run(() =>
            {
                try
                {
                    databaseConnection.Connect();
                    try
                    {
                        // Define the SQL query to delete a friend request
                        string query = @"
                            DELETE FROM FriendRequests
                            WHERE SenderUsername = @SenderUsername AND ReceiverUsername = @ReceiverUsername";

                        using (var command = new SqlCommand(query, databaseConnection.GetConnection()))
                        {
                            command.Parameters.AddWithValue("@SenderUsername", senderUsername);
                            command.Parameters.AddWithValue("@ReceiverUsername", receiverUsername);
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