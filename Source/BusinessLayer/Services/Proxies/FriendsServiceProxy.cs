using System;
using System.Collections.Generic;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Services.Proxies
{
    public class FriendsServiceProxy : ServiceProxy, IFriendsService
    {
        private readonly IUserService userService;

        public FriendsServiceProxy(IUserService userService, string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public List<Friendship> GetAllFriendships()
        {
            try
            {
                var currentUserId = userService.GetCurrentUser()?.UserId ??
                    throw new InvalidOperationException("User is not logged in");

                return GetAsync<List<Friendship>>($"Friends/{currentUserId}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error retrieving friendships", ex);
            }
        }

        public void RemoveFriend(int friendshipIdentifier)
        {
            try
            {
                DeleteAsync<object>($"Friends/{friendshipIdentifier}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error removing friend", ex);
            }
        }

        public int GetFriendshipCount(int userIdentifier)
        {
            try
            {
                return GetAsync<int>($"Friends/{userIdentifier}/count").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error retrieving friendship count", ex);
            }
        }

        public bool AreUsersFriends(int userIdentifier1, int userIdentifier2)
        {
            try
            {
                return GetAsync<bool>($"Friends/check?user1={userIdentifier1}&user2={userIdentifier2}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error checking friendship status", ex);
            }
        }

        public int? GetFriendshipIdentifier(int userIdentifier1, int userIdentifier2)
        {
            try
            {
                return GetAsync<int?>($"Friends/id?user1={userIdentifier1}&user2={userIdentifier2}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error retrieving friendship ID", ex);
            }
        }

        public void AddFriend(int userIdentifier, int friendIdentifier)
        {
            try
            {
                PostAsync("Friends", new { UserId = userIdentifier, FriendId = friendIdentifier }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error adding friend", ex);
            }
        }
    }
}