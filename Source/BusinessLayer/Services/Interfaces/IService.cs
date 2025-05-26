using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface IService
    {
        List<User> GetFirst10UsersMatchedSorted(string username);
        string UpdateCurrentUserIpAddress(int userId);
        int MessageRequest(int senderUserId, int receiverUserId);
        List<User> GetUsersWhoSentMessageRequest(int receiverId);
        void HandleMessageAcceptOrDecline(int senderUserId, int receiverUserId);
        List<User> SortAscending(List<User> usersList);
        List<User> SortDescending(List<User> usersList);
        FriendshipStatus GetFriendshipStatus(int currentUserId, int otherUserId);
        void SendFriendRequest(int senderUserId, int receiverUserId);
        void CancelFriendRequest(int senderUserId, int receiverUserId);
        void ToggleFriendRequest(FriendshipStatus friendshipStatus, int senderUserId, int receiverUserId);
        void OnCloseWindow(int userId);
    }
}