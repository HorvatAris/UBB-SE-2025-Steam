using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Repositories;

namespace BusinessLayer.Services
{
    public class Service : IService
    {
        private IRepository repository;

        public const int MAXIMUM_NUMBER_OF_DISPLAYED_USERS = 10;
        public const int MESSAGE_REQUEST_FOUND = 0;
        public const int MESSAGE_REQUEST_NOT_FOUND = 1;
        public const int ERROR_CODE = -1;
        public const int HARDCODED_USER_ID = 1;

        public Service()
        {
            this.repository = new Repository();
        }
        public Service(IRepository repository)
        {
            this.repository = repository;
        }

        public List<User> GetFirst10UsersMatchedSorted(string username)
        {
            try
            {
                string selectQuery = this.GetSelectQueryForUsersByName(username);
                List<User> foundUsers = this.repository.GetUsers(selectQuery);
                foundUsers = this.SortAscending(foundUsers);
                foreach (User user in foundUsers)
                {
                    user.FriendshipStatus = GetFriendshipStatus(currentUserId: HARDCODED_USER_ID, otherUserId: user.UserId);
                }
                return foundUsers.Take(Service.MAXIMUM_NUMBER_OF_DISPLAYED_USERS).ToList();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                return new List<User>();
            }
        }

        public string UpdateCurrentUserIpAddress(int userId)
        {
            try
            {
                string newIpAddress = "none";
                //  BusinessLayer.Services.ChatService.GetLocalIpAddress();
                this.repository.UpdateUserIpAddress(newIpAddress, userId);
                return newIpAddress;
            }
            catch (Exception ex)
            {
                return BusinessLayer.Models.ChatConstants.GET_IP_REPLACER;
            }
        }

        public int MessageRequest(int senderUserId, int receiverUserId)
        {
            if (senderUserId == receiverUserId)
            {
                return Service.ERROR_CODE;
            }

            try
            {
                bool alreadyInvited = this.repository.CheckMessageInviteRequestExistance(senderUserId, receiverUserId);

                Dictionary<string, object> invite = new Dictionary<string, object>();

                invite[Repository.MESSAGE_INVITES_SENDER_ROW] = senderUserId;
                invite[Repository.MESSAGE_INVITES_RECEIVER_ROW] = receiverUserId;

                switch (alreadyInvited)
                {
                    case true:
                        this.repository.RemoveMessageRequest(invite);
                        return Service.MESSAGE_REQUEST_FOUND;
                    case false:
                        this.repository.SendNewMessageRequest(invite);
                        return Service.MESSAGE_REQUEST_NOT_FOUND;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                return Service.ERROR_CODE;
            }
        }

        public List<User> GetUsersWhoSentMessageRequest(int receiverId)
        {
            try
            {
                List<User> foundUsers = new List<User>();
                List<int> foundIds = this.repository.GetInvites(receiverId);

                foreach (int id in foundIds)
                {
                    string selectQuery = this.GetSelectQueryForUsersById(id);
                    List<User> foundUser = this.repository.GetUsers(selectQuery);
                    foundUsers.AddRange(foundUser);
                }

                return foundUsers;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                return new List<User>();
            }
        }

        public void HandleMessageAcceptOrDecline(int senderUserId, int receiverUserId)
        {
            try
            {
                Dictionary<string, object> invite = new Dictionary<string, object>();

                invite.Add(Repository.MESSAGE_INVITES_SENDER_ROW, senderUserId);
                invite.Add(Repository.MESSAGE_INVITES_RECEIVER_ROW, receiverUserId);

                this.repository.RemoveMessageRequest(invite);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        public List<User> SortAscending(List<User> usersList)
        {
            usersList.Sort((User firstUser, User secondUser) => string.Compare(firstUser.Username, secondUser.Username));
            return usersList;
        }

        public List<User> SortDescending(List<User> usersList)
        {
            usersList.Sort((User firstUser, User secondUser) => string.Compare(secondUser.Username, firstUser.Username));
            return usersList;
        }

        public FriendshipStatus GetFriendshipStatus(int currentUserId, int otherUserId)
        {
            try
            {
                // Don't show friend status with yourself
                if (currentUserId == otherUserId)
                {
                    return FriendshipStatus.Friends;
                }

                // Check if users are already friends
                if (this.repository.CheckFriendshipExists(currentUserId, otherUserId))
                {
                    return FriendshipStatus.Friends;
                }

                // Check if current user has sent a friend request to the other user
                if (this.repository.CheckFriendRequestExists(currentUserId, otherUserId))
                {
                    return FriendshipStatus.RequestSent;
                }

                // Check if other user has sent a friend request to the current user
                if (this.repository.CheckFriendRequestExists(otherUserId, currentUserId))
                {
                    return FriendshipStatus.RequestReceived;
                }

                // No relationship exists
                return FriendshipStatus.NotFriends;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                return FriendshipStatus.NotFriends;
            }
        }

        public void SendFriendRequest(int senderUserId, int receiverUserId)
        {
            try
            {
                this.repository.SendFriendRequest(senderUserId, receiverUserId);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        public void CancelFriendRequest(int senderUserId, int receiverUserId)
        {
            try
            {
                this.repository.CancelFriendRequest(senderUserId, receiverUserId);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        public void ToggleFriendRequest(FriendshipStatus friendshipStatus, int senderUserId, int receiverUserId)
        {
            try
            {
                switch (friendshipStatus)
                {
                    case FriendshipStatus.RequestSent:
                        this.CancelFriendRequest(senderUserId, receiverUserId);
                        break;
                    case FriendshipStatus.RequestReceived:
                        this.SendFriendRequest(senderUserId, receiverUserId);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        public void OnCloseWindow(int userId)
        {
            try
            {
                this.repository.CancelAllMessageRequests(userId);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        private string GetSelectQueryForUsersByName(string username)
        {
            return $"SELECT * FROM {Repository.USER_TABLE_NAME} WHERE username LIKE '%{username}%'";
        }

        private string GetSelectQueryForUsersById(int userId)
        {
            return $"SELECT * FROM {Repository.USER_TABLE_NAME} WHERE {Repository.USER_ID_ROW} = {userId}";
        }
    }
}