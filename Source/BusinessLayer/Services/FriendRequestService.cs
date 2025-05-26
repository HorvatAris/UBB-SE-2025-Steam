using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services
{
    public class FriendRequestService : IFriendRequestService
    {
        private readonly IFriendRequestRepository friendRequestRepository;
        private readonly IFriendService friendService;

        public FriendRequestService(IFriendRequestRepository friendRequestRepository, IFriendService friendService)
        {
            this.friendRequestRepository = friendRequestRepository ?? throw new ArgumentNullException(nameof(friendRequestRepository));
            this.friendService = friendService ?? throw new ArgumentNullException(nameof(friendService));
        }

        public async Task<IEnumerable<FriendRequest>> GetFriendRequestsAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            }

            return await friendRequestRepository.GetFriendRequestsAsync(username);
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

            // Set the request date to now
            request.RequestDate = DateTime.Now;

            return await friendRequestRepository.AddFriendRequestAsync(request);
        }

        public async Task<bool> AcceptFriendRequestAsync(string senderUsername, string receiverUsername)
        {
            if (string.IsNullOrEmpty(senderUsername) || string.IsNullOrEmpty(receiverUsername))
            {
                throw new ArgumentException("Sender and receiver usernames must be provided");
            }

            // Get the friend request details before deleting it
            var requests = await friendRequestRepository.GetFriendRequestsAsync(receiverUsername);
            FriendRequest requestToAccept = null;

            foreach (var request in requests)
            {
                if (request.Username == senderUsername)
                {
                    requestToAccept = request;
                    break;
                }
            }

            if (requestToAccept == null)
            {
                // Request not found
                return false;
            }

            // First, add as friend
            bool friendAdded = await friendService.AddFriendAsync(
                senderUsername,
                receiverUsername,
                requestToAccept.Email,
                requestToAccept.ProfilePhotoPath);

            if (!friendAdded)
            {
                return false;
            }

            // Then delete the friend request
            return await friendRequestRepository.DeleteFriendRequestAsync(senderUsername, receiverUsername);
        }

        public async Task<bool> RejectFriendRequestAsync(string senderUsername, string receiverUsername)
        {
            if (string.IsNullOrEmpty(senderUsername) || string.IsNullOrEmpty(receiverUsername))
            {
                throw new ArgumentException("Sender and receiver usernames must be provided");
            }

            // Simply delete the friend request
            return await friendRequestRepository.DeleteFriendRequestAsync(senderUsername, receiverUsername);
        }
    }
}