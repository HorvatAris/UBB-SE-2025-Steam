using SteamHub.ApiContract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Repositories
{
    public interface IFriendshipsRepository
    {
        Task<List<Friendship>> GetAllFriendshipsAsync(int userId);
        Task AddFriendshipAsync(int userId, int friendId);
        Task<Friendship> GetFriendshipByIdAsync(int friendshipId);
        Task RemoveFriendshipAsync(int friendshipId);
        Task<int> GetFriendshipCountAsync(int userId);
        Task<int?> GetFriendshipIdAsync(int userId, int friendId);
        Task<bool> AddFriendAsync(string user1Username, string user2Username, string friendEmail, string friendProfilePhotoPath);
    }
}
