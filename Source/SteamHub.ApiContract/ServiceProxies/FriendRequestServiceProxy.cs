using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class FriendRequestServiceProxy : ServiceProxy, IFriendRequestService
    {
        public FriendRequestServiceProxy(string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
        }

        public async Task<IEnumerable<FriendRequest>> GetFriendRequestsAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            }

            try
            {
                return await GetAsync<List<FriendRequest>>($"FriendRequest?username={username}");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetFriendRequestsAsync: {ex.Message}");
                return new List<FriendRequest>();
            }
        }

        public async Task<bool> SendFriendRequestAsync(FriendRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.ReceiverUsername))
            {
                throw new ArgumentException("Sender and receiver usernames must be provided");
            }

            try
            {
                await PostAsync("FriendRequest", request);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in SendFriendRequestAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AcceptFriendRequestAsync(string senderUsername, string receiverUsername)
        {
            if (string.IsNullOrEmpty(senderUsername) || string.IsNullOrEmpty(receiverUsername))
            {
                throw new ArgumentException("Sender and receiver usernames must be provided");
            }

            try
            {
                await PostAsync("FriendRequest/accept", new
                {
                    SenderUsername = senderUsername,
                    ReceiverUsername = receiverUsername
                });
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in AcceptFriendRequestAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RejectFriendRequestAsync(string senderUsername, string receiverUsername)
        {
            if (string.IsNullOrEmpty(senderUsername) || string.IsNullOrEmpty(receiverUsername))
            {
                throw new ArgumentException("Sender and receiver usernames must be provided");
            }

            try
            {
                await PostAsync("FriendRequest/reject", new
                {
                    SenderUsername = senderUsername,
                    ReceiverUsername = receiverUsername
                });
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in RejectFriendRequestAsync: {ex.Message}");
                return false;
            }
        }
    }
}