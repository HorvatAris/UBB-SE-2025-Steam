using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface IFriendService
    {
        Task<IEnumerable<Friend>> GetFriendsAsync(string username);
        Task<bool> AddFriendAsync(string user1Username, string user2Username, string friendEmail, string friendProfilePhotoPath);
        Task<bool> RemoveFriendAsync(string user1Username, string user2Username);
    }
}