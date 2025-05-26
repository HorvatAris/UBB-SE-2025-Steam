using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.Models;

namespace BusinessLayer.Repositories
{
    public interface IFriendRequestRepository
    {
        Task<IEnumerable<FriendRequest>> GetFriendRequestsAsync(string username);
        Task<bool> AddFriendRequestAsync(FriendRequest request);
        Task<bool> DeleteFriendRequestAsync(string senderUsername, string receiverUsername);
    }
} 