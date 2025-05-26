using System.Collections.Generic;
using BusinessLayer.Models;

namespace BusinessLayer.Repositories.Interfaces
{
    public interface IRepository
    {
        List<User> GetUsers(string selectQuery);

        void SendNewMessageRequest(Dictionary<string, object> invite);

        void RemoveMessageRequest(Dictionary<string, object> request);

        void UpdateUserIpAddress(string userIpAddress, int userId);

        bool CheckMessageInviteRequestExistance(int senderUserId, int receiverUserId);

        void CancelAllMessageRequests(int userId);

        List<int> GetInvites(int receiverId);

        void SendFriendRequest(int senderUserId, int receiverUserId);

        void CancelFriendRequest(int senderUserId, int receiverUserId);

        bool CheckFriendRequestExists(int senderUserId, int receiverUserId);

        bool CheckFriendshipExists(int userId1, int userId2);
    }
}