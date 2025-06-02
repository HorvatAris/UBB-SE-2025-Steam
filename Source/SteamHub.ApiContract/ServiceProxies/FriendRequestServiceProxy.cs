using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;
using System.Net.Http.Json;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class FriendRequestServiceProxy : ServiceProxy, IFriendRequestService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public FriendRequestServiceProxy(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
        }

        public async Task<IEnumerable<FriendRequest>> GetFriendRequestsAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            }

            try
            {
                var response = await _httpClient.GetAsync($"FriendRequest?username={username}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<FriendRequest>>(_options);
                return result ?? new List<FriendRequest>();
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
                var response = await _httpClient.PostAsJsonAsync("FriendRequest", request, _options);
                response.EnsureSuccessStatusCode();
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
                var response = await _httpClient.PostAsJsonAsync("FriendRequest/accept", new
                {
                    SenderUsername = senderUsername,
                    ReceiverUsername = receiverUsername
                }, _options);
                response.EnsureSuccessStatusCode();
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
                var response = await _httpClient.PostAsJsonAsync("FriendRequest/reject", new
                {
                    SenderUsername = senderUsername,
                    ReceiverUsername = receiverUsername
                }, _options);
                response.EnsureSuccessStatusCode();
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