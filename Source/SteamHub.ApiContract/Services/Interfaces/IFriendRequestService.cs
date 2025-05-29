using System.Collections.Generic;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models;

namespace SteamHub.ApiContract.Services.Interfaces
{
    public interface IFriendRequestService
    {
        Task<IEnumerable<FriendRequest>> GetFriendRequestsAsync(string username);
        Task<bool> SendFriendRequestAsync(FriendRequest request);
        Task<bool> AcceptFriendRequestAsync(string senderUsername, string receiverUsername);
        Task<bool> RejectFriendRequestAsync(string senderUsername, string receiverUsername);
    }
}